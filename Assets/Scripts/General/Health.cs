using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100.0f;

    public event Action ServerOnDie;
    public event Action<float, float> ClientOnHealthUpdate;

    [SyncVar(hook=nameof(HandleHealthUpdate))]
    private float curHealth;

    #region server

    public override void OnStartServer()
    {
        curHealth = maxHealth;

        Base.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer()
    {
        Base.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void ServerHandlePlayerDie(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId) return;

        Damage(curHealth);
    }

    [Server]
    public void Damage(float damage)
    {
        if (curHealth == 0) return;

        curHealth = Mathf.Max(curHealth - damage, 0);
        if (curHealth > 0) return;

        Debug.Log("health is zero");

        ServerOnDie?.Invoke();
    }

    #endregion

    #region client

    [Client]
    private void HandleHealthUpdate(float oldHealth, float newHealth)
    {
        ClientOnHealthUpdate?.Invoke(newHealth, maxHealth);
    }

    #endregion
}
