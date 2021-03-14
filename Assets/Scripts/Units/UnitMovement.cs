using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float minChaseRange = 5.0f;
    [SerializeField] private float maxChaseRange = 10.0f;

    #region server

    public override void OnStartServer()
    {
        GameOverManager.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverManager.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        if (target != null)
        {
            float distSquared = (target.transform.position - transform.position).sqrMagnitude;
            if (distSquared > minChaseRange * minChaseRange 
                && distSquared < maxChaseRange * maxChaseRange)
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
        ServerMove(pos);
    }

    [Server]
    public void ServerMove(Vector3 pos)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            return;
        }

        agent.SetDestination(hit.position);
    }

    [Server]
    private void ServerHandleGameOver()
    {
        if (agent.isOnNavMesh && agent.hasPath) agent.ResetPath();
    }

    #endregion
}
