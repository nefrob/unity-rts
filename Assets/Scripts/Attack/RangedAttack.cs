using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RangedAttack : Attack
{
    [SerializeField] private GameObject projectilePrefab = null;
    
    #region server

    [ServerCallback]
    protected override void DoAttack()
    {
        Quaternion projectileRot = Quaternion.LookRotation(
            targeter.GetTarget().GetTargetPoint().position - attackOrigin.position);

        GameObject projectileInstance = Instantiate(projectilePrefab,
            attackOrigin.position, projectileRot);

        NetworkServer.Spawn(projectileInstance, connectionToClient); 
    }

    #endregion

}
