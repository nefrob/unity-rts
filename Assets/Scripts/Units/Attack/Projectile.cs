using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float lifetime = 5.0f;
    [SerializeField] private float attackDamage = 10.0f;
    [SerializeField] private bool lookVelocityDir = true;
    [SerializeField] private bool deleteAfterTrigger = true;

    [SyncVar(hook = nameof(ClientHandleVelocityUpdated))]
    private Vector3 velocity;

    private void Start()
    {
        rb.velocity = velocity;
    }

    private void Update()
    {
        if (lookVelocityDir) transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    #region server

    public override void OnStartServer()
    {
        Invoke(nameof(DestroyProjectile), lifetime);
    }

    [Server]
    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

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
        }

        // Keep visual but stop further triggers
        if (deleteAfterTrigger) DestroyProjectile();
        else GetComponent<Collider>().enabled = false; // stop further triggers
    }

    [Server]
    private void DestroyProjectile()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region client

    [Client]
    private void ClientHandleVelocityUpdated(Vector3 oldVelocity, Vector3 newVelocity)
    {
        rb.velocity = newVelocity;
    }

    #endregion
}
