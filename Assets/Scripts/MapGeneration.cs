using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public int[,] mapArray;
    [Range(5, 200)] public int minDimensions;
    [Range(10, 500)] public int maxDimensions;
    public GameObject testTile;

    int[,] newMap(int min, int max)
    {
        int[,] newMap = new int[Random.Range(min, max), Random.Range(min, max)];
        return newMap;
    }

    int[] shuffle(int[] list)
    {
        for (int a = 0; a < list.Length; a++)
        {
            int newRand = Random.Range(0, list.Length);
            int saveInt = list[newRand];
            list[newRand] = list[a];
            list[a] = saveInt;
        }
        return list;
    }

    int smoothValue(int[,] tileMap, int x, int y)
    {
        int newValue = tileMap[x, y];
        int maxValue = 9;
        int[] iterate = new int[] { -1, 0, 1 };
        iterate = shuffle(iterate);

        foreach (int a in iterate)
        {
            foreach (int b in iterate)
            {
                try
                {
                    if (tileMap[x + a, y + b] > newValue)
                    {
                        newValue += Random.Range(3,6);
                    }
                    if (tileMap[x + a, y + b] < newValue)
                    {
                        newValue -= Random.Range(3, 6);
                    }
                }
                catch
                {
                    maxValue -= 1;
                }
            }
        }
        if (newValue > 100)
        {
            newValue = 100;
        }
        if (newValue < 0)
        {
            newValue = 0;
        }

        if (maxValue != 9)
        {
            newValue = 0;
        }

        return newValue;
    }

    int sharpenValue(int[,] tileMap, int x, int y)
    {
        int newValue = tileMap[x, y];

        if (newValue > 50)
        {
            int difference = newValue - 50;
            if (Random.Range(0, difference) > 10)
            {
                newValue += Random.Range(20, 50);
            }
        }

        else
        {
            if (Random.Range(0, newValue) < 40)
            {
                newValue -= Random.Range(5, 20);
            }
        }

        if (newValue > 100)
        {
            newValue = 100;
        }
        if (newValue < 0)
        {
            newValue = 0;
        }
        return newValue;
    }

    int[,] randomNumbers(int[,] tileMap, int maxValue, int nullChance)
    {
        for (int a = 0; a < tileMap.GetLength(0); a++)
        {
            for (int b = 0; b < tileMap.GetLength(1); b++)
            {
                if (Random.Range(0, nullChance) == 0)
                {
                    tileMap[a, b] = 100;
                    //tileMap[a, b] = Random.Range(0, maxValue);
                }
            }
        }
        return tileMap;
    }

    int[,] smoothMap(int[,] tileMap, int iterations)
    {
        for (int x = 0; x < iterations; x++)
        {
            for (int a = 0; a < tileMap.GetLength(0); a++)
            {
                for (int b = 0; b < tileMap.GetLength(1); b++)
                {
                    tileMap[a, b] = sharpenValue(tileMap, a, b);
                    tileMap[a, b] = smoothValue(tileMap, a, b);
                }
            }
        }
        return tileMap;
    }

    void Start()
    {
        if (minDimensions > maxDimensions)
        {
            (minDimensions, maxDimensions) = (maxDimensions, minDimensions);
        }
        mapArray = newMap(minDimensions, maxDimensions);
        mapArray = randomNumbers(mapArray, 100, 2);
        mapArray = smoothMap(mapArray, 10);

        for (int a  = 0; a < mapArray.GetLength(0); a ++)
        {
            for (int b = 0; b < mapArray.GetLength(1); b++)
            {
                GameObject newTile = Instantiate(testTile, new Vector3(a * 0.32f, b * 0.32f, 0), Quaternion.identity);
                
                if (mapArray[a, b] > 10 & mapArray[a, b] < 20)
                {
                    newTile.GetComponent<SpriteRenderer>().color = new Color(0.75f,0.5f,0.45f);
                }
                else if (mapArray[a, b] > 20 & mapArray[a, b] < 100)
                {
                    newTile.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.1f, 0.1f);
                }
                else
                {
                    newTile.GetComponent<SpriteRenderer>().color = new Color(0, 0.6f, 0);
                }

                //newTile.GetComponent<SpriteRenderer>().color = new Color((float)mapArray[a, b] / 100, (float)mapArray[a, b] / 100, (float)mapArray[a, b] / 100);
            }
            
        }
    }
}
