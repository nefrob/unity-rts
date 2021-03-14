using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MeleeAttack : Attack
{
    [SerializeField] private GameObject weaponObject = null;
    [SerializeField] private Collider weaponCollider = null;
    [SerializeField] private float weaponDisplayTime = 0.25f;
    [SerializeField] private float attackDamage = 10.0f;

    private float remainingActiveTime;

    [SyncVar(hook = nameof(ClientHandleWeaponActive))]
    private bool syncWeaponActive;

    #region server

    [ServerCallback]
    private void Start()
    {
        remainingActiveTime = 0.0f;
    }

    [ServerCallback]
    protected override void Update()
    {
        base.Update();
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

    [Server]
    public void OnWeaponHit(Collider other) 
    {
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity identity))
        {
            if (identity.connectionToClient == connectionToClient) return; // self hit, ignore
        }
        
        if (other.TryGetComponent<Health>(out Health health))
        {
            health.Damage(attackDamage);
            weaponCollider.enabled = false;
        }
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("collision");
    }
    
    #endregion

    #region client

    [Client]
    private void ClientHandleWeaponActive(bool oldStatus, bool newStatus)
    {
        Debug.Log($"server says weapon: {newStatus}");
        weaponObject.SetActive(newStatus);
        // weaponCollider.enabled = newStatus; // FIXME: need this?
    }

    #endregion
}
