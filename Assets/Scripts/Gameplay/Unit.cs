using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Selectable
{
    private NavMeshAgent agent;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Maybe migrate to function called on all selected items
        if (this.IsSelected() && Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    // TODO: if decide want world position on ground layer
    // void TryWorldMove(Vector3 pos)
    // {
    //     Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(pos);
    //     RaycastHit hit;

    //     if (Physics.Raycast(mouseWorldPos, Camera.main.transform.forward,
    //         out hit, Mathf.Infinity, groundMask))
    //     {
    //         agent.SetDestination(hit.point);
    //     }
    // }
}
