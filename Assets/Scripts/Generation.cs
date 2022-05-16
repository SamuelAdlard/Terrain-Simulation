using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public GameObject Tile;
    public Tile[,] Tiles = new Tile[10,10];
    void Start()
    {
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                Tiles[x, y].tile = Instantiate(Tile, new Vector3(x, 0, y), Quaternion.identity);
                Tiles[x, y].tile.name = x.ToString() + y.ToString();
            }
        }
    }

    int RandomType(int x, int y)
    {
        int RandomNumber = 0;
        if (x == 0 && y == 0)
        {
            RandomNumber = Random.Range(0, 2);
            return RandomNumber;
        }
        else if(x == 0)
        {
            if (Tiles[x, y - 1].type == 0)
            {
                RandomNumber = Random.Range(0, 5);
                if (RandomNumber >= 3)
                { 
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if(Tiles[x, y - 1].type == 5)
            {
                RandomNumber = Random.Range(0, 2);
                if (RandomNumber == 0)
                {
                    return 1;
                }
                else
                {
                    return 5;
                }
            }
            else
            {
                RandomNumber = Random.Range(0, 5);
                if ()
                {

                }
            }
        }

    }    
   
}

public class Tile
{
    public GameObject tile;
    public int type = 0;
    //0 = sea
    //1 = desert
    //2 = grass
    //3 = forest
    //4 = fresh water
    //5 = mountain
    public float water = 0f;
    
}
