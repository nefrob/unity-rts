using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float lifetime = 5.0f;
    [SerializeField] private float force = 10.0f;

    void Start()
    {
        rb.velocity = transform.forward * force;
    }

    #region server

    public override void OnStartServer()
    {
        Invoke(nameof(DestoryProjectile), lifetime);
    }

    [Server]
    private void DestoryProjectile()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

}
