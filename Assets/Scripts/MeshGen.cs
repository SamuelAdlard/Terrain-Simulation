using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float seed = 0;
    Vertex[,] vertices = new Vertex[100, 100];
    Vector3[] newVertices = new Vector3[10000];
    Vector2[] newUV = new Vector2[10000];
    int[] triangles = new int[58806];
    public float scale = 0.5f;
    void Start()
    {
        Mesh mesh = new Mesh();
        

        for (int f = 0; f < newUV.Length; f++)
        {
            newUV[f] = new Vector2(0,0);
        }
        
        
        int i = 0;
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                vertices[x, y] = new Vertex();
                Vector3 position = new Vector3(x, 0, y);
                vertices[x, y].index = i;
                vertices[x, y].position = position;
                newVertices[i] = position;
                
                i++;
            }
        }

        i = 0;
        int j = 0;
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                if (x < 99 && y > 99)
                {
                    triangles[j] = vertices[x, y].index;
                    triangles[j + 1] = vertices[x, y + 1].index;
                    triangles[j + 2] = vertices[x + 1, y].index;
                    triangles[j] = vertices[x + 1, y].index;
                    triangles[j + 1] = vertices[x, y + 1].index;
                    triangles[j + 2] = vertices[x + 1, y + 1].index;
                }
                

                j += 6;
                i++;
            }
        }

        mesh.Clear();
        mesh.vertices = newVertices;

        mesh.triangles = triangles;
        mesh.uv = newUV;
        mesh.name = "Land";
        mesh.RecalculateNormals();
        meshFilter.mesh.Clear();
        meshFilter.mesh = mesh;

        foreach (int item in meshFilter.mesh.triangles)
        {
            print(item);
        }

        

    }

    
}

public class Vertex
{
    public int index;
    public Vector3 position;
}

