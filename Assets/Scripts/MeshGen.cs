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
    Color[] colours = new Color[10000];
    int[] triangles = new int[58806];
    public Color[] Colours;
    public Vector2[] colourXY;
    public float mountainHeight = 0.6f;
    public float scale = 0.5f;
    public float waterMeshHeight = 0;
    public bool vertexColour = false;
    public Generation terrain;
    public Material vertexColourMat;
    public void MakeMesh()
    {
        Mesh mesh = new Mesh();
        

        for (int f = 0; f < newUV.Length; f++)
        {
            
            newNormals[f] = new Vector3(0,1,0);
        }

        if (vertexColour)
        {
            gameObject.GetComponent<MeshRenderer>().material = vertexColourMat;
        }
        
        int i = 0;
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                //float noise = Mathf.PerlinNoise((x + seed) * scale, (y + seed) * scale);
                float noise = terrain.Tiles[x, y].height; 
                
                float seaLevel = terrain.waterLevel;
                float tileHeight = noise;
                Vector3 position;
                vertices[x, y] = new Vertex();
                if (tileHeight <= seaLevel)
                {
                    
                    position = new Vector3(x, waterMeshHeight, y);
                }
                else if(tileHeight >= mountainHeight)
                {
                    
                    position = new Vector3(x, Mathf.Pow(tileHeight * 2.75f, 3),y);
                }
                else
                {
                    
                    position = new Vector3(x, tileHeight * 10, y);
                }
                if (vertexColour == true)
                {
                    colours[i] = Colours[(int)terrain.Tiles[x, y].type];
                }
                else
                {
                    newUV[i] = UV(colourXY[(int)terrain.Tiles[x, y].type].x, colourXY[(int)terrain.Tiles[x, y].type].y);
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
        if (vertexColour)
        {
            mesh.SetColors(colours);
        }
        
        
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        

        

        

    }

    Vector2 UV(float Chunkx, float Chunky)
    {
        Chunkx = Mathf.RoundToInt(Chunkx);
        Chunky = Mathf.RoundToInt(Chunky);
        const float chunksize = 0.25f;
        float UVstartx;
        float UVstarty;
        float UVendx;
        float UVendy;
        UVstartx = chunksize * Chunkx + 0.05f;
        UVstarty = chunksize * Chunky + 0.05f;
        UVendx = UVstartx + chunksize - 0.1f;
        UVendy = UVstarty + chunksize - 0.1f;
        return new Vector2((UVstartx + UVendx) / 2, (UVstarty + UVendy) / 2);        
    }

}

public class Vertex
{
    public int index;
    public Vector3 position;
}

