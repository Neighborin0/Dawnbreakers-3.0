using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MapGrid
{
    private int width;
    private int height;
    public float cellSize;
    private int[,] gridArray;
    public Transform parent;
   // private GameObject objectToGenerate;
    public MapGrid(int width, int height, float cellSize, Transform parent)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.parent = parent;
        //this.objectToGenerate = objectToGenerate;
        gridArray = new int[width, height];
        Debug.Log(width + " " + height);
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.black, 1000f);
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.black, 1000f);
            }
               
        }
        Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.black, 1000f);
        Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.black, 1000f);
    }

    public Vector3 GetWorldPos(float x, float y)
    {
        return new Vector3(x,y) * cellSize;
    }
 
}

