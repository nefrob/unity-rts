using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Selectable
{
    // Navigation agent
    private NavMeshAgent agent;

    // Targets eligible for follow
    [SerializeField] private LayerMask targetMask = 0;
    [SerializeField] private string targetTag = "Unit";
    private Transform target;

    // Map vision
    [SerializeField] private float visionDist = 5.0f;
    [SerializeField] private MeshFilter visionMeshFilter = null;
    private Mesh  visionMesh;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        target = null;
        GenerateVisionMesh();
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

    private void GenerateVisionMesh(int points = 10)
    {
        List<Vector3> verts = new List<Vector3> { };
        float x;
        float y;
        for (int i = 0; i < points; i ++)
        {
            x = visionDist * Mathf.Sin((2 * Mathf.PI * i) / points);
            y = visionDist * Mathf.Cos((2 * Mathf.PI * i) / points);
            verts.Add(new Vector3(x, y, 0f));
        }

        List<int> triangles = new List<int> { };
        for(int i = 0; i < (points - 2); i++)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
        }

        visionMesh = new Mesh();
        visionMeshFilter.mesh = visionMesh;
        visionMesh.vertices  = verts.ToArray();
        visionMesh.triangles = triangles.ToArray();
    }
}
