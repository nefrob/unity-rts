using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float lifetime = 5.0f;
    [SerializeField] private float attackDamage = 10.0f;

    [SyncVar(hook = nameof(ClientHandleVelocityUpdated))]
    private Vector3 velocity;

    private void Start()
    {
        rb.velocity = velocity;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(rb.velocity);

        // TODO: object tracking for moving accuracy?
        // var dir = target.position - transform.position;
        // dir.y = 0;
        // float adjustStrength = 1f;
        // Vector3 v = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        // float magnitude = v.magnitude;
        // Vector3 newVelocity = (adjustStrength * dir.normalized + (1 - adjustStrength) 
        //     * v.normalized) * magnitude;
        // newVelocity.y = rb.velocity.y;
        // rb.AddForce(newVelocity, ForceMode.VelocityChange);
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

    #region client

    [Client]
    private void ClientHandleVelocityUpdated(Vector3 oldVelocity, Vector3 newVelocity)
    {
        rb.velocity = newVelocity;
    }

    #endregion
}
