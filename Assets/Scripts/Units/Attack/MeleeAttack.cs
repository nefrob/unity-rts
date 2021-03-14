using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MeleeAttack : Attack
{
    [SerializeField] private GameObject weaponObject = null;
    [SerializeField] private Collider weaponCollider = null;
    [SerializeField] private float weaponDisplayTime = 0.25f;

    private float remainingActiveTime;

    [SyncVar(hook = nameof(ClientHandleWeaponActive))]
    private bool syncWeaponActive;

    #region server

    [ServerCallback]
    private void Start()
    {
        remainingActiveTime = 0.0f;
        syncWeaponActive = false;
    }

    [ServerCallback]
    private void Update()
    {
        if (remainingActiveTime > 0) 
        {
            remainingActiveTime -= Time.deltaTime;
            if (remainingActiveTime <= 0)
            {
                weaponObject.SetActive(false);
                syncWeaponActive = false;
            }
        }
    }

    [Server]
    protected override void DoAttack()
    {
        weaponObject.SetActive(true);
        weaponCollider.enabled = true;
        remainingActiveTime = weaponDisplayTime;
        syncWeaponActive = true;
    }

    #endregion

    #region client

    [Client]
    private void ClientHandleWeaponActive(bool oldStatus, bool newStatus)
    {
        weaponObject.SetActive(newStatus);
        // weaponCollider.enabled = newStatus; // FIXME: need this?
    }

    #endregion
}
