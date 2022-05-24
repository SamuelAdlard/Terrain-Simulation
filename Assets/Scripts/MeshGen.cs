using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float seed = 0;
    Vertex[,] vertices = new Vertex[100, 100];
    Vector3[] newVertices = new Vector3[10000];
    int[][] triangles = new int[3][];
    public float scale = 0.5f;
    void Start()
    {
        Mesh mesh = new Mesh();
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
                print(i);
                i++;
            }
        }

        mesh.vertices = newVertices;
        //mesh.triangles = ;
        mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
        print(newVertices[0]);

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    
}

public class Vertex
{
    public int index;
    public Vector3 position;
}

