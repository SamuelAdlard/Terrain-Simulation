using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public GameObject Tile;
    public Material[] materials;
    public int seed;
    public Tile[,] Tiles = new Tile[100,100];
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
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {

                Tiles[x, y] = new Tile();
                Tiles[x, y].tile = Instantiate(Tile, new Vector3(x, 0, y), Quaternion.identity);
                Tiles[x, y].tile.name = x.ToString() + y.ToString() + " " + Mathf.PerlinNoise(x * 0.025f, y * 0.025f);
                
                if (Mathf.PerlinNoise((x + seed) * 0.05f, (y + seed) * 0.05f) <= 0.4f)
                {
                    Tiles[x, y].type = types.sea;
                    
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

    

    
   
}

public class Tile
{
    public GameObject tile;
    public Generation.types type = 0;



    public float waterreductionRate = 1;
    public float water = 0f;
    public float temperature = 20f;
}
