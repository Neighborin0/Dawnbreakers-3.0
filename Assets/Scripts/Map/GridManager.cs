using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
  public int width, height;
    public LabTile tilePrefab;


    void Start()
    {
       GenerateGrid();
    }
    void GenerateGrid()
    {
        /*for (int i = 0; i < GetComponent<MeshFilter>().sharedMesh.vertexCount; i++) 
        {
            Instantiate(tilePrefab, GetComponent<MeshFilter>().sharedMesh.vertices[i], this.transform.rotation);
        }
        */
       

    }
 
}

