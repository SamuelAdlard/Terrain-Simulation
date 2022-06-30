using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

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
    public GameObject tree;

    public GameObject Cactus;
    public float scale = 0.5f;
    public float waterMeshHeight = 0;
    public bool vertexColour = false;
    public bool bitMapTexture = false;
    public int width = 100, height = 100;
    public float mountainMeshHeight = 10f;
    public Generation terrain;
    public Material vertexColourMat;
    public Texture2D texture;
    Mesh mesh;
    int numberOfCycles = 0;
    public void MakeMesh()
    {
        texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        mesh = new Mesh();
        
        
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
                    
                    position = new Vector3(x, Mathf.Pow(tileHeight * mountainMeshHeight, 3),y);
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
                    
                    newUV[i] = UV(x, y);
                }


                if (terrain.Tiles[x, y].type == Generation.types.forest)
                {
                    Instantiate(tree, position + new Vector3(Random.Range(-0.5f,0.5f), -0.1f, Random.Range(-0.5f, 0.5f)), Quaternion.Euler(-90,Random.Range(0f ,360f),0)).transform.parent = transform;
                }

                if (terrain.Tiles[x, y].type == Generation.types.desert && Random.Range(0,5) == 1)
                {
                    Instantiate(Cactus, position + new Vector3(Random.Range(-0.5f,0.5f), -0.1f, Random.Range(-0.5f, 0.5f)), Quaternion.Euler(-90,Random.Range(0f ,360f),0)).transform.parent = transform;
                }

                vertices[x, y].index = i;
                vertices[x, y].position = position;
                newVertices[i] = position;
                
                i++;
            }
        }
        
        MakeTexture();

        i = 0;
        int j = 0;
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                if (x < 98 && y < 98)
                {
                    
                    

                    
                    if (terrain.Tiles[x ,y].type != Generation.types.sea || terrain.Tiles[x, y + 1].type != Generation.types.sea || terrain.Tiles[x + 1, y].type != Generation.types.sea)
                    {
                        triangles[j] = vertices[x, y].index;
                        triangles[j + 1] = vertices[x, y + 1].index;
                        triangles[j + 2] = vertices[x + 1, y].index;
                    }

                    if (terrain.Tiles[x + 1, y].type != Generation.types.sea || terrain.Tiles[x, y + 1].type != Generation.types.sea || terrain.Tiles[x + 1, y + 1].type != Generation.types.sea)
                    {
                        triangles[j + 3] = vertices[x + 1, y].index;
                        triangles[j + 4] = vertices[x, y + 1].index;
                        triangles[j + 5] = vertices[x + 1, y + 1].index;
                    }
                    
                }
                

                j += 6;
                i++;
            }
        }

        print(triangles[i]);

        UpdateMesh();
        gameObject.AddComponent<MeshCollider>();
        transform.localScale = new Vector3(10, 10, 10);
    }

    void UpdateMesh()
    {
        mesh.vertices = newVertices;
        mesh.triangles = triangles;
        mesh.uv = newUV;
        mesh.name = "Land";
        texture.Apply();
        if (vertexColour)
        {
            mesh.SetColors(colours);
        }

        if (bitMapTexture)
        {
            gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
        }

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }



    void MakeTexture()
    {
        for (int x = 0; x < 1000; x++)
        {
            for (int y = 0; y < 1000; y++)
            {
                texture.SetPixel(x, y, Colours[(int)terrain.Tiles[x / 10, y / 10].type]);
            }
        }
    }



    
    public void Simulate()
    {
        numberOfCycles++;
        if (terrain.continueGeneration)
        {
            int i = 0;
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    if (vertexColour == true)
                    {
                        colours[i] = Colours[(int)terrain.Tiles[x, y].type];
                        
                    }
                    else
                    {
                        newUV[i] = UV(colourXY[(int)terrain.Tiles[x, y].type].x, colourXY[(int)terrain.Tiles[x, y].type].y);
                        
                        
                    }

                    i++;
                }
            }
        }

        
    }


    Vector2 UV(float Chunkx, float Chunky)
    {
        Chunkx = Mathf.RoundToInt(Chunkx);
        Chunky = Mathf.RoundToInt(Chunky);
        const float chunksize = 0.01f;
        float UVstartx;
        float UVstarty;
        float UVendx;
        float UVendy;
        UVstartx = chunksize * Chunkx + 0.0001f;
        UVstarty = chunksize * Chunky + 0.0001f;
        UVendx = UVstartx + chunksize - 0.0001f;
        UVendy = UVstarty + chunksize - 0.0001f;
        return new Vector2((UVstartx + UVendx) / 2, (UVstarty + UVendy) / 2);        
    }

    

}

public class Vertex
{
    public int index;
    public Vector3 position;
}

