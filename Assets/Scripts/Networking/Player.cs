using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField] private Building[] availableBuildings = new Building[0];
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private float buildingRangeLimit = 2.0f;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 200;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    public event Action<int> ClientOnResourcesUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    private Color teamColor = new Color();
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

    public Color GetTeamColor()
    {
        return teamColor;
    }

    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(
                    point + buildingCollider.center,
                    buildingCollider.size / 2,
                    Quaternion.identity,
                    buildingBlockLayer))
        {
            return false;
        }

        foreach (Building building in buildings)
        {
            if ((point - building.transform.position).sqrMagnitude
                <= buildingRangeLimit * buildingRangeLimit)
            {
                return false;
            }
        }

        return true;
    }

    #region server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawn += ServerHandleUnitSpawn;
        Unit.ServerOnUnitDespawn += ServerHandleUnitDespawn;
        Building.ServerOnBuildingSpawn += ServerHandleBuildingSpawn;
        Building.ServerOnBuildingDespawn += ServerHandleBuildingDespawn;

        DontDestroyOnLoad(gameObject);
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

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
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

        if (resources < buildingToPlace.GetPrice()) return;

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
        if (!CanPlaceBuilding(buildingCollider, point)) return;

        GameObject buildingInstance =
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetResources(resources - buildingToPlace.GetPrice());
    }

    [Server]
    private void ServerHandleUnitSpawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        units.Add(unit);
    }

    [Server]
    private void ServerHandleUnitDespawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        units.Remove(unit);
    }

    [Server]
    private void ServerHandleBuildingSpawn(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) 
        { 
            return; 
        }

        buildings.Add(building);
    }

    [Server]
    private void ServerHandleBuildingDespawn(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) 
        { 
            return; 
        }

        buildings.Remove(building);
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) return;
        ((RTSNetworkManager)NetworkManager.singleton).StartGame();
    }


    #endregion

    #region client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) return;

        Unit.AuthoryOnUnitSpawn += AuthoryHandleUnitSpawn;
        Unit.AuthoryOnUnitDespawn += AuthoryHandleUnitDespawn;
        Building.AuthorityOnBuildingSpawn += AuthorityHandleBuildingSpawn;
        Building.AuthorityOnBuildingDespawn += AuthorityHandleBuildingDespawn;
    }

     public override void OnStartClient()
    {
        if (NetworkServer.active) return;

        DontDestroyOnLoad(gameObject);

        ((RTSNetworkManager) NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) return;

        ((RTSNetworkManager) NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) return;

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

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) return;
        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    #endregion
}
