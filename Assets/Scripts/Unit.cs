using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

using static System.Collections.Specialized.BitVector32;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.Shapes;
using System.Data.Common;

public enum PlayerState { IDLE, DECIDING, READY, WAITING }
public enum Stat { ATK, DEF, ARMOR, HP }
public class Unit : MonoBehaviour
{
    [NonSerialized]
    public string unitName;

    public int currentHP;
    public bool IsPlayerControlled;
    public List<GameObject> skillUIs;
    [NonSerialized]
    public IntentContainer intentUI;
    public Healthbar health;
    [NonSerialized]
    public NamePlate namePlate;
    [NonSerialized]
    public bool IsHighlighted;
    [NonSerialized]
    public bool isDarkened;
    [NonSerialized]
    public bool IsDead;
    [SerializeField]
    public bool Dying = false;
    [NonSerialized]
    public bool StaminaHighlightIsDisabled = false;
    [NonSerialized]
    public EnemyBehavior behavior;
    public List<UnityEngine.Sprite> charPortraits;
    public Animator anim;
    public bool Execute = false;
    public List<UnityEngine.Sprite> MiniMapIcons;
    public List<EffectIcon> statusEffects;
    public Vector3 offset;
    public Vector3 enemyintentOffset;
    [NonSerialized]
    public TimeLineChild timelinechild;
    [NonSerialized]
    public bool StopMovingToUnit = false;
    IEnumerator lightCoroutine;
    IEnumerator fadeCoroutine;
    public bool IsHidden;

    //text stuff
    public List<DialogueHandler> introText;
    public List<Item> inventory;
    public List<DialogueHandler> levelUpScreenQuotes;
    public List<DialogueHandler> deathQuotes;

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
    public event Action<Unit> OnPlayerUnitDeath;

    //player states
    public PlayerState state;

    //stats
    public int maxHP;
    public int attackStat;
    public int defenseStat;
    public int armor = 0;


    public Light spotLight;
    public float actionCostMultiplier = 1;
    public DamageType[] resistances;
    public DamageType[] weaknesses;

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
                    sprite.material.SetColor("_OutlineColor", Color.white * 2f);
                    sprite.material.SetColor("_CharacterEmission", new Color(0.01f, 0.01f, 0.01f));

                }
                else if (IsHidden)
                {
                    sprite.material.SetFloat("_OutlineThickness", 1f);
                    sprite.material.SetColor("_OutlineColor", Color.black);
                    sprite.material.SetColor("_CharacterEmission", new Color(-0.3f, -0.3f, -0.3f, 1f));
                }
                else if (state == PlayerState.DECIDING && IsPlayerControlled)
                {

                    sprite.material.SetFloat("_OutlineThickness", 1f);
                    sprite.material.SetColor("_OutlineColor", Color.yellow * 2f);
                    sprite.material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f, 1f));

                }
                else if (state == PlayerState.READY && BattleSystem.Instance.state == BattleStates.DECISION_PHASE && IsPlayerControlled)
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

           /* if (stamina != null)
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
           */
           //Hit Detection
            if (hit.collider == this.GetComponent<BoxCollider>() && !isDarkened && !OverUI())
            {
                if (BattleSystem.Instance.state == BattleStates.DECISION_PHASE && CombatTools.CheckIfAnyUnitIsDeciding())
                {
                    if (!this.IsPlayerControlled)
                    {
                        BattleLog.Instance.DisplayCharacterStats(this);
                    }
                }
                if (BattleSystem.Instance.state == BattleStates.IDLE)
                {
                    BattleLog.Instance.DisplayCharacterStats(this);
                }
                if (BattleSystem.Instance.CheckPlayableState())
                {
                    sprite.material.SetColor("_CharacterEmission", new Color(0.01f, 0.01f, 0.01f));
                    if(timelinechild != null)
                        this.timelinechild.Shift(this);
                }

            }
            //Makes sure Timeline Child Automatically Returns When Not Selected
            else if (timelinechild != null && !timelinechild.HighlightedIsBeingOverwritten)
            {
                timelinechild.Return();
            }
            //Decision Phase and Darkened
            if (isDarkened && BattleSystem.Instance.state == BattleStates.DECISION_PHASE)
            {
                sprite.material.SetColor("_CharacterEmission", new Color(-0.01f, -0.01f, -0.01f));
            }
            //Decision Start
            if (Input.GetMouseButtonUp(0))
            {
                if (hit.collider != null && hit.collider == this.GetComponent<BoxCollider>() && BattleSystem.Instance.state == BattleStates.DECISION_PHASE && !OverUI() && state != PlayerState.DECIDING)
                {
                    if (!CombatTools.CheckIfAnyUnitIsTargetting())
                    {
                        BattleLog.Instance.itemText.text = "";
                        BattleLog.Instance.DisplayCharacterStats(this);
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
                        BattleLog.Instance.DisplayCharacterStats(this);
                        if(state == PlayerState.READY)
                        {
                            foreach (var skill in skillUIs)
                            {
                                var actionContainer = skill.GetComponent<ActionContainer>();
                                if (actionContainer.action.actionStyle != Action.ActionStyle.STANDARD)
                                {
                                    CombatTools.ReturnPipCounter().AddPip();
                                    actionContainer.action.actionStyle = Action.ActionStyle.STANDARD;
                                }
                            }
                            Director.Instance.timeline.RemoveTimelineChild(this);
                        }
                        StartDecision();

                    }

                }
            }

        }
       
    }

    public void PlayUnitAction(string AnimationName, Unit unit)
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
    public virtual void Intro() { }


    public void Queue(Action action)
    {
        var newAction = UnityEngine.Object.Instantiate(action);
        BattleSystem.SetUIOff(action.unit);
        QueueAction(newAction);
    }
    private void QueueAction(Action action)
    {
        if (action.unit != null && action.targets != null)
        {
            Director.Instance.timeline.DoCost(CombatTools.DetermineTrueCost(action), action.unit);
            BattleSystem.Instance.AddAction(action);
            BattleSystem.SetUIOff(action.unit);
        }
    }

    public void StartDecision(bool DoesNotSetToUnitPos = true)
    {
        BattleSystem.SetUIOn(this);
        var sprite = GetComponent<SpriteRenderer>();
        if (DoesNotSetToUnitPos)
            LabCamera.Instance.MoveToUnit(this, Vector3.zero, sprite.bounds.center.x / 5f, 0, 0, 1, true);
        else
            LabCamera.Instance.MoveToUnit(this, new Vector3(0, 16.8f, 0), sprite.bounds.center.x / 5f, 0, 0);

        BattleSystem.Instance.state = BattleStates.DECISION_PHASE;
        if(timelinechild != null)
        {
            Director.Instance.timeline.RemoveTimelineChild(this);
        }  
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
        /*if (state != PlayerState.WAITING)
            state = PlayerState.IDLE;
        */

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
        BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase(deathQuotes[UnityEngine.Random.Range(0, deathQuotes.Count)].name), true, true, false);     
        BattleSystem.Instance.BattlePhasePause = true;
    }


    public void ChangeUnitsLight(Light light, float desiredIntensity, float amountToRaiseBy, Color lightColor, float delay = 0, float stagnantDelay = 0)
    {

        Tools.StartAndCheckCoroutine(lightCoroutine, ChangeUnitsLightCoroutine(light, desiredIntensity, amountToRaiseBy, lightColor ,delay, stagnantDelay));
    }
    private IEnumerator ChangeUnitsLightCoroutine(Light light, float desiredIntensity, float amountToRaiseBy, Color lightColor ,float delay = 0, float stagnantDelay = 0)
    {
        light.color = lightColor;
        light.intensity = 0;
        while (light.intensity < desiredIntensity)
        {
            light.intensity += amountToRaiseBy;
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(stagnantDelay);
        while (light != null && light.intensity > 0)
        {
            if (light != null)
            {
                light.intensity -= amountToRaiseBy;
            }
            else
                break;
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

    public void FadeIntent(bool FadeOut)
    {
        Tools.StartAndCheckCoroutine(fadeCoroutine, FadeIntentsCoroutine(FadeOut));
    }
    private IEnumerator FadeIntentsCoroutine( bool FadeOut)
    {
        if (FadeOut)
        {
            intentUI.GetComponent<UnityEngine.UI.Button>().interactable = false;
            StartCoroutine(Tools.FadeObject(intentUI.GetComponent<Image>(), 0.005f, false, false));
            StartCoroutine(Tools.FadeText(intentUI.textMesh, 0.005f, false, false));
            StartCoroutine(Tools.FadeText(intentUI.damageNums, 0.005f, false, false));
            StartCoroutine(Tools.FadeText(intentUI.costNums, 0.005f, false, false));
            yield return new WaitForSeconds(1f);
            intentUI.gameObject.SetActive(false);
        }
        else
        {
            Tools.SetImageColorAlphaToZero(intentUI.GetComponent<Image>());
            intentUI.GetComponent<UnityEngine.UI.Button>().interactable = true;
            StartCoroutine(Tools.FadeObject(intentUI.GetComponent<Image>(), 0.005f, true, false));
            StartCoroutine(Tools.FadeText(intentUI.textMesh, 0.005f, true, false));
            StartCoroutine(Tools.FadeText(intentUI.damageNums, 0.005f, true, false));
            StartCoroutine(Tools.FadeText(intentUI.costNums, 0.005f, true, false));
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

    public void DoOnPlayerUnitDeath()
    {
        OnPlayerUnitDeath?.Invoke(this);
    }




}
