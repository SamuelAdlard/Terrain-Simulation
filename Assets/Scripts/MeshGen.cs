using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
    public MeshFilter meshFiter;
    Mesh mesh = new Mesh();
    int[,] vertices = new int[33, 33];
    Vector3[] newVertices = new Vector3[1089];
    public float scale = 0.5f;
    void Start()
    {
        int side = 0;
        int y = 0;
        int x = 0;
        for (int i = 0; i < meshFiter.mesh.vertexCount; i++)
        {
            vertices[x, y] = i;
            x++;
            
            if(i <= side + 33)
            {
                side += 33;
                x = 0;
                y++;
            }
            
        }
        
        for (int x1 = 0; x < 33; x1++)
        {
            for (int y1 = 0; y < 33; y1++)
            {
                newVertices[x1 + y1] = new Vector3(0, 0, 0);
            }
        }


    }

   
}
