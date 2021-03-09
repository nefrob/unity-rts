﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField] private Building[] availableBuildings = new Building[0];

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;

    public event Action<int> ClientOnResourcesUpdated;

    private List<Unit> units = new List<Unit>();
    private List<Building> buildings = new List<Building>();

    public List<Unit> GetAllUnits()
    {
        return units;
    }

    public List<Building> GetAllBuildings()
    {
        return buildings;
    }

    public int GetResources()
    {
        return resources;
    }

    #region server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawn += ServerHandleUnitSpawn;
        Unit.ServerOnUnitDespawn += ServerHandleUnitDespawn;
        Building.ServerOnBuildingSpawn += ServerHandleBuildingSpawn;
        Building.ServerOnBuildingDespawn += ServerHandleBuildingDespawn;

    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawn -= ServerHandleUnitSpawn;
        Unit.ServerOnUnitDespawn -= ServerHandleUnitDespawn;
        Building.ServerOnBuildingSpawn -= ServerHandleBuildingSpawn;
        Building.ServerOnBuildingDespawn -= ServerHandleBuildingDespawn;

    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach (Building building in availableBuildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) return;

        GameObject buildingInstance =
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    private void ServerHandleUnitSpawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        units.Add(unit);
    }

    private void ServerHandleUnitDespawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        units.Remove(unit);
    }

    private void ServerHandleBuildingSpawn(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) 
        { 
            return; 
        }

        buildings.Add(building);
    }

    private void ServerHandleBuildingDespawn(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) 
        { 
            return; 
        }

        buildings.Remove(building);
    }

    #endregion

    #region client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) return;

        Unit.AuthoryOnUnitSpawn -= AuthoryHandleUnitSpawn;
        Unit.AuthoryOnUnitDespawn -= AuthoryHandleUnitDespawn;
        Building.AuthorityOnBuildingSpawn += AuthorityHandleBuildingSpawn;
        Building.AuthorityOnBuildingDespawn += AuthorityHandleBuildingDespawn;

    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) return;

        Unit.AuthoryOnUnitSpawn -= AuthoryHandleUnitSpawn;
        Unit.AuthoryOnUnitDespawn -= AuthoryHandleUnitDespawn;
        Building.AuthorityOnBuildingSpawn -= AuthorityHandleBuildingSpawn;
        Building.AuthorityOnBuildingDespawn -= AuthorityHandleBuildingDespawn;

    }

    private void AuthoryHandleUnitSpawn(Unit unit)
    {
        units.Add(unit);
    }

    private void AuthoryHandleUnitDespawn(Unit unit)
    {
        units.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawn(Building building)
    {
        buildings.Add(building);
    }

    private void AuthorityHandleBuildingDespawn(Building building)
    {
        buildings.Remove(building);
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    #endregion
}
