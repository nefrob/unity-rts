using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Selectable
{
    // Naviagation agent
    private NavMeshAgent agent;

    // Targets eligible for follow
    [SerializeField] private LayerMask targetMask = 0;
    [SerializeField] private string targetTag = "Unit";
    private Transform target;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        target = null;
    }

    void Update()
    {
        // Update tracking
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        // FIXME: migrate to function called on all selected items
        if (this.IsSelected() && Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetMask))
            {
                if (hit.transform.tag == targetTag)
                {
                    target = hit.transform;
                } else 
                {
                    target = null;
                }

                agent.SetDestination(hit.point);
            }
        }
    }
}
