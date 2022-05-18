using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public GameObject Tile;
    public GameObject Cloud;
    public Material[] materials;
    public int seed;
    public int size = 50;
    public Tile[,] Tiles = new Tile[50,50];
    public Cloud[,] Clouds = new Cloud[50, 50];
    float nextTick = 0;
    public float delay = 2;
    public enum types
    {
        sea,
        desert,
        grass,
        forest,
        freshwater,
        mountain
    }

    void Start()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Clouds[x, y] = new Cloud();
                Clouds[x, y].cloud = Instantiate(Cloud, new Vector3(x, 2, y), Quaternion.identity);
                Clouds[x, y].cloud.SetActive(false);
                Tiles[x, y] = new Tile();
                Tiles[x, y].tile = Instantiate(Tile, new Vector3(x, 0, y), Quaternion.identity);
                Tiles[x, y].temperature = y * 0.05f + 10 + Random.Range(-5, 6);
                if (Tiles[x, y].type == types.sea)
                {
                    Tiles[x, y].temperature += 5;
                }
                Tiles[x, y].tile.name = x.ToString() + y.ToString() + " " + Mathf.PerlinNoise(x * 0.025f, y * 0.025f) + " " + Tiles[x, y].temperature;
               
                if (Mathf.PerlinNoise((x + seed) * 0.05f, (y + seed) * 0.05f) <= 0.4f)
                {
                    Tiles[x, y].type = types.sea;
                    Tiles[x, y].tile.transform.position = new Vector3(Tiles[x, y].tile.transform.position.x, -0.1f, Tiles[x, y].tile.transform.position.z);
                }
                else if (Mathf.PerlinNoise((x + seed) * 0.05f, (y + seed) * 0.05f) > 0.4f && Mathf.PerlinNoise((x + seed) * 0.05f, (y + seed) * 0.05f) < 0.6f)
                {
                    Tiles[x, y].type = types.desert;
                    
                }
                else
                {
                    Tiles[x, y].tile.transform.localScale = new Vector3(1, 2, 1);
                    Tiles[x, y].type = types.mountain;
                }
                Tiles[x, y].tile.GetComponent<MeshRenderer>().material = materials[(int)Tiles[x, y].type];
            }
        }
    }

    private void Update()
    {

        if (nextTick < Time.time)
        {
            print("Hi");
            nextTick = Time.time + delay;
            Simulate();
        }
       
        
    }

    void Simulate()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                
                if (Clouds[x, y].cloud.activeSelf == true)
                {
                    if (Tiles[x, y].type != types.sea)
                    {
                        if(Clouds[x, y].water > 2)
                        {
                            Tiles[x, y].water += 2;
                            Clouds[x, y].water -= 2;
                        }
                        else
                        {
                            Tiles[x, y].water += Clouds[x, y].water;
                            Clouds[x, y].water = 0;
                        }
                    }
                    
                    Vector2[] neighbours = GetNeighbours(x, y);
                    Vector2 bestPos = new Vector2(x, y);
                    float lowestTemp = 100;
                    foreach (Vector2 neighbour in neighbours)
                    {
                        
                        if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].temperature <= lowestTemp && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type != types.mountain)
                        {
                            

                            bestPos = neighbour;
                            lowestTemp = Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].temperature;
                        }
                    }
                    
                    Clouds[x, y].cloud.SetActive(false);
                    Clouds[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].water += Clouds[x, y].water;
                    Clouds[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].isCloud = true;
                    Clouds[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].cloud.SetActive(true);
                    
                }

                if (Tiles[x, y].type == types.sea)
                {
                    Clouds[x, y].water += Tiles[x, y].temperature * 0.05f + Random.Range(-15,6);
                    
                }

                if (Clouds[x, y].water > 2.5f)
                {
                    Clouds[x, y].isCloud = true;
                    Clouds[x, y].cloud.SetActive(true);

                }
                else
                {
                    Clouds[x, y].isCloud = false;
                    Clouds[x, y].cloud.SetActive(false);
                }

                if (Tiles[x, y].water >= 3)
                {
                    Tiles[x, y].type = types.grass;
                    
                }
                else if(Tiles[x, y].water >= 5)
                {
                    Tiles[x, y].type = types.forest;
                }
                Tiles[x, y].temperature = y * 0.05f + 10 + Random.Range(-5, x);
                Tiles[x, y].tile.GetComponent<MeshRenderer>().material = materials[(int)Tiles[x, y].type];
            }
        }
    }
    Vector2[] GetNeighbours(int x, int y)
    {
        Vector2[] neighbours = new Vector2[4];
        if (x < size - 1)
        {
            neighbours[0] = new Vector2(x + 1, y);
        }
        else
        {
            neighbours[0] = new Vector2(size + 1, size);
        }

        if (x > 0)
        {
            neighbours[1] = new Vector2(x - 1, y);
        }
        else
        {
            neighbours[1] = new Vector2(size + 1, size);
        }

        if (y < size - 1)
        {
            neighbours[2] = new Vector2(x, y + 1);
        }
        else
        {
            neighbours[2] = new Vector2(size + 1, size);
        }

        if (y > 0)
        {
            neighbours[3] = new Vector2(x, y - 1);
        }
        else
        {
            neighbours[3] = new Vector2(size + 1, size);
        }

        return neighbours;
    }
}

public class Cloud
{
    public bool isCloud = false;
    public float water;
    public GameObject cloud;
    
}
public class Tile
{
    public GameObject tile;
    public Generation.types type = 0;



    public float waterreductionRate = 1;
    public float water = 0f;
    public float temperature = 40f;
}
