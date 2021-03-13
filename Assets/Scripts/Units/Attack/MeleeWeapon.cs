using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MeleeWeapon : NetworkBehaviour
{
    [SerializeField] private float attackDamage = 10.0f;
    [SerializeField] private Collider trigger = null;

    #region server

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity identity))
        {
            if (identity.connectionToClient == connectionToClient) return; // self hit, ignore
        }
        
        if (other.TryGetComponent<Health>(out Health health))
        {
            health.Damage(attackDamage);
            trigger.enabled = false; // disable so only one hit per attack time
        }
    }

    #endregion
}
