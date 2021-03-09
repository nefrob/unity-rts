﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float lifetime = 5.0f;
    [SerializeField] private float attackDamage = 10.0f;

    #region server

    public override void OnStartServer()
    {
        Invoke(nameof(DestroyProjectile), lifetime);
    }

    [Server]
    public void SetInitialVelocity(Vector3 velocity)
    {
        rb.velocity = velocity;
    }

    [ServerCallback]
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(rb.velocity);

        //? object tracking for moving accuracy?
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity identity))
        {
            if (identity.connectionToClient == connectionToClient) return; // self hit
        }

        if (other.TryGetComponent<Health>(out Health health))
        {
            health.Damage(attackDamage);
        }

        DestroyProjectile();
    }

    [Server]
    private void DestroyProjectile()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

}
