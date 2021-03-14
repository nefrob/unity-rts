using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBasePrefab = null;
    [SerializeField] private GameObject gameOverManagerPrefab = null;
    [SerializeField] private Color[] teamColors = null;
    [SerializeField] private string gameScene = "TestScene";

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public List<Player> Players { get; } = new List<Player>();

    private bool isGameInProgress = false;

    #region server

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) return;
        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (Players.Count == 0) return;
        Player player = conn.identity.GetComponent<Player>();
        Players.Remove(player);
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();
        isGameInProgress = false;
    }

    [Server]
    public void StartGame()
    {
        if (Players.Count < 2) return;

        isGameInProgress = true;
        ServerChangeScene(gameScene);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        Player player = conn.identity.GetComponent<Player>();
        Players.Add(player);
        player.SetDisplayName($"Player {Players.Count}");
        player.SetTeamColor(teamColors[Players.Count - 1]);
        player.SetPartyOwner(Players.Count == 1);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith(gameScene))
        {
            GameObject gameOverHandlerInstance = Instantiate(gameOverManagerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance);

            foreach(Player player in Players)
            {
                GameObject baseInstance = Instantiate(
                    unitBasePrefab,
                    GetStartPosition().position,
                    Quaternion.identity);

                NetworkServer.Spawn(baseInstance, player.connectionToClient);
            }
        }
    }

    #endregion

    #region client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion
}
