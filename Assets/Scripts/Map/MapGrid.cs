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
    private float cellSize;
    private int[,] gridArray;
    private Transform parent;
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
               /* var obj = objectToGenerate;
                obj.transform.position = GetWorldPos(x, y) + new Vector3(cellSize, cellSize) * .5f;
                obj.transform.localScale = Vector3.one;
                obj.transform.parent = parent;
               */
            }
        }

        //this.objectToGenerate = objectToGenerate;
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x,y) * cellSize;
    }
 
}

