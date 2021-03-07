using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    private List<Unit> units = new List<Unit>();

    public List<Unit> GetAllUnits()
    {
        return units;
    }

    #region server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawn += ServerHandleUnitSpawn;
        Unit.ServerOnUnitDespawn += ServerHandleUnitDespawn;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawn -= ServerHandleUnitSpawn;
        Unit.ServerOnUnitDespawn -= ServerHandleUnitDespawn;
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

    #endregion

    #region client

    public override void OnStartClient()
    {
        if (!isClientOnly) return;

        Unit.AuthoryOnUnitSpawn -= AuthoryHandleUnitSpawn;
        Unit.AuthoryOnUnitDespawn -= AuthoryHandleUnitDespawn;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly) return;

        Unit.AuthoryOnUnitSpawn -= AuthoryHandleUnitSpawn;
        Unit.AuthoryOnUnitDespawn -= AuthoryHandleUnitDespawn;
    }

    private void AuthoryHandleUnitSpawn(Unit unit)
    {
        if (!hasAuthority) return;

        units.Add(unit);
    }

    private void AuthoryHandleUnitDespawn(Unit unit)
    {
        if (!hasAuthority) return;

        units.Remove(unit);
    }

    #endregion
}
