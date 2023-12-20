using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

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
    public Vector3 MaxMapBounds;
    public Vector3 MinMapBounds;
    public float ZoomAmount;
    public float defaultZoom;
    public List<MapNode> currentNodes;
    [SerializeField]
    public Grid grid;

    public bool DoneOpening = false;
    public Collider mapCollider;


    public float MaxZoom;
    public float MinZoom;

    public GameObject mapControlBar;
    public bool enableMapControls = true;

    public int completedNodeCount;

    public event Action<MapController> ReEnteredMap;

    private List<Vector3> storedDecorPositions;


    public static MapController Instance { get; private set; }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            if (SceneManager.GetActiveScene().name == "MAP2")
                StartCoroutine(LoadSlots());
            if (OptionsManager.Instance != null)
            {
                if (OptionsManager.Instance.blackScreen.color.a > 0)
                    StartCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.001f, false));
            }
            DontDestroyOnLoad(gameObject);
        }

    }

    void Start()
    {

        if (Director.Instance.DevMode)
        {
            GenerateNodesFromFlow(MapFlow.DevFlow);
        }
        else
        {
            GenerateNodesFromFlow(MapFlow.TutorialFlow);
        }
        SceneManager.sceneLoaded += SaveSceneData;


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
                this.GetComponent<ParticleSystem>().Stop();

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
                OptionsManager.Instance.blackScreen.gameObject.SetActive(true);
                StartCoroutine(LoadSlots());
                StartCoroutine(DoReEnteredMap());
                this.GetComponent<ParticleSystem>().Play();
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
        if (Input.GetKeyDown(KeyCode.M) && SceneManager.GetActiveScene().name == "MAP2")
        {
            if (enableMapControls)
            {
                enableMapControls = false;
                mapControlBar.GetComponent<MoveableObject>().Move(enableMapControls);
                mapControlBar.SetActive(enableMapControls);
            }
            else
            {
                enableMapControls = true;
                mapControlBar.SetActive(enableMapControls);
                mapControlBar.GetComponent<MoveableObject>().Move(enableMapControls);

            }
        }

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

        for (int i = 0; i < UnityEngine.Random.Range(200, 300); i++)
        {
            var decor = Instantiate(mapObjects[UnityEngine.Random.Range(0, mapObjects.Count)], Vector3.zero, Quaternion.identity, grid.transform);
            if (decor.GetComponent<SpriteRenderer>() != null)
            {
                decor.transform.localScale = new Vector3(UnityEngine.Random.Range(180, 240), UnityEngine.Random.Range(280, 320), UnityEngine.Random.Range(180, 240));
            }
            else
            {
                decor.transform.localPosition = new Vector3(UnityEngine.Random.Range(-3f, 3), 0, Tools.RandomExcept(-4f, 4, -1f, 0.5f));
                decor.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
    }
    public void SpawnMiniMe(Vector3 position)
    {
        int i = 0;
        foreach (var unit in Director.Instance.party)
        {
            var MM = Instantiate(miniMePrefab, new Vector3(position.x - (i * 1.3f), position.y + 1f - (i * 0.4f), position.z - 1.5f), Quaternion.identity, grid.transform);
            var rigidbody = MM.GetComponent<Rigidbody2D>();
            rigidbody.simulated = false;


            MM.unit = unit;
            MM.mapIcon.sprite = MM.unit.MiniMapIcons[0];
            MM.transform.localRotation = new Quaternion(175, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w);
            foreach (var sprite in MM.unit.MiniMapIcons)
            {
                MM.mapIcons.Add(sprite);
            }
            MM.mapIcon.material.SetFloat("OutlineThickness", 1f);
            MM.mapIcon.material.SetColor("OutlineColor", Color.black);
            if (i == 0 && LabCamera.Instance != null)
                LabCamera.Instance.MoveAndFollowGameObject(MM.gameObject, new Vector3(0, MinZoom, -MinZoom * 3.4f));

            i++;
        }

    }
    IEnumerator lineCoroutine;
    public void GenerateNodesFromFlow(List<LabNode> mapFlow)
    {
        int i = 0;


        foreach (var node in mapFlow)
        {
            foreach (var prefab in nodePrefabs)
            {
                if (prefab.NodeName.Equals(node.RoomType.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var newNode = Instantiate(prefab, new Vector3(0, 1, 0), Quaternion.identity, mapCanvas.transform);
                    var rectTransform = newNode.GetComponent<RectTransform>();
                    rectTransform.localPosition = grid.CellToWorld(new Vector3Int((i + 1) - 5, 0, -2));
                    newNode.transform.rotation = new Quaternion(0, 0, 0, 0);
                    if (!StartingPositionHasBeenSet)
                    {
                        StartingPosition = grid.CellToWorld(new Vector3Int(i - 5, 0, -2));
                        StartingPositionHasBeenSet = true;
                        newNode.IsStartingNode = true;
                        newNode.IsEnabled = true;
                        storedTransform = newNode.transform.position;
                        SpawnMiniMe(new Vector3(newNode.transform.position.x, newNode.transform.position.y + 0.7f, newNode.transform.position.z));
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
                    if (node.RoomType == MapFlow.RoomType.BOSS)
                    {
                        var bossNode = newNode.GetComponent<BossNode>();
                        foreach (var enemy in node.enemies)
                        {
                            bossNode.enemies.Add(Director.Instance.Unitdatabase.Where(obj => obj.name == enemy).FirstOrDefault());
                        }
                    }
                    if (node.RoomType == MapFlow.RoomType.TUTORIAL)
                    {
                        var tutorialNode = newNode.GetComponent<TutorialEncounterNode>();
                        foreach (var enemy in node.enemies)
                        {
                            tutorialNode.enemies.Add(Director.Instance.Unitdatabase.Where(obj => obj.name == enemy).FirstOrDefault());
                        }
                    }
                    currentNodes.Add(newNode);
                    if (!newNode.IsStartingNode && !Director.Instance.DevMode)
                    {
                        newNode.gameObject.SetActive(false);
                    }
                    if (!Director.Instance.DevMode)
                    {
                        enableMapControls = true;
                    }

                    break;
                }

            }
        }
        if (enableMapControls && SceneManager.GetActiveScene().name == "MAP2")
            mapControlBar.SetActive(true);

        if (!Director.Instance.DevMode && SceneManager.GetActiveScene().name == "MAP2")
            Director.Instance.StartCoroutine(DoLevelDrop());
        else if(SceneManager.GetActiveScene().name == "MAP2")
        {
            var levelDropObj = GameObject.FindObjectOfType<LevelDrop>();
            levelDropObj.gameObject.SetActive(false);
            DoneOpening = true;
        }

        SpawnDecorations();
    }

    private IEnumerator DoLevelDrop()
    {
        Debug.LogWarning("Coronus Should Be Popping Up");
        OptionsManager.Instance.CanPause = false;
        Tools.ToggleUiBlocker(false, true);
        var levelDropObj = GameObject.FindObjectOfType<LevelDrop>();
        levelDropObj.gameObject.SetActive(true);
        levelDropObj.bar.gameObject.SetActive(true);
        yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 0) || !OptionsManager.Instance.blackScreen.gameObject.activeSelf);
        Debug.LogWarning("Level Drop Starting?");
        Director.Instance.StartCoroutine(levelDropObj.DoOpening());
        yield return new WaitUntil(() => levelDropObj.Done);
        levelDropObj.gameObject.SetActive(false);
        Tools.ToggleUiBlocker(true, true);
        OptionsManager.Instance.CanPause = true;
        DoneOpening = true;
    }

    private IEnumerator DrawLine(Vector3 pointToDrawTo, GameObject node)
    {
        var MM = LabCamera.Instance.followTarget;
        node.transform.localScale = new Vector3(0, 0, 0);

        LabCamera.Instance.state = LabCamera.CameraState.IDLE;
        LabCamera.Instance.MoveToGameObject(node);
        float compressor = 2.1f;
        var lineInstance = Instantiate(linePrefab, mapCanvas.transform);
        lineInstance.gameObject.SetActive(true);
        lineInstance.SetPosition(0, new Vector3(storedTransform.x + compressor, storedTransform.y, storedTransform.z));
        lineInstance.SetPosition(1, storedTransform);
        lineCoroutine = Tools.SmoothMoveLine(lineInstance, new Vector3(pointToDrawTo.x - compressor, pointToDrawTo.y, pointToDrawTo.z), 0.01f);
        StartCoroutine(lineCoroutine);
        storedTransform = pointToDrawTo;
        yield return new WaitForSeconds(0.4f);
        currentNodes[completedNodeCount].IsEnabled = true;
        currentNodes[completedNodeCount].gameObject.SetActive(true);
        StartCoroutine(Tools.SmoothScale(node.GetComponent<RectTransform>(), node.GetComponent<MapNode>().oldScaleSize, 0.01f));
        yield return new WaitForSeconds(1.2f);
        currentNodes[completedNodeCount].mapline = lineInstance.gameObject;
        LabCamera.Instance.MoveAndFollowGameObject(MM, new Vector3(0, MinZoom, -MapController.Instance.MinZoom * 3.4f));
        yield return new WaitForSeconds(0.5f);
        foreach (var unit in Director.Instance.party)
        {
            unit.DoEnteredMap();
        }
        OptionsManager.Instance.CanPause = true;
    }
    public IEnumerator LoadSlots()
    {
        yield return new WaitForSeconds(0.001f);
        Tools.ClearAllCharacterSlots();
        Director.Instance.CreateCharacterSlots(Director.Instance.party);
    }

    public IEnumerator DoReEnteredMap(bool setup = true)
    {
        if (setup)
        {
            StartCoroutine(Tools.FadeObject(OptionsManager.Instance.blackScreen, 0.001f, false));
            LabCamera.Instance.followDisplacement = new Vector3(0, MinZoom, -MapController.Instance.MinZoom * 3.4f);
            LabCamera.Instance.cam.fieldOfView = defaultZoom;
            yield return new WaitUntil(() => OptionsManager.Instance.blackScreen.color == new Color(0, 0, 0, 1));
            SpawnMiniMe(currentNodes[completedNodeCount].transform.position);
            yield return new WaitForSeconds(1f);
            foreach (Transform child in transform)
            {
                if (child.GetComponent<MiniMapIcon>() != null)
                {
                    child.GetComponent<MiniMapIcon>().state = MiniMapIcon.MapIconState.IDLE;
                }
            }
        }
        completedNodeCount++;

        if (!DoneOpening)
        {
            Director.Instance.StartCoroutine(DoLevelDrop());
        }
        yield return new WaitUntil(() => DoneOpening);
        Debug.LogWarning("Line Should Be Rendering");
        StartCoroutine(DrawLine(currentNodes[completedNodeCount].transform.position, currentNodes[completedNodeCount].gameObject));
        yield return new WaitForSeconds(1.2f);
        Tools.ToggleUiBlocker(true, true, true);
        Director.Instance.CharacterSlotEnable();
        if (enableMapControls)
        {
            mapControlBar.SetActive(true);
            mapControlBar.GetComponent<MoveableObject>().Move(true);
        }
        ReEnteredMap?.Invoke(this);
    }
}