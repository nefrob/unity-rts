using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverManager : NetworkBehaviour
{
    private List<Base> bases = new List<Base>();

    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    #region server

    public override void OnStartServer()
    {
        Base.ServerOnBaseSpawn += ServerHandleBaseSpawn;
        Base.ServerOnBaseDespawn += ServerHandleBaseDespawn;
    }

    public override void OnStopServer()
    {
        Base.ServerOnBaseSpawn -= ServerHandleBaseSpawn;
        Base.ServerOnBaseDespawn -= ServerHandleBaseDespawn;
    }

    [Server]
    private void ServerHandleBaseSpawn(Base b)
    {
        bases.Add(b);
    }

    [Server]
    private void ServerHandleBaseDespawn(Base b)
    {
        Debug.Log($"on base despawn, {bases.Count}");
        bases.Remove(b);
        if (bases.Count != 1) return;

        int playerId = bases[0].connectionToClient.connectionId; 
        RpcGameOver($"Player {playerId}");

        Debug.Log("server on game over");
        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        Debug.Log("rpc game over");
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
