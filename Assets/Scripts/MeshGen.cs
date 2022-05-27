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
    Vector3[] newNormals = new Vector3[10000];
    int[] triangles = new int[58806];
    public float mountainHeight = 0.6f;
    public float seaLevel = 0.3f;
    public float scale = 0.5f;
    public float waterMeshHeight = 0;
    void Start()
    {
        Mesh mesh = new Mesh();
        

        for (int f = 0; f < newUV.Length; f++)
        {
            newUV[f] = new Vector2(0,0);
            newNormals[f] = new Vector3(0,1,0);
        }
        
        
        int i = 0;
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                float noise = Mathf.PerlinNoise((x + seed) * scale, (y + seed) * scale);

                float tileHeight = noise;
                Vector3 position;
                vertices[x, y] = new Vertex();
                if (tileHeight <= seaLevel)
                {
                    position = new Vector3(x, waterMeshHeight, y);
                }
                else if(tileHeight >= mountainHeight)
                {
                    position = new Vector3(x, Mathf.Pow(tileHeight * 3, 3),y);
                }
                else
                {
                    position = new Vector3(x, tileHeight * 10, y);
                }
                
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
                if (x < 98 && y < 98)
                {
                    triangles[j] = vertices[x, y].index;
                    triangles[j + 1] = vertices[x, y + 1].index;
                    triangles[j + 2] = vertices[x + 1, y].index;
                    triangles[j + 3] = vertices[x + 1, y].index;
                    triangles[j + 4] = vertices[x, y + 1].index;
                    triangles[j + 5] = vertices[x + 1, y + 1].index;
                }
                

                j += 6;
                i++;
            }
        }

        print(triangles[i]);

        mesh.Clear();
        mesh.vertices = newVertices;
        mesh.triangles = triangles;
        mesh.uv = newUV;
        mesh.name = "Land";
        mesh.normals = newNormals;
        
        meshFilter.mesh = mesh;
        

        

        

    }

    
}

public class Vertex
{
    public int index;
    public Vector3 position;
}

