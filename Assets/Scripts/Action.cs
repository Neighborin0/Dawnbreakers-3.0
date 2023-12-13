using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using System;

public enum DamageType
{
    NULL,
    //physical types
    SLASH,
    PIERCE,
    STRIKE,
    CLEAVE,
    //elements
    LIGHT,
    DARK,
    COLD,
    HEAT
};
public abstract class Action : ScriptableObject
{

    //general action parameters
    
    public ActionStyle actionStyle;
    public string ActionName;
    public string description;
    public float duration;
    public string damageText;
    public bool Done = false;
    public bool New = false;
    public int numberofUses;
    public bool limited = false;
    public bool CanBeStyled = true;

    public int damage;
    public int lightDamage;
    public int heavyDamage;

    public int statAmount;
    public int lightStatAmount;
    public int heavyStatAmount;

    public float cost;
    public float lightCost;
    public float heavyCost;



    public enum TargetType { ENEMY, SELF, ALL_ENEMIES, ALLY };
    public enum ActionType { ATTACK, STATUS };
    public enum ActionStyle { STANDARD, LIGHT, HEAVY };
  

    public TextMeshProUGUI text;
    public Unit unit;
    public TargetType targetType;
    public ActionType actionType;
    public DamageType damageType;
    public Unit targets;

    void Start()
    {
        text.text = ActionName;
        damageText = damage.ToString();
        actionStyle = ActionStyle.STANDARD;   
    }

    public void ResetAction()
    {
        var newAction = Instantiate(this);
        if (actionType == Action.ActionType.STATUS && statAmount != 0)
        {
            statAmount = newAction.statAmount;
        }
        else
        {
            damage = newAction.damage;
        }
        cost = newAction.cost;
    }
    public void OnActivated(){ Director.Instance.StartCoroutine(ExecuteAction()); }
    public virtual IEnumerator ExecuteAction() { yield break; }

    public virtual string GetDescription() { return ""; }

  
}
