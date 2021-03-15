using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RangedAttack : Attack
{
    [SerializeField] private GameObject projectilePrefab = null;
    [Range(20.0f, 75.0f)] public float launchAngle = 30.0f; // FIXME: use randomized launch angle? or angle to get over wall?
    [SerializeField] private bool isMeleeProjectile = false; // TODO: make actual melee attack type
    
    #region server

    [Server]
    protected override void DoAttack()
    {
        Vector3 lookDir = targeter.GetTarget().GetTargetPoint().position - attackOrigin.position;
        if (isMeleeProjectile) lookDir.y = 0;
        Quaternion projectileRot = Quaternion.LookRotation(lookDir);

        GameObject projectileInstance = Instantiate(projectilePrefab,
            attackOrigin.position, projectileRot);

        Vector3 velocity = Vector3.zero;
        if (!isMeleeProjectile) velocity = ComputeLaunchVelocity();
        projectileInstance.GetComponent<Projectile>().SetVelocity(velocity);

        NetworkServer.Spawn(projectileInstance, connectionToClient); 
    }

    // Ref: https://vilbeyli.github.io/Projectile-Motion-Tutorial-for-Arrows-and-Missiles-in-Unity3D/
    [Server]
    private Vector3 ComputeLaunchVelocity()
    {
        Transform target = targeter.GetTarget().GetTargetPoint();

        Vector3 projectileXZPos = new Vector3(attackOrigin.position.x, 0.0f, attackOrigin.position.z);
        Vector3 targetXZPos = new Vector3(target.position.x, 0.0f, target.position.z);
    
        transform.LookAt(targetXZPos);

        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(launchAngle * Mathf.Deg2Rad);
        float H = target.position.y - transform.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)) );
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        return globalVelocity;
    }

    #endregion
}
