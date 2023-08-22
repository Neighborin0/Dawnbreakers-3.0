using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.EventSystems;

public enum PlayerState { IDLE, DECIDING, READY, WAITING }
public enum Stat { ATK, DEF, SPD, HP }
public class Unit : MonoBehaviour
{
    [NonSerialized]
    public string unitName;
    [NonSerialized]
    public int currentHP;
    public bool IsPlayerControlled;
    public List<GameObject> skillUIs;
    [NonSerialized]
    public IntentContainer intentUI;
    public Healthbar health;
    [NonSerialized]
    public StaminaBar stamina;
    [NonSerialized]
    public NamePlate namePlate;
    [NonSerialized]
    public bool IsHighlighted;
    [NonSerialized]
    public bool isDarkened;
    [NonSerialized]
    public bool IsDead;
    [NonSerialized]
    public bool Dying = false;
    [NonSerialized]
    public bool StaminaHighlightIsDisabled = false;
    [NonSerialized]
    public EnemyBehavior behavior;
    public List<Sprite> charPortraits;
    public Animator anim;
    public bool Execute = false;
    public List<Sprite> MiniMapIcons;
    public List<EffectIcon> statusEffects;
    public Vector3 offset;
    public Vector3 enemyintentOffset;
    [NonSerialized]
    public TimeLineChild timelinechild;
    [NonSerialized]
    public bool StopMovingToUnit = false;
    [NonSerialized]
    public float StartingStamina;
    IEnumerator generalCoroutine;

    //text stuff
    public List<string> introText;
    public List<Item> inventory;
    public List<LabLine> deathQuotes;

    //action events
    public event Action<Unit> BattlePhaseEnd;
    public event Action<Unit> BattlePhaseClose;
    public event Action<Unit> OnDamaged;
    public event Action<Unit> BattleStarted;
    public event Action<Unit> BattlePostStarted;
    public event Action<Unit> BattleEnded;
    public event Action<Unit> ActionEnded;
    public event Action<Unit> EnteredMap;
    public event Action<Unit> OnPreDeath;

    //player states
    public PlayerState state;

    //stats
    public int maxHP;
    public int attackStat;
    public int defenseStat;
    public int speedStat;
    public Light spotLight;
    public float actionCostMultiplier = 1;

    //actions
    public List<Action> actionList;
    [NonSerialized]
    public GameObject ActionLayout;

    //summon stuff 
    public string[] summonables;
    [NonSerialized]
    public bool IsSummon = false;
   
    void Start()
    {
        currentHP = maxHP;
        state = PlayerState.IDLE;
        spotLight = GetComponentInChildren<Light>();
    }


    void Update()
    {
        var hit = Tools.GetMousePos();
        var sprite = this.gameObject.GetComponent<SpriteRenderer>();
        if (BattleSystem.Instance != null)
        {
            if (!Dying)
            {
                if (IsHighlighted)
                {
                    sprite.material.SetFloat("_OutlineThickness", 1f);
                    sprite.material.SetColor("_OutlineColor", Color.white);
                    sprite.material.SetColor("_CharacterEmission", new Color(0.01f, 0.01f, 0.01f));

                }
                else if (state == PlayerState.DECIDING)
                {

                    sprite.material.SetFloat("_OutlineThickness", 1f);
                    sprite.material.SetColor("_OutlineColor", Color.yellow);
                    sprite.material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f, 1f));

                }
                else if (state == PlayerState.WAITING)
                {

                    sprite.material.SetFloat("_OutlineThickness", 1f);
                    sprite.material.SetColor("_OutlineColor", Color.black);
                    sprite.material.SetColor("_CharacterEmission", new Color(-0.01f, -0.01f, -0.01f, 1f));

                }
                else
                {
                    sprite.material.SetFloat("_OutlineThickness", 1f);
                    sprite.material.SetColor("_OutlineColor", Color.black);
                    sprite.material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f, 1f));
                }
            }


            else
            {
                sprite.material.SetFloat("_OutlineThickness", 1f);
                sprite.material.SetColor("_OutlineColor", Color.black);
                sprite.material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f, 1f));
            }


            if (stamina != null)
            {
                if (IsPlayerControlled && stamina.slider.value == stamina.slider.maxValue && state == PlayerState.WAITING)
                {
                    state = PlayerState.IDLE;
                    StartCoroutine(StartDelayedDecision());
                }
                if (state == PlayerState.IDLE && IsPlayerControlled)
                {
                    foreach (var unit in Tools.GetAllUnits())
                    {
                        unit.stamina.Paused = true;
                    }
                }
            }
            if (hit.collider == this.GetComponent<BoxCollider>() && !isDarkened && !OverUI())
            {
                if (BattleSystem.Instance.state == BattleStates.DECISION_PHASE && Tools.CheckIfAnyUnitIsDeciding())
                {
                    if (!this.IsPlayerControlled)
                    {
                        BattleLog.Instance.DisplayCharacterStats(this, true);
                    }
                }
                if (BattleSystem.Instance.state == BattleStates.IDLE)
                {
                    BattleLog.Instance.DisplayCharacterStats(this, true);
                }
                if (BattleSystem.Instance.CheckPlayableState())
                {
                    sprite.material.SetColor("_CharacterEmission", new Color(0.01f, 0.01f, 0.01f));
                    this.timelinechild.Shift(this);
                }

            }
            else if (timelinechild != null && !timelinechild.HighlightedIsBeingOverwritten)
            {

                timelinechild.Return();

            }

            if (isDarkened && BattleSystem.Instance.state == BattleStates.DECISION_PHASE)
            {
                sprite.material.SetColor("_CharacterEmission", new Color(-0.01f, -0.01f, -0.01f));
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (hit.collider != null && hit.collider == this.GetComponent<BoxCollider>() && BattleSystem.Instance.state == BattleStates.DECISION_PHASE && !OverUI() && state != PlayerState.DECIDING)
                {
                    if (!Tools.CheckIfAnyUnitIsTargetting())
                    {
                        BattleLog.Instance.itemText.text = "";
                        BattleLog.Instance.DisplayCharacterStats(this, true);
                        //LabCamera.Instance.MoveToUnit(this);
                        if (this.IsPlayerControlled)
                        {
                            foreach (var x in Tools.GetAllUnits())
                            {
                                BattleSystem.SetUIOff(x);
                            }
                        }
                    }

                }
            }
            if (state == PlayerState.IDLE && BattleSystem.Instance.state == BattleStates.DECISION_PHASE && !IsHighlighted && !OverUI() || state == PlayerState.READY && BattleSystem.Instance.state == BattleStates.DECISION_PHASE && !IsHighlighted && !OverUI())
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (hit.collider != null && hit.collider == this.GetComponent<BoxCollider>() && this.IsPlayerControlled)
                    {
                        StopMovingToUnit = false;
                        BattleLog.Instance.DisplayCharacterStats(this, true);
                        StartDecision();

                    }

                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (state == PlayerState.DECIDING && BattleSystem.Instance.state == BattleStates.DECISION_PHASE)
                {
                    ExitDecision();
                }

            }
        }
       



    }

    public void PlayAction(string AnimationName, Unit unit)
    {
        var stateId = Animator.StringToHash(AnimationName);
        if (unit.anim != null)
        {
            if (unit.anim.HasState(0, stateId))
            {
                UnityEngine.Debug.Log(AnimationName);
                unit.anim.Play(AnimationName);
            }
            else
            {
                unit.anim.Play("Idle");
                print("Doing alternate execution");
                StartCoroutine(ExecuteAnimator());
            }
        }
        else
        {
            print("Doing alternate execution");
            StartCoroutine(ExecuteAnimator());
        }

    }
    public virtual void Intro()
    {

    }


    public void Queue(Action action)
    {
        var newAction = UnityEngine.Object.Instantiate(action);
        this.StopCoroutine(QueueAction(newAction));
        BattleSystem.SetUIOff(action.unit);
        this.StartCoroutine(QueueAction(newAction));
    }
    private IEnumerator QueueAction(Action action)
    {
        var battlesystem = BattleSystem.Instance;
        yield return new WaitUntil(() => action.unit.stamina.slider.value == action.unit.stamina.slider.maxValue && battlesystem.state != BattleStates.BATTLE);
        if (action.unit != null && action.targets != null)
        {
            if (action.unit.state == PlayerState.DECIDING)
            {
                yield break;
            }

            action.unit.stamina.DoCost(action.cost);
            battlesystem.AddAction(action);
            BattleSystem.SetUIOff(action.unit);
        }
        yield break;
    }

    public void StartDecision()
    {
        BattleSystem.SetUIOn(this);
        LabCamera.Instance.MoveToUnit(this);
        BattleSystem.Instance.state = BattleStates.DECISION_PHASE;
        print("Unit is deciding an action");
    }

    public IEnumerator StartDelayedDecision()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => BattleSystem.Instance.ActionsToPerform.Count == 0 && BattleSystem.Instance.state == BattleStates.IDLE || BattleSystem.Instance.ActionsToPerform.Count == 0 && BattleSystem.Instance.state == BattleStates.DECISION_PHASE);
        BattleLog.Instance.GetComponent<MoveableObject>().Move(true);
        StartDecision();
    }


    public void ExitDecision()
    {
        if (state != PlayerState.WAITING)
            state = PlayerState.IDLE;

        BattleSystem.SetUIOff(this);
        foreach (TimeLineChild child in Director.Instance.timeline.children)
        {
            if (child.CanClear)
            {
                Director.Instance.timeline.children.Remove(child);
                Destroy(child.gameObject);
                break;
            }
        }
        this.IsHighlighted = false;
    }

    public IEnumerator ExecuteAnimator()
    {
        Execute = true;
        yield return new WaitForSeconds(0.00001f);
        Execute = false;
    }

    public void DoDeathQuote()
    {
        BattleLog.Instance.ClearAllBattleLogText();
        if(BattleSystem.Instance.playerUnits.Count <= 1)
        {
            BattleLog.Instance.CharacterDialog(deathQuotes, true, true);
        }
        else
        {
            BattleLog.Instance.CharacterDialog(deathQuotes, true, false);
        }
        BattleSystem.Instance.BattlePhasePause = true;
    }


    public void ChangeUnitsLight(Light light, float desiredIntensity, float amountToRaiseBy, float delay = 0, float stagnantDelay = 0)
    {
        generalCoroutine = ChangeUnitsLightCoroutine(light, desiredIntensity, amountToRaiseBy, delay, stagnantDelay);
        StartCoroutine(generalCoroutine);
    }
    private IEnumerator ChangeUnitsLightCoroutine(Light light, float desiredIntensity, float amountToRaiseBy, float delay = 0, float stagnantDelay = 0)
    {
        while (light.intensity < desiredIntensity)
        {
            light.intensity += amountToRaiseBy;
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(stagnantDelay);
        while (light.intensity > 0)
        {
            light.intensity -= amountToRaiseBy;
            yield return new WaitForSeconds(delay);
        }

    }

    public void ReturnTimelineChild()
    {
        if (BattleSystem.Instance != null && timelinechild != null)
        {
            timelinechild.Return();
            print("TIMELINE CHILDREN SHOULD BE RETURNING");
        }

    }
    private bool OverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void DoBattlePhaseEnd()
    {
        BattlePhaseEnd?.Invoke(this);
    }

    public void DoOnDamaged()
    {
        OnDamaged?.Invoke(this);
    }

    public void DoBattleStarted()
    {
        BattleStarted?.Invoke(this);
    }

    public void DoBattleEnded()
    {
        BattleEnded?.Invoke(this);
    }

    public void DoActionEnded()
    {
        ActionEnded?.Invoke(this);
    }

    public void DoOnPreDeath()
    {
        OnPreDeath?.Invoke(this);
    }

    public void DoBattlePhaseClose()
    {
        BattlePhaseClose?.Invoke(this);
    }

    public void DoPostBattleStarted()
    {
        BattlePostStarted?.Invoke(this);
    }

    public void DoEnteredMap()
    {
        EnteredMap?.Invoke(this);
    }




}
