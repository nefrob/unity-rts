using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Building : NetworkBehaviour
{
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;
    [SerializeField] private GameObject buildingPreview = null;

    public static event Action<Building> ServerOnBuildingSpawn;
    public static event Action<Building> ServerOnBuildingDespawn;

    public static event Action<Building> AuthorityOnBuildingSpawn;
    public static event Action<Building> AuthorityOnBuildingDespawn;

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetId()
    {
        return id;
    }

    public int GetPrice()
    {
        return price;
    }

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

    #region server

    public override void OnStartServer()
    {
        ServerOnBuildingSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawn?.Invoke(this);
    }

    #endregion

    #region client

    [ClientCallback]
    private void Update()
    {
        Vector3 currentPos = transform.position;
        transform.position = new Vector3(
            Mathf.Round(currentPos.x),
            Mathf.Round(currentPos.y), 
            Mathf.Round(currentPos.z));
    }

    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawn?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) return;

        AuthorityOnBuildingDespawn?.Invoke(this);
    }

    #endregion
}
