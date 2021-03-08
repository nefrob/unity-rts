using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Attack : NetworkBehaviour
{
    [SerializeField] protected Targeter targeter = null;
    [SerializeField] protected Transform attackOrigin = null;
    [SerializeField] protected float attackSpeed = 1.0f;
    [SerializeField] protected float attackRange = 1.0f;
    [SerializeField] protected float attackDamage = 10.0f;

    [SerializeField] protected float rotationSpeed = 10.0f;

    protected float lastAttackTime;

    #region server

    [ServerCallback]
    private void Start()
    {
        lastAttackTime = -1.0f;
    }

    [ServerCallback]
    private void Update()
    {
        if (targeter.GetTarget() == null) return;

        if (!InRange()) return;

        Quaternion targetRot = Quaternion.LookRotation(
            targeter.GetTarget().transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetRot,
            rotationSpeed * Time.deltaTime);

        if (CanAttack())
        {
            DoAttack();
            lastAttackTime = Time.time;
        }
    }

    [Server]
    protected virtual void DoAttack() { }

    [Server]
    private bool InRange()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude
            <= attackRange * attackRange;
    }

    [Server]
    private bool CanAttack() 
    { 
        return Time.time > (1 / attackSpeed) + lastAttackTime;
    }

    #endregion
}
