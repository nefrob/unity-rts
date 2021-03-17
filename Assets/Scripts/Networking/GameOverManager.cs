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
        bases.Remove(b);
        if (bases.Count != 1) return;

        // TODO: if you play twice in a row, game over manager exception here.
        // Not sure why, could be action event handlers aren't cleared properly.
        // Shouldn't be the case since scene is reloaded in between.

        string name = bases[0].connectionToClient.identity.GetComponent<Player>().GetDisplayName();
        RpcGameOver(name);

        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
