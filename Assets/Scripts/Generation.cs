using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Generation : MonoBehaviour
{
    public GameObject Tile;
    public GameObject Cloud;
    public Material[] materials;
    public int seed;
    public float scale = 0.05f;
    public float mountainLevel = 0.65f;
    
    public int size = 50;
    public Tile[,] Tiles = new Tile[200,200];
    public Cloud[,] Clouds = new Cloud[200, 200];
    public bool[,] Rivers = new bool[200, 200];
    float nextTick = 0;
    public float delay = 2;
    public float waterLevel = 0.35f;
    public int numberOfCycles;
    public MeshGen meshGen;
    public bool continueGeneration = false;
    public bool showSim = true;

    public GameObject button;
    public GameObject Player;
    public GameObject menuCamera;
    public TMP_InputField seedInput;
    public TMP_InputField cyclesInput;


    bool simulating = false;
    int cycles = 0;
    public enum types
    {
        sea,
        desert,
        grass,
        forest,
        freshwater,
        mountain
    }

    public void BeingSimulation()
    {
        try
        {
            seed = int.Parse(seedInput.text);
            numberOfCycles = int.Parse(cyclesInput.text);
            seedInput.gameObject.SetActive(false);
            cyclesInput.gameObject.SetActive(false);
            button.SetActive(false);
        }
        catch
        {
            return;
        }
        
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Clouds[x, y] = new Cloud();
                
                
                Tiles[x, y] = new Tile();
                
                Tiles[x, y].temperature = y * 0.05f + 10 + Random.Range(-2, 3);
                if (showSim)
                {
                    Clouds[x, y].cloud = Instantiate(Cloud, new Vector3(x, 2, y), Quaternion.identity);
                    Clouds[x, y].cloud.SetActive(false);
                    Tiles[x, y].tile = Instantiate(Tile, new Vector3(x, 0, y), Quaternion.identity);
                }
                float noise = Mathf.PerlinNoise((x + seed) * scale, (y + seed) * scale);
                float noise1 = Mathf.PerlinNoise((x + seed / 2) * scale, (y + seed / 2) * scale);
                float noise2 = Mathf.PerlinNoise((x + seed / 3) * scale, (y + seed / 3) * scale);
                float tileHeight = (noise + noise1 + noise2) / 3;

                Tiles[x, y].height = tileHeight;
                if (Tiles[x, y].type == types.sea)
                {
                    Tiles[x, y].temperature += 5;
                }

               
               
                if (tileHeight <= waterLevel)
                {
                    Tiles[x, y].type = types.sea;
                    if (showSim)
                    {
                        Tiles[x, y].tile.transform.position = new Vector3(Tiles[x, y].tile.transform.position.x, -0.1f, Tiles[x, y].tile.transform.position.z);
                    }
                    
                }
                else if (tileHeight > waterLevel && tileHeight < mountainLevel)
                {
                    Tiles[x, y].type = types.desert;
                   
                }
                else
                {
                    if (showSim)
                    {
                        Tiles[x, y].tile.transform.localScale = new Vector3(1, 2, 1);
                    }
                       
                    Tiles[x, y].type = types.mountain;
                }
                
                if (showSim)
                {
                    Tiles[x, y].tile.GetComponent<MeshRenderer>().material = materials[(int)Tiles[x, y].type];
                }
                
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool nearSea = false;
                bool nearMountain = false;
                bool nearRiver = false;
                foreach (Vector2 neighbour in GetNeighbours(x, y))
                {
                    if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.sea)
                    {
                        nearSea = true;
                    }

                    if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.mountain)
                    {
                        nearMountain = true;
                    }

                    if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.freshwater)
                    {
                        nearRiver = true;

                    }
                }

                if (nearMountain && Random.Range(0,10) == 0 && Tiles[x, y].type != types.mountain )
                {
                    Tiles[x, y].water += 500;
                    
                }
            }
        }

        if (continueGeneration)
        {
            meshGen.MakeMesh();
        }
        simulating = true;
    }

    private void Update()
    {
        if(simulating)
        {
            bool simulated = false;
            if (nextTick < Time.time && cycles < numberOfCycles || continueGeneration)
            {
           
                nextTick = Time.time + delay;
                Simulate();
                cycles++;
                if (continueGeneration)
                {
                    meshGen.Simulate();
                }
            }
            print(cycles);
            if (cycles == numberOfCycles - 1 && !continueGeneration && !simulated)
            {
                meshGen.MakeMesh();
                menuCamera.SetActive(false);
                Player.SetActive(true);
                simulated = true;
            }
        }
        
        
        
       
        
    }

    void Simulate()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {

                if (Input.GetKeyDown("g"))
                {
                    if (Tiles[x, y].type == types.sea)
                    {
                        Tiles[x, y].type = types.desert;
                    }
                }

                if (Input.GetKeyDown("f"))
                {
                    if (Tiles[x, y].type == types.forest)
                    {
                        Tiles[x, y].water = 6;
                        Tiles[x, y].type = types.grass;
                    }
                }

                if (Tiles[x, y].type == types.sea)
                {
                    Clouds[x, y].water += Tiles[x, y].temperature * 0.005f;

                }
                else if(Tiles[x, y].type == types.forest)
                {
                    Clouds[x, y].water += Tiles[x, y].temperature * 0.005f;
                }

                if (showSim)
                {
                    Clouds[x, y].cloud.name = Clouds[x, y].water.ToString();
                }
               
                if (Clouds[x, y].water > 5f)
                {
                    Clouds[x, y].isCloud = true;

                    if (showSim)
                    {
                        Clouds[x, y].cloud.SetActive(true);
                    }
                    

                }
                else
                {
                    Clouds[x, y].isCloud = false;
                    if (showSim)
                    {
                        Clouds[x, y].cloud.SetActive(false);
                    }
                    
                }

                bool nearSea = false;
                bool nearMountain = false;
                bool nearRiver = false;
                bool nearDesert = false;
                foreach (Vector2 neighbour in GetNeighbours(x, y))
                {
                    if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.sea)
                    {
                        nearSea = true;
                    }

                    if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.mountain)
                    {
                        nearMountain = true;
                    }

                    if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.freshwater)
                    {
                        nearRiver = true;
                    }

                    if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.desert)
                    {
                        nearDesert = true;
                    }
                }

                

                if (Tiles[x, y].type == types.freshwater)
                {
                    Vector2 bestPos = new Vector2(x, y);
                    float lowestHeight = 2;

                    foreach (Vector2 neighbour in GetNeighbours(x, y))
                    {

                        if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].height <= lowestHeight && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type != types.mountain)
                        {
                            bestPos = neighbour;
                            lowestHeight = Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].height;
                        }

                        if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type != types.mountain && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type != types.sea && Tiles[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].water < 5)
                        {
                            Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].water += 1;
                        }

                        
                    }

                    if (bestPos != new Vector2(x, y) && Tiles[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].type != types.sea)
                    {
                        Tiles[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].water += 100;
                        Tiles[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].type = types.freshwater;
                        
                    }
                    
                }











                if (Clouds[x, y].isCloud == true)
                {
                    if (Tiles[x, y].type != types.sea && Random.Range(0,2) == 1)
                    {

                        
                        
                        if(Clouds[x, y].water > 5)
                        {
                            Tiles[x, y].water += 5;
                            Clouds[x, y].water -= 5;
                        }
                        else
                        {
                            Tiles[x, y].water += Clouds[x, y].water;
                            Clouds[x, y].water = 0;
                            Clouds[x, y].isCloud = false;
                            Clouds[x, y].cloud.SetActive(false);
                        }
                    }
                    
                    Vector2[] neighbours = GetNeighbours(x, y);
                    Vector2 bestPos = new Vector2(x, y);
                    float lowestTemp = 100;                   

                    if (Tiles[x, y].type != types.forest)
                    {
                        foreach (Vector2 neighbour in neighbours)
                        {

                            if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].temperature <= lowestTemp && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type != types.mountain)
                            {
                                bestPos = neighbour;
                                lowestTemp = Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].temperature;
                            }

                            if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.mountain)
                            {
                                nearSea = true;
                                Tiles[x, y].water += Clouds[x, y].water;
                                Clouds[x, y].water = 0;
                                break;
                            }

                           
                        }
                    }
                    else if(Random.Range(0,2) == 1)
                    {
                        foreach (Vector2 neighbour in neighbours)
                        {

                            if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].temperature <= lowestTemp && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type != types.mountain)
                            {
                                bestPos = neighbour;
                                lowestTemp = Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].temperature;
                            }

                            if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.mountain)
                            {
                                Tiles[x, y].water += Clouds[x, y].water;
                                Clouds[x, y].water = 0;
                                break;
                            }
                        }
                    }

                    if (showSim)
                    {
                        Clouds[x, y].cloud.SetActive(false);
                        Clouds[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].cloud.SetActive(true);
                    }
                    
                    Clouds[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].water += Clouds[x, y].water;
                    Clouds[Mathf.RoundToInt(bestPos.x), Mathf.RoundToInt(bestPos.y)].isCloud = true;
                    
                    
                }

                if (Tiles[x, y].type == types.forest && nearDesert)
                {
                    foreach (Vector2 neighbour in GetAllNeighbours(x, y))
                    {
                        if (neighbour.x < size + 1 && Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].type == types.desert)
                        {
                            Tiles[Mathf.RoundToInt(neighbour.x), Mathf.RoundToInt(neighbour.y)].water += 40;
                        }
                    }
                }



                if (Tiles[x ,y].type != types.sea)
                {
                    if (Tiles[x, y].water >= 5 && Tiles[x, y].water <= 50)
                    {
                        Tiles[x, y].type = types.grass;
                    }
                    else if (Tiles[x, y].water >= 50 && Tiles[x, y].water < 100 && !nearDesert)
                    {
                        Tiles[x, y].type = types.forest;
                    }
                    else if (Tiles[x, y].type != types.mountain && Tiles[x, y].type != types.sea && Tiles[x, y].water < 10)
                    {
                        Tiles[x, y].type = types.desert;
                    }
                }


                


                Tiles[x, y].temperature = y * 0.05f + 10 + Random.Range(-2, 3);
                if (showSim)
                {
                    Tiles[x, y].tile.GetComponent<MeshRenderer>().material = materials[(int)Tiles[x, y].type];
                }
                
                if (Clouds[x, y].water < 0)
                {
                    Clouds[x, y].water = 0;
                }

                if (Clouds[x, y].water > 10)
                {
                    
                    Clouds[x, y].water = 0;
                    Clouds[x, y].isCloud = false;
                    if (showSim)
                    {
                        Clouds[x, y].cloud.SetActive(false);
                    }
                    

                }

                if (Tiles[x, y].water > 0)
                {
                    Tiles[x, y].water -= Tiles[x, y].temperature * 0.0025f;
                }
                
            }
        }
    }


    Vector2[] GetAllNeighbours(int x, int y)
    {
        Vector2[] neighbours = new Vector2[8];
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

        if (y > 0 && x > 0)
        {
            neighbours[4] = new Vector2(x - 1, y - 1);
        }
        else
        {
            neighbours[4] = new Vector2(size + 1, size);
        }

        if (y > 0 && x < size - 1)
        {
            neighbours[5] = new Vector2(x + 1 , y - 1);
        }
        else
        {
            neighbours[5] = new Vector2(size + 1, size);
        }

        if (y < size - 1 && x > 0)
        {
            neighbours[6] = new Vector2(x - 1, y + 1);
        }
        else
        {
            neighbours[6] = new Vector2(size + 1, size);
        }

        if (y < size - 1 && x < size - 1)
        {
            neighbours[7] = new Vector2(x + 1, y + 1);
        }
        else
        {
            neighbours[7] = new Vector2(size + 1, size);
        }

        return neighbours;
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

    
    public float height = 0;
    public float waterreductionRate = 1;
    public float water = 0f;
    public float temperature = 40f;
}
