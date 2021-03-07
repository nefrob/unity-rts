using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Unit: Selectable
{
    [SerializeField] private UnitMovement unitMovement = null;

    [SerializeField] private Targeter targeter = null;

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

    #region server

    public override void OnStartServer()
    {
        ServerOnUnitSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawn?.Invoke(this);
    }

    #endregion

    #region client

    public override void OnStartClient()
    {
        if (!isClientOnly || !hasAuthority) return;

        AuthoryOnUnitSpawn?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) return;

        AuthoryOnUnitDespawn?.Invoke(this);
    }

    #endregion
}
