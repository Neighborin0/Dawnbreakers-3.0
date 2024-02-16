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
using static UnityEngine.EventSystems.EventTrigger;
using System.Linq.Expressions;

public enum PlayerState { IDLE, DECIDING, READY, WAITING }
public enum Stat { ATK, DEF, ARMOR, HP }
public class Unit : MonoBehaviour
{

    public string unitName;

    public int currentHP;
    public bool IsPlayerControlled;
    public List<GameObject> skillUIs;
    [NonSerialized]
    public IntentContainer intentUI;
    public Healthbar health;
    [NonSerialized]
    public NamePlate namePlate;
    //[NonSerialized]
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
    [NonSerialized]
    public bool Execute = false;
    public List<UnityEngine.Sprite> MiniMapIcons;
    public List<EffectIcon> statusEffects;
    public Vector3 offset;
    public Vector3 enemyintentOffset;
    public Vector3 camOffset = Vector3.zero;
    public Vector3 CustomScale = Vector3.zero;
    [NonSerialized]
    public TimeLineChild timelinechild;
    [NonSerialized]
    public bool StopMovingToUnit = false;
    [NonSerialized]
    public bool DoesntLoseArmorAtStartOfRound = false;
    IEnumerator lightCoroutine;
    IEnumerator fadeCoroutine;
    [NonSerialized]
    public bool IsHidden;
    private EventTrigger eventTrigger;
    [NonSerialized]
    public bool HasMiniTimelineChild = false;


    public bool OverrideEmission = false;

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
    public event Action<Unit, ActionContainer> OnActionSelected;

    //player states
    public PlayerState state;

    //stats
    public int maxHP;
    public int attackStat;
    public int defenseStat;
    public int armor = 0;


    public Light spotLight;
    public float actionCostMultiplier = 1;
    public float knockbackModifider = 0;
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
    public bool HitEmissionChanged = false;


    void Start()
    {
        currentHP = maxHP;
        state = PlayerState.IDLE;
        spotLight = GetComponentInChildren<Light>();
    }


    void Update()
    {
        if (BattleSystem.Instance == null)
            return;

        var hit = Tools.GetMousePos();
        var sprite = gameObject.GetComponent<SpriteRenderer>();

        if (!Dying && !OverrideEmission)
        {
            if (IsHighlighted)
            {
                sprite.material.SetFloat("_OutlineThickness", 1f);
                sprite.material.SetColor("_OutlineColor", Color.white * 2f);
                sprite.material.SetColor("_Color", new Color(1f, 1f, 1f, 1));
                sprite.material.SetColor("_CharacterEmission", new Color(0.01f, 0.01f, 0.01f));

            }
            else if (IsHidden)
            {
                sprite.material.SetFloat("_OutlineThickness", 1f);
                sprite.material.SetColor("_OutlineColor", Color.black);
                sprite.material.SetColor("_Color", new Color(0.05f, 0.05f, 0.05f, 1f));
                sprite.material.SetColor("_CharacterEmission", new Color(-1f, -1f, -1f, 1f));
            }
            else if (state == PlayerState.DECIDING && IsPlayerControlled)
            {

                sprite.material.SetFloat("_OutlineThickness", 1f);
                sprite.material.SetColor("_OutlineColor", Color.yellow * 2f);
                sprite.material.SetColor("_Color", new Color(1f, 1f, 1f, 1));
                sprite.material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f, 1f));

            }
            else if (state == PlayerState.READY && BattleSystem.Instance.state == BattleStates.DECISION_PHASE && IsPlayerControlled)
            {

                sprite.material.SetFloat("_OutlineThickness", 1f);
                sprite.material.SetColor("_OutlineColor", Color.black);
                sprite.material.SetColor("_Color", new Color(0.2f, 0.2f, 0.2f, 1));
                sprite.material.SetColor("_CharacterEmission", new Color(-0.01f, -0.01f, -0.01f, 0.1f));

            }
            else if(isDarkened)
            {
                sprite.material.SetFloat("_OutlineThickness", 1f);
                sprite.material.SetColor("_OutlineColor", Color.black);
                sprite.material.SetColor("_Color", new Color(0.2f, 0.2f, 0.2f, 1));
                sprite.material.SetColor("_CharacterEmission", new Color(-0.01f, -0.01f, -0.01f, 0.1f));
            }
            else if (HitEmissionChanged)
            {
                sprite.material.SetFloat("_OutlineThickness", 1f);
                sprite.material.SetColor("_OutlineColor", Color.black);
                sprite.material.SetColor("_Color", new Color(1f, 1f, 1f, 1));
                sprite.material.SetColor("_CharacterEmission", new Color(1f, 1f, 1f, 1f));
            }
            else
            {
                sprite.material.SetFloat("_OutlineThickness", 1f);
                sprite.material.SetColor("_OutlineColor", Color.black);
                sprite.material.SetColor("_Color", new Color(1f, 1f, 1f, 1));
                sprite.material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f, 1f));
            }
        }

        // Hit Detection
        if (hit.collider == GetComponent<BoxCollider>() && !isDarkened && !OverUI())
        {
            if (BattleSystem.Instance.state == BattleStates.DECISION_PHASE && CombatTools.CheckIfAnyUnitIsDeciding())
            {
                if (!IsPlayerControlled)
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
                if (timelinechild != null)
                {
                    timelinechild.Shift(this);
                }
            }
        }
        else if (timelinechild != null && !timelinechild.HighlightedIsBeingOverwritten)
        {
            timelinechild.Return();
        }

        // Decision Start
        if (Input.GetMouseButtonUp(0))
        {
            if (hit.collider != null && hit.collider == GetComponent<BoxCollider>() && BattleSystem.Instance.state == BattleStates.DECISION_PHASE && !OverUI() && state != PlayerState.DECIDING)
            {
                if (!CombatTools.CheckIfAnyUnitIsTargetting())
                {
                    BattleLog.Instance.itemText.text = "";
                    BattleLog.Instance.DisplayCharacterStats(this);
                    if (IsPlayerControlled)
                    {
                        foreach (var x in Tools.GetAllUnits())
                        {
                            BattleSystem.SetUIOff(x);
                        }
                    }
                }
            }
            if ((state == PlayerState.IDLE || state == PlayerState.READY) && BattleSystem.Instance.state == BattleStates.DECISION_PHASE && !IsHighlighted && !OverUI())
            {
                if (hit.collider != null && hit.collider == GetComponent<BoxCollider>() && IsPlayerControlled)
                {
                    StopMovingToUnit = false;
                    BattleLog.Instance.DisplayCharacterStats(this);
                    //Cancelling Action
                    if (state == PlayerState.READY)
                    {
                        foreach (var skill in skillUIs)
                        {
                            var actionContainer = skill.GetComponent<ActionContainer>();
                            if (actionContainer.action != null && actionContainer.action.actionStyle != Action.ActionStyle.STANDARD)
                            {
                                CombatTools.ReturnPipCounter().AddPip();
                                actionContainer.action.actionStyle = Action.ActionStyle.STANDARD;
                            }
                        }
                        FadeIntent(true);
                        Director.Instance.timeline.RemoveTimelineChild(this);
                    }
                    StartDecision();
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
            BattleSystem.Instance.DisplayIntent(action, this);
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
        if (timelinechild != null)
        {
            Director.Instance.timeline.RemoveTimelineChild(this);
        }
        AudioManager.QuickPlay("button_Hit_004", true);
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
        if (BattleSystem.Instance.playerUnits.Count != 1)
        {
            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase(deathQuotes[UnityEngine.Random.Range(0, deathQuotes.Count)].name), true, true, false);
        }
        else
        {
            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase(deathQuotes[UnityEngine.Random.Range(0, deathQuotes.Count)].name), true, true, false, false, false);
        }
        BattleSystem.Instance.BattlePhasePause = true;
    }


    public void ChangeUnitsLight(Light light, float desiredIntensity, float amountToRaiseBy, Color lightColor, float delay = 0, float stagnantDelay = 0)
    {

        Tools.StartAndCheckCoroutine(lightCoroutine, ChangeUnitsLightCoroutine(light, desiredIntensity, amountToRaiseBy, lightColor, delay, stagnantDelay));
    }
    private IEnumerator ChangeUnitsLightCoroutine(Light light, float desiredIntensity, float amountToRaiseBy, Color lightColor, float delay = 0, float stagnantDelay = 0)
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
        intentUI.GetComponent<ScalableObject>().DisableScale(0);
    }

    private bool FadingOutIntent;
    private IEnumerator FadeIntentsCoroutine(bool FadeOut)
    {
        if (FadeOut)
        {
            FadingOutIntent = true;
            if (intentUI != null)
            {
                var intentImage = intentUI.GetComponent<Image>();
                intentUI.GetComponent<LabSpriteSwap>().interactable = false;
                intentUI.GetComponent<LabSpriteSwap>().Revert();
                while (intentUI.GetComponent<Image>().color.a > 0 && intentUI.gameObject != null && FadingOutIntent)
                {
                    intentUI.GetComponent<Image>().color = new Color(intentImage.color.r, intentImage.color.g, intentImage.color.b, intentImage.color.a - 0.1f);
                    intentUI.textMesh.color = new Color(intentUI.textMesh.color.r, intentUI.textMesh.color.g, intentUI.textMesh.color.b, intentUI.textMesh.color.a - 0.1f);
                    intentUI.damageNums.color = new Color(intentUI.damageNums.color.r, intentUI.damageNums.color.g, intentUI.damageNums.color.b, intentUI.damageNums.color.a - 0.1f);
                    intentUI.costNums.color = new Color(intentUI.costNums.color.r, intentUI.costNums.color.g, intentUI.costNums.color.b, intentUI.costNums.color.a - 0.1f);
                    yield return new WaitForSeconds(0.01f);
                }
                if (!FadingOutIntent)
                    yield return new WaitUntil(() => intentImage.color.a <= 0);
                if (FadingOutIntent)
                    intentUI.gameObject.SetActive(false);
            }

        }
        else
        {
            FadingOutIntent = false;
            Tools.SetImageColorAlphaToZero(intentUI.GetComponent<Image>());
            intentUI.GetComponent<LabSpriteSwap>().interactable = true;
            var intentImage = intentUI.GetComponent<Image>();
            while (intentUI.GetComponent<Image>().color.a > 0 && intentUI.gameObject != null && !FadingOutIntent)
            {
                intentUI.GetComponent<Image>().color = new Color(intentImage.color.r, intentImage.color.g, intentImage.color.b, intentImage.color.a + 0.1f);
                intentUI.textMesh.color = new Color(intentUI.textMesh.color.r, intentUI.textMesh.color.g, intentUI.textMesh.color.b, intentUI.textMesh.color.a + 0.1f);
                intentUI.damageNums.color = new Color(intentUI.damageNums.color.r, intentUI.damageNums.color.g, intentUI.damageNums.color.b, intentUI.damageNums.color.a + 0.1f);
                intentUI.costNums.color = new Color(intentUI.costNums.color.r, intentUI.costNums.color.g, intentUI.costNums.color.b, intentUI.costNums.color.a + 0.1f);
                yield return new WaitForSeconds(0.01f);
            }
            if (!FadingOutIntent)
                yield return new WaitUntil(() => intentImage.color.a >= 1);
            if (!FadingOutIntent)
                intentUI.gameObject.SetActive(true);
        }

    }
    private bool OverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void OnMouseEnter()
    {
        if (BattleSystem.Instance != null && BattleSystem.Instance.state == BattleStates.DECISION_PHASE && !OverUI())
        {
            AudioManager.QuickPlay("button_hover", true);
        }
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

    public void DoOnActionSelected(ActionContainer actionContainer)
    {
        OnActionSelected?.Invoke(this, actionContainer);
    }




}
