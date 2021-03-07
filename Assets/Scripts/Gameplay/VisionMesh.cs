using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionMesh : MonoBehaviour
{
    public float visionDist = 5.0f;

    [SerializeField] private MeshFilter visionMeshFilter = null;
    [SerializeField] private MeshCollider visionCollider = null;
    private Mesh visionMesh;
    
    private void Awake()
    {
        visionMeshFilter = GetComponent<MeshFilter>();
        visionCollider = GetComponent<MeshCollider>();
        visionMesh = new Mesh();
        GenerateVisionMesh();
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

        visionMeshFilter.mesh = visionMesh;
        visionMesh.vertices  = verts.ToArray();
        visionMesh.triangles = triangles.ToArray();

        visionCollider.sharedMesh = visionMesh;
    }

    // TODO: need teams support for vision tracking below

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player 2")
        {
            // TODO: enable mesh renderer and increase ref count on p2 object
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // TODO: decrease vision ref count on other player's object
        // TODO: if it hits zero, disable visuals for the object
    }
}
