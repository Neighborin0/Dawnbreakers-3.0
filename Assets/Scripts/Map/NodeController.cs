using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static MapFlow;
using JetBrains.Annotations;
using static Autodesk.Fbx.FbxTime;

public class NodeController : MonoBehaviour
{
    public List<NodeSpawner> currentRooms;
    //public int maxAmountOfRooms;


    [SerializeField]
    public Grid mapGrid;
    public Vector3 targetScale = Vector3.one;

    public List<MapNode> NodePrefabs;
    public Canvas parentCanvas;

    [SerializeField]
    private Vector3Int spawnPoint;

    [SerializeField]
    private bool CreatedSpawnPoint = false;

    public Vector3Int LastCellSpawnPoint;

    public NodeSpawner spawnPointPrefab;

    public LineRenderer linePrefab;

    public bool DoneGenerating = false;
    public static NodeController Instance { get; private set; }

    public Floor currentFloor = Floor.CORONUS;

    public MapTemplate currentMapTemplate;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        generateCoroutine = GenerateNodes();
        StartCoroutine(generateCoroutine);
    }

    public void Update()
    {
        if (DoneGenerating)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (generateCoroutine != null)
                {
                    StopCoroutine(generateCoroutine);
                }
                for (int i = 0; i < currentRooms.Count; i++)
                {
                    Destroy(currentRooms[i].gameObject);
                }
                foreach (var line in GameObject.FindObjectsOfType<LineRenderer>().ToList())
                {
                    Destroy(line.gameObject);
                }
                foreach (var MapNode in GameObject.FindObjectsOfType<MapNode>().ToList())
                {
                    Destroy(MapNode.gameObject);
                }
                CreatedSpawnPoint = false;
                spawnPoint = Vector3Int.zero;
                generateCoroutine = GenerateNodes();
                StartCoroutine(generateCoroutine);
                DoneGenerating = false;
            }
        }
    }

    public IEnumerator generateCoroutine;
    public IEnumerator GenerateNodes()
    {
        //Creates MapTemplate To Use

        List<MapTemplate> viableMapTemplates = new List<MapTemplate>();
        foreach (var template in Director.Instance.mapTemplates)
        {
            if (template.floorToSpawnOn == currentFloor)
            {
                viableMapTemplates.Add(template);
            }
        }
        if(viableMapTemplates == null)
        {
            Debug.LogError($"Insert a viable map template for the floor:{currentFloor}");
        }
        currentMapTemplate = viableMapTemplates[Random.Range(0, viableMapTemplates.Count)];
        print($"Using Template:{currentMapTemplate.name}");
        


        int maxGridSizeX = 7;
        int maxGridSizeY = 15;

        int minGridSizeX = -7;
        int minGridSizeY = -15;


        int spawnPointRange = 0;

        //sets up spawn point at a random position in the world
        var spawnPos = mapGrid.CellToWorld(new Vector3Int(Random.Range(-spawnPointRange, spawnPointRange) * 2, Random.Range(-spawnPointRange, spawnPointRange)));
        var spawnNode = Instantiate(spawnPointPrefab, spawnPos, Quaternion.identity, parentCanvas.transform);
        spawnNode.transform.localScale = NodeController.Instance.targetScale;
        spawnNode.isSpawnPointNode = true;
        spawnNode.unlocked = true;
        spawnNode.occupied = true;
        LastCellSpawnPoint = mapGrid.WorldToCell(spawnPos);
        spawnPoint = mapGrid.WorldToCell(spawnPos);
        CreatedSpawnPoint = true;
        currentRooms.Add(spawnNode);


        yield return new WaitUntil(() => CreatedSpawnPoint);

        yield return new WaitForSeconds(0.5f);
        //new spawnPoints are generated
        for (int i = 0; i < currentMapTemplate.roomsToSpawn.Count; i++)
        {
            //moves potential spawnpoint in a certain cardinal direction
            int RandomX = Random.Range(-1, 2);
            int RandomY = Random.Range(-1, 2);

            //prevents diagonal nodes
            if (RandomX != 0 && RandomY != 0)
            {
                float randomValue = Random.value;

                if (randomValue < 0.5f)
                {
                    RandomX = 0;
                }
                else
                {
                    RandomY = 0;
                }
            }


            //if the new spawnpoint position doesn't move or moves beyond the grid size, the function is cancelled is incremented back
            if (RandomX == 0 && RandomY == 0 &&
                (LastCellSpawnPoint.x + RandomX >= maxGridSizeX || LastCellSpawnPoint.y + RandomY >= maxGridSizeY ||
                 LastCellSpawnPoint.x + RandomX <= minGridSizeX || LastCellSpawnPoint.y + RandomY <= minGridSizeY))
            {
                i--;
            }
            else
            {

                //actual spawnpoint is generated
                var gridPos = mapGrid.CellToWorld(new Vector3Int(LastCellSpawnPoint.x + RandomX, LastCellSpawnPoint.y + RandomY));
                LastCellSpawnPoint = mapGrid.WorldToCell(gridPos);
                var newNode = Instantiate(spawnPointPrefab, gridPos, Quaternion.identity, parentCanvas.transform);
                newNode.transform.localScale = NodeController.Instance.targetScale;
                foreach (var otherNode in currentRooms.ToList())
                {
                    //checks to see if potential spawnpoint is on top of another node. If it is, then that node is destroyed.
                    if (newNode != null && otherNode != null && mapGrid.WorldToCell(otherNode.transform.position) == mapGrid.WorldToCell(newNode.transform.position))
                    {
                        i--;
                        currentRooms.Remove(newNode);
                        Destroy(newNode.gameObject);
                        break;
                    }
                }

                if (newNode.gameObject != null)
                {
                    currentRooms.Add(newNode);
                }
                ClearNullNodes();
                yield return new WaitForSeconds(0.1f);
            }
        }

        foreach (var node in currentRooms)
        {
            node.CheckForAdjacentNodes();
        }
        RenameRooms();
        DoneGenerating = true;
        DrawAllNodeLines();
        PopulateNodes();
        //spawned = true;
    }

    private void ClearNullNodes()
    {
        currentRooms.RemoveAll(item => item == null);
    }

    private void RenameRooms()
    {
        for (int i = 0; i < currentRooms.Count; i++)
        {
            currentRooms[i].gameObject.name = i.ToString();
        }
    }

    private void DrawAllNodeLines()
    {
        for (int i = 0; i < currentRooms.Count; i++)
        {
            currentRooms[i].DrawLinesToAdjacentNodes();
        }
    }

    private void PopulateNodes()
    {
        //ensures boss node is always the first node to spawn
        var index = currentMapTemplate.roomsToSpawn.FindIndex(room => room == RoomType.BOSS);
        var item = currentMapTemplate.roomsToSpawn[index];
        currentMapTemplate.roomsToSpawn[index] = currentMapTemplate.roomsToSpawn[0];
        currentMapTemplate.roomsToSpawn[0] = item;

        foreach (var roomType in currentMapTemplate.roomsToSpawn)
        {
            switch (roomType)
            {
                case RoomType.COMBAT:
                    {
                        SpawnCombatNode();
                    }
                    break;

                case RoomType.CHEST:
                    {
                        SpawnChestNode();
                    }
                    break;

                case RoomType.BOSS:
                    {
                        SpawnBossNode();
                    }
                    break;

                case RoomType.EVENT:
                    {
                        SpawnEventNode();
                    }
                    break;

                case RoomType.TUTORIAL:
                    {
                        SpawnTutorialNode();
                    }
                    break;
                case RoomType.SHOP:
                    {
                        SpawnShopNode();
                    }
                    break;
                case RoomType.ELITE:
                    {
                        SpawnEliteNode();
                    }
                    break;

            }
        }
     
    }

    private void SpawnBossNode()
    {
        NodeSpawner farthestNode = null;
        float farthestNodeDistance = 0;
        foreach (var otherNode in currentRooms)
        {
            float otherNodeDistance = Mathf.Abs(otherNode.transform.position.x) + Mathf.Abs(otherNode.transform.position.y);

            if (farthestNode != null)
            {
                farthestNodeDistance = Mathf.Abs(farthestNode.transform.position.x) + Mathf.Abs(farthestNode.transform.position.y);
            }

            if (otherNodeDistance > farthestNodeDistance)
            {
                farthestNode = otherNode;
            }

        }

        var bossNode = Instantiate(NodePrefabs.FirstOrDefault(node => node.nodeType == RoomType.BOSS), farthestNode.transform.position, Quaternion.identity, farthestNode.transform);
        farthestNode.occupied = true;
        farthestNode.GetComponent<Image>().enabled = false;
    }

    private void SpawnCombatNode()
    {
        NodeSpawner unoccupiedNode = null;
        foreach (var otherNode in currentRooms)
        {
            if(!otherNode.occupied) 
            { 
                unoccupiedNode = otherNode;
                break;
            }
        }

        var combatNode = Instantiate(NodePrefabs.FirstOrDefault(node => node.nodeType == RoomType.COMBAT).gameObject, unoccupiedNode.transform.position, Quaternion.identity, unoccupiedNode.transform);
        unoccupiedNode.occupied = true;
        unoccupiedNode.GetComponent<Image>().enabled = false;
    }

    private void SpawnChestNode()
    {
        NodeSpawner unoccupiedNode = null;
        foreach (var otherNode in currentRooms)
        {
            if (!otherNode.occupied)
            {
                unoccupiedNode = otherNode;
                break;
            }
        }

        var Node = Instantiate(NodePrefabs.FirstOrDefault(node => node.nodeType == RoomType.CHEST).gameObject, unoccupiedNode.transform.position, Quaternion.identity, unoccupiedNode.transform);
        unoccupiedNode.occupied = true;
        unoccupiedNode.GetComponent<Image>().enabled = false;
    }

    private void SpawnEventNode()
    {
        NodeSpawner unoccupiedNode = null;
        foreach (var otherNode in currentRooms)
        {
            if (!otherNode.occupied)
            {
                unoccupiedNode = otherNode;
                break;
            }
        }

        var Node = Instantiate(NodePrefabs.FirstOrDefault(node => node.nodeType == RoomType.EVENT).gameObject, unoccupiedNode.transform.position, Quaternion.identity, unoccupiedNode.transform);
        unoccupiedNode.occupied = true;
        unoccupiedNode.GetComponent<Image>().enabled = false;
    }

    private void SpawnTutorialNode()
    {
        NodeSpawner unoccupiedNode = null;
        foreach (var otherNode in currentRooms)
        {
            if (!otherNode.occupied)
            {
                unoccupiedNode = otherNode;
                break;
            }
        }

        var Node = Instantiate(NodePrefabs.FirstOrDefault(node => node.nodeType == RoomType.TUTORIAL).gameObject, unoccupiedNode.transform.position, Quaternion.identity, unoccupiedNode.transform);
        unoccupiedNode.occupied = true;
        unoccupiedNode.GetComponent<Image>().enabled = false;
    }

    private void SpawnShopNode()
    {
        NodeSpawner unoccupiedNode = null;
        foreach (var otherNode in currentRooms)
        {
            if (!otherNode.occupied)
            {
                unoccupiedNode = otherNode;
                break;
            }
        }

        var Node = Instantiate(NodePrefabs.FirstOrDefault(node => node.nodeType == RoomType.SHOP).gameObject, unoccupiedNode.transform.position, Quaternion.identity, unoccupiedNode.transform);
        unoccupiedNode.occupied = true;
        unoccupiedNode.GetComponent<Image>().enabled = false;
    }

    private void SpawnEliteNode()
    {
        NodeSpawner unoccupiedNode = null;
        foreach (var otherNode in currentRooms)
        {
            if (!otherNode.occupied)
            {
                unoccupiedNode = otherNode;
                break;
            }
        }

        var Node = Instantiate(NodePrefabs.FirstOrDefault(node => node.nodeType == RoomType.ELITE).gameObject, unoccupiedNode.transform.position, Quaternion.identity, unoccupiedNode.transform);
        unoccupiedNode.occupied = true;
        unoccupiedNode.GetComponent<Image>().enabled = false;
    }

}
