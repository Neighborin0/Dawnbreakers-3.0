using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class MapController : MonoBehaviour
{
    public List<MapNode> nodePrefabs;
    public MiniMapIcon miniMePrefab;
    public Canvas mapCanvas;
    private bool StartingPositionHasBeenSet = false;
    public List<GameObject> mapObjects;
    public Vector3 StartingPosition;
    public LineRenderer linePrefab;
    [SerializeField]
    public bool Loaded = true;
    [SerializeField]
    private Vector3 storedTransform = Vector3.zero;
    [SerializeField]
    private float delay;
    public List<MapNode> currentNodes;

    public int completedNodeCount;

    public static MapController Instance { get; private set; }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            StartCoroutine(LoadSlots());
            DontDestroyOnLoad(gameObject);
        }

    }

    void Start()
    {
        GenerateNodesFromFlow(MapFlow.TestFlow);
        SpawnMiniMe();
        SceneManager.sceneLoaded += SaveSceneData;
        SpawnDecorations();
    }

    private void SaveSceneData(Scene scene, LoadSceneMode mode)
    {
        if (this != null)
        {
            if (Loaded)
            {
                foreach (var minimapIcon in GameObject.FindObjectsOfType<MiniMapIcon>())
                {
                    Destroy(minimapIcon.gameObject);
                }
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
                Loaded = false;

            }
            else
            {
                Director.Instance.characterTab.transform.transform.SetAsLastSibling();
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                    //StartingPosition = child.position;
                    if (child.GetComponent<MapNode>() != null)
                    {
                        var MN = child.GetComponent<MapNode>();
                        if (MN.IsEnabled)
                        {
                            child.gameObject.SetActive(true);
                        }
                    }
                }
                Loaded = true;
                Director.Instance.blackScreen.gameObject.SetActive(true);
                StartCoroutine(LoadSlots());
                StartCoroutine(DoReEnteredMap());
            }
        }
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            float force = 800f;
            StartCoroutine(DoPlayerJump(force));
        }
        */
    }
    public IEnumerator DoPlayerJump(float force)
    {
        foreach (var unit in GameObject.FindObjectsOfType<MiniMapIcon>())
        {
            yield return new WaitUntil(() => unit.state == MiniMapIcon.MapIconState.IDLE);
            unit.GetComponent<Rigidbody2D>().simulated = true;
            unit.state = MiniMapIcon.MapIconState.JUMPING;
            StartCoroutine(Tools.ApplyLaunchToGameObject(unit.gameObject, force, delay));
            yield return new WaitUntil(() => unit.GetComponent<Rigidbody2D>().velocity == Vector2.zero);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => unit.GetComponent<Rigidbody2D>().velocity == Vector2.zero);
            unit.state = MiniMapIcon.MapIconState.IDLE;
            unit.GetComponent<Rigidbody2D>().simulated = false;
        }
    }

    public void SpawnDecorations()
    {
        for (int i = 0; i < UnityEngine.Random.Range(20, 30); i++)
        {
            var decor = Instantiate(mapObjects[UnityEngine.Random.Range(0, mapObjects.Count)], new Vector3(UnityEngine.Random.Range(-160f, 1920), UnityEngine.Random.Range(-510, 510), 0), Quaternion.Euler(0, 0, 0), mapCanvas.transform);
            decor.transform.localScale = new Vector3(200, 300, 200);
            decor.transform.localPosition = new Vector3(decor.transform.position.x, decor.transform.position.y, 0);
        }
    }
    public void SpawnMiniMe()
    {
        int i = 0;
        foreach (var unit in Director.Instance.party)
        {
            var MM = Instantiate(miniMePrefab, StartingPosition + new Vector3(i * 2, 2, -1 + i), Quaternion.identity, mapCanvas.transform);
            var rigidbody = MM.GetComponent<Rigidbody2D>();
            rigidbody.simulated = false;
            MM.unit = unit;
            MM.mapIcon.sprite = MM.unit.MiniMapIcons[0];
            foreach (var sprite in MM.unit.MiniMapIcons)
            {
                MM.mapIcons.Add(sprite);
            }
            MM.mapIcon.material.SetFloat("OutlineThickness", 1f);
            MM.mapIcon.material.SetColor("OutlineColor", Color.black);
            //LabCamera.Instance.MoveAndFollowGameObject(MM.gameObject, new Vector3(0, 0, -120));
            i++;
        }

    }
    IEnumerator lineCoroutine;
    public void GenerateNodesFromFlow(List<LabNode> mapFlow)
    {
        int i = 0;

        var mapGrid = new MapGrid(20, 1, 170, this.transform);
        foreach (var node in mapFlow)
        {
            foreach (var prefab in nodePrefabs)
            {
                if (prefab.NodeName.Equals(node.RoomType.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var newNode = Instantiate(prefab, new Vector3(0, 0, -1), Quaternion.identity, mapCanvas.transform);
                    var rectTransform = newNode.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = mapGrid.GetWorldPos(i + 1, 0);
                    rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, -1);
                    newNode.transform.rotation = new Quaternion(0, 0, 0, 0);
                    if (!StartingPositionHasBeenSet)
                    {
                        StartingPosition = newNode.transform.position;
                        StartingPositionHasBeenSet = true;
                        newNode.IsStartingNode = true;
                        newNode.IsEnabled = true;
                        storedTransform = newNode.transform.position;
                    }
                    i++;
                    if (node.RoomType == MapFlow.RoomType.COMBAT)
                    {
                        var combatNode = newNode.GetComponent<CombatNode>();
                        foreach (var enemy in node.enemies)
                        {
                            combatNode.enemies.Add(Director.Instance.Unitdatabase.Where(obj => obj.name == enemy).FirstOrDefault());
                        }
                    }
                    currentNodes.Add(newNode);
                    if (!newNode.IsStartingNode && !Director.Instance.DevMode)
                    {
                        newNode.gameObject.SetActive(false);
                    }
                    break;
                }

            }
        }
    }

    private void DrawLine(Vector3 pointToDrawTo)
    {
        float compressor = 2;
        var lineInstance = Instantiate(linePrefab, mapCanvas.transform);
        lineInstance.SetPosition(0, new Vector3(storedTransform.x + compressor, storedTransform.y, storedTransform.z));
        lineInstance.SetPosition(1, storedTransform);
        lineCoroutine = Tools.SmoothMoveLine(lineInstance, new Vector3(pointToDrawTo.x - compressor, pointToDrawTo.y, pointToDrawTo.z), 0.01f);
        StartCoroutine(lineCoroutine);

        storedTransform = pointToDrawTo;
    }
    public IEnumerator LoadSlots()
    {
        yield return new WaitForSeconds(0.001f);
        Tools.ClearAllCharacterSlots();
        Director.Instance.CreateCharacterSlots(Director.Instance.party);
    }

    public IEnumerator DoReEnteredMap()
    {
        StartCoroutine(Tools.FadeObject(Director.Instance.blackScreen, 0.001f, false));
        yield return new WaitUntil(() => Director.Instance.blackScreen.color == new Color(0, 0, 0, 1));
        SpawnMiniMe();
        yield return new WaitForSeconds(1f);
        foreach (Transform child in transform)
        {
            if (child.GetComponent<MiniMapIcon>() != null)
            {
                child.GetComponent<MiniMapIcon>().state = MiniMapIcon.MapIconState.IDLE;
            }
        }
        foreach (var unit in Director.Instance.party)
        {
            unit.DoEnteredMap();
        }
        /// float force = 800f;
        // StartCoroutine(DoPlayerJump(force));
        completedNodeCount++;
        currentNodes[completedNodeCount].IsEnabled = true;
        currentNodes[completedNodeCount].gameObject.SetActive(true);
        DrawLine(currentNodes[completedNodeCount].transform.position);
    }
}