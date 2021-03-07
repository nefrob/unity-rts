using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

    [SerializeField] private LayerMask targetMask = 0;

    [SerializeField] private Targeter targeter = null;

    [SerializeField] float chaseRange = 10.0f;

    #region server

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude
                > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            } else if (agent.hasPath)
            {
                agent.ResetPath();
            }

            return;
        }

        if (!agent.hasPath) return; // stop clear path on set frame

        // Stop fighting to get to same location
        if (agent.remainingDistance > agent.stoppingDistance) return;
        agent.ResetPath();
    }

    [Command]
    public void CmdMoveUnit(Vector3 pos)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            return;
        }

        agent.SetDestination(hit.position);
    }

    #endregion

    // #region Client

    // public override void OnStartAuthority()
    // {
    //     base.OnStartAuthority();
    //     cam = Camera.main;
    // }

    // [ClientCallback]
    // private void Update()
    // {
    //     if (!hasAuthority) return;

    //     // TODO: !this.isSelected() && 
    //     if (!Mouse.current.rightButton.wasPressedThisFrame) return;

    //     Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    //     RaycastHit hit;

    //     if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetMask))
    //     {
    //         CmdMoveUnit(hit.point);
    //     }
    // }

    // #endregion
}
