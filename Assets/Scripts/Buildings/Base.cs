using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Base : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<Base> ServerOnBaseSpawn;
    public static event Action<Base> ServerOnBaseDespawn;
    public static event Action<int> ServerOnPlayerDie;

    #region server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnBaseSpawn?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawn?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }

    #endregion
}
