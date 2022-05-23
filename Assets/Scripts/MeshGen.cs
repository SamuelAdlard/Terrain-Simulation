using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float seed = 0;
    
    int[,] vertices = new int[34, 34];
    Vector3[] newVertices = new Vector3[1089];
    public float scale = 0.5f;
    void Awake()
    {
        Mesh mesh = new Mesh();
        int side = 0;
        int y = 0;
        int x = 0;
        for (int i = 0; i < meshFilter.mesh.vertexCount; i++)
        {
            print(x + "," + y);
            vertices[x, y] = i;
            x++;
            
            if(i >= side + 33)
            {
                side += 33;
                x = 0;
                y++;
            }
            
        }

        
        
        for (int i = 0; i < 1089; i++)
        {
            newVertices[i] = new Vector3(meshFilter.mesh.vertices[i].x, Mathf.PerlinNoise((x * scale) + seed, (y * scale) + seed) * 1, meshFilter.mesh.vertices[i].x);
        }

        mesh.vertices = newVertices;
        mesh.uv = meshFilter.mesh.uv;
        mesh.triangles = meshFilter.mesh.triangles;
        mesh.normals = meshFilter.mesh.normals;
        meshFilter.mesh = mesh;
    }

   
}
