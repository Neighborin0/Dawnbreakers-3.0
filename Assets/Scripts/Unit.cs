using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public enum PlayerState { IDLE, DECIDING, READY }
public enum Stat { ATK, DEF, SPD, HP }
public class Unit : MonoBehaviour
{
    public string unitName;
    public int currentHP;
    public bool IsPlayerControlled;
    public List<GameObject> skillUIs;
    public IntentContainer intentUI;
    public Healthbar health;
    public StaminaBar stamina;
    public NamePlate namePlate;
    public bool IsHighlighted;
    public bool isDarkened;
    public bool IsDead;
    public bool StaminaHighlightIsDisabled = false;
    public EnemyBehavior behavior;
    public List<Sprite> charPortraits;
    public Animator anim;
    public bool Execute = false;
    public List<Sprite> MiniMapIcons;
    public Vector3 offset;
    public TimeLineChild timelinechild;
    bool StopMovingToUnit = false;

    //text stuff
    public List<string> introText;
    public List<Item> inventory;

    //action events
    public event Action<Unit> BattlePhaseEnd;
    public event Action<Unit> BattlePhaseClose;
    public event Action<Unit> OnDamaged;
    public event Action<Unit> BattleStarted;
    public event Action<Unit> BattleEnded;
    public event Action<Unit> ActionEnded;
    public event Action<Unit> EnteredMap;

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

    //summon stuff 
    public string[] summonables;
    public bool IsSummon = false;
    void Awake()
    {
        spotLight = GetComponentInChildren<Light>();
    }
    void Start()
    {
        currentHP = maxHP;
        Debug.Log(maxHP);
        Debug.Log(attackStat);
        Debug.Log(defenseStat);
        Debug.Log(speedStat);
        state = PlayerState.IDLE;
        var particleSys = this.GetComponent<ParticleSystem>();
        particleSys.Stop();
    }


    void Update()
    {
        var hit = Tools.GetMousePos();
        var sprite = this.gameObject.GetComponent<SpriteRenderer>();
        if (BattleSystem.Instance != null)
        {
            if (IsHighlighted)
            {
                sprite.material.SetFloat("_OutlineThickness", 1f);
                sprite.material.SetColor("_OutlineColor", Color.white);
                sprite.material.SetColor("_CharacterEmission", new Color(0.1f, 0.1f, 0.1f));

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
       /* if (state == PlayerState.READY && anim != null && IsPlayerControlled)
        {
            anim.SetBool("READY", true);
        }
        else if (anim != null && IsPlayerControlled)
        {
            anim.SetBool("READY", false);
        }
       */

        if (BattleSystem.Instance != null)
        {

            if (hit.collider == this.GetComponent<BoxCollider>() && !isDarkened && BattleSystem.Instance.state == BattleStates.DECISION_PHASE)
            {
                sprite.material.SetColor("_CharacterEmission", new Color(0.1f, 0.1f, 0.1f));
                BattleLog.DisplayCharacterStats(this);

            }
            if (isDarkened && BattleSystem.Instance.state == BattleStates.DECISION_PHASE)
            {
                sprite.material.SetColor("_CharacterEmission", new Color(-0.1f, -0.1f, -0.1f));
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (hit.collider != null && hit.collider == this.GetComponent<BoxCollider>() && BattleSystem.Instance.state == BattleStates.DECISION_PHASE)
                {
                    BattleLog.DisplayCharacterStats(this);
                    LabCamera.Instance.MoveToUnit(this);
                }
            }

            if (state == PlayerState.IDLE && BattleSystem.Instance.state == BattleStates.DECISION_PHASE || state == PlayerState.READY && BattleSystem.Instance.state == BattleStates.DECISION_PHASE)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (hit.collider != null && hit.collider == this.GetComponent<BoxCollider>() && this.IsPlayerControlled)
                    {
                        StopMovingToUnit = false;
                        BattleSystem.Instance.ResetBattleLog();
                        BattleLog.DisplayCharacterStats(this);
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
        print("Unit is deciding an action");
    }

    

    public void ExitDecision()
    {  
        
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
        print("no thinky");
    }

    public IEnumerator ExecuteAnimator()
    {
        Execute = true;
        yield return new WaitForSeconds(0.00001f);
        Execute = false;
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

    public void DoBattlePhaseClose()
    {
        BattlePhaseClose?.Invoke(this);
    }

    public void DoEnteredMap()
    {
        EnteredMap?.Invoke(this);
    }




}
