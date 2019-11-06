using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRock : MonoBehaviour
{
    public int seed = 37828342;
    private Material mat;
    private List<Vector3> vertices;
    private List<Vector3> doneVerts;
    private Vector3 center;
    void Start()
    {
        vertices = new List<Vector3>();
        doneVerts = new List<Vector3>();

        Random.InitState(seed);
        float offset = Random.Range(-4, 4);
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        for (int s = 0; s < mesh.vertices.Length; s++)
        {
            vertices.Add(mesh.vertices[s]);
        }

        center = GetComponent<Renderer>().bounds.center;
        
        for (int v = 0; v < vertices.Count; v++)
        {
            bool used = false;
            for (int i = 0; i < doneVerts.Count; i++)
            {
                if (doneVerts[i] == vertices[v])
                {
                    used = true;
                }
            }
            
            if (!used)
            {
                Vector3 curVector = vertices[v];
                doneVerts.Add(curVector);
                int smoothing = Random.Range(4, 6);
                Vector3 changedVector = (curVector + ((curVector - center) * (Mathf.PerlinNoise((float)v / offset, (float)v / offset) / smoothing)));

                for (int s = 0; s < vertices.Count; s++)
                {
                    if (vertices[s] == curVector)
                        vertices[s] = changedVector;
                }
            }
        }

        mesh.SetVertices(vertices);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
