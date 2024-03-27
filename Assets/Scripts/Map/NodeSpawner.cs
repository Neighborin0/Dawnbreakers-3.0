using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static MapFlow;
using JetBrains.Annotations;

public class NodeSpawner : MonoBehaviour
{
    public bool isSpawnPointNode = false;
    public List<NodeSpawner> adjacentNodes;
    public Vector3Int cellPos;
    public bool unlocked = false;
    public bool occupied = false;

    public bool DrawingMapLines = false;

    public void Start()
    {
       cellPos = ReturnWorldPositionAsCell(NodeController.Instance.mapGrid);
    }
    public void CheckForAdjacentNodes()
    {
        var nodeController = NodeController.Instance;

        foreach (var otherNode in nodeController.currentRooms.ToList())
        {
            if (this != null && otherNode != null && !adjacentNodes.Contains(otherNode))
            {
                int deltaX = cellPos.x - otherNode.cellPos.x;
                int deltaY = cellPos.y - otherNode.cellPos.y;

                // CellPos is adjacent horizontally
                if (Mathf.Abs(deltaX) == 1 && deltaY == 0)
                {
                    adjacentNodes.Add(otherNode);
                }
                // CellPos is adjacent vertically
                else if (Mathf.Abs(deltaY) == 1 && deltaX == 0)
                {
                    adjacentNodes.Add(otherNode);
                }
            }
        }
    }

    IEnumerator lineCoroutine;

    public void DrawLinesToAdjacentNodes()
    {
        DrawingMapLines = true;
        float compressor = -2;

        foreach (var otherNode in adjacentNodes)
        {
            //if (!otherNode.unlocked)
           // {
                var lineInstance = Instantiate(NodeController.Instance.linePrefab, NodeController.Instance.parentCanvas.transform);
                lineInstance.gameObject.SetActive(true);
               // lineInstance.SetWidth(10, 10);

                int deltaX = cellPos.x - otherNode.cellPos.x;
                int deltaY = cellPos.y - otherNode.cellPos.y;

                Vector3 pointToDrawTo = otherNode.transform.position;

                // CellPos is adjacent horizontally
                if (Mathf.Abs(deltaX) == 1 && deltaY == 0)
                {
                    lineInstance.SetPosition(0, new Vector3(this.transform.position.x + compressor, this.transform.position.y, this.transform.position.z));
                    lineInstance.SetPosition(1, otherNode.transform.position);
                    lineCoroutine = Tools.SmoothMoveLine(lineInstance, new Vector3(pointToDrawTo.x - compressor, pointToDrawTo.y, pointToDrawTo.z), 0.01f);
                }
                // CellPos is adjacent vertically
                else if (Mathf.Abs(deltaY) == 1 && deltaX == 0)
                {
                    lineInstance.SetPosition(0, new Vector3(this.transform.position.x, this.transform.position.y + compressor, this.transform.position.z));
                    lineInstance.SetPosition(1, otherNode.transform.position);
                    lineCoroutine = Tools.SmoothMoveLine(lineInstance, new Vector3(pointToDrawTo.x, pointToDrawTo.y - compressor, pointToDrawTo.z), 0.01f);
                }
                otherNode.unlocked = true;
                StartCoroutine(lineCoroutine);
           // }
        }
        DrawingMapLines = false;
    }


    public Vector3Int ReturnWorldPositionAsCell(Grid mapGrid)
    {
        Vector3Int positionToReturn = mapGrid.WorldToCell(this.transform.position);
        return positionToReturn;
    }

   
}
