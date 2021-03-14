using System;
using UnityEngine;
using Mirror;

public class Unit: Selectable
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private Health health = null;
    [SerializeField] private int price = 100;

    public static event Action<Unit> ServerOnUnitSpawn;
    public static event Action<Unit> ServerOnUnitDespawn;

    public static event Action<Unit> AuthoryOnUnitSpawn;
    public static event Action<Unit> AuthoryOnUnitDespawn;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    public int GetPrice()
    {
        return price;
    }

    #region server

    public override void OnStartServer()
    {
        ServerOnUnitSpawn?.Invoke(this);
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        ServerOnUnitDespawn?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region client

    public override void OnStartAuthority()
    {
        AuthoryOnUnitSpawn?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) return;

        AuthoryOnUnitDespawn?.Invoke(this);
    }

    #endregion
}
