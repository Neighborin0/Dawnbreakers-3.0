using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public abstract class Action : ScriptableObject
{

    //general action parameters
    public string ActionName;
    public int damage;
    public string description;
    public float cost;
    public float duration;
    public string damageText;
    public bool Done = false;
    public bool New = false;
    public int statAmount;
    public int numberofUses;
    public bool limited = false;

    public enum TargetType { ENEMY, SELF, ALL_ENEMIES, ALLY };
    public enum ActionType { ATTACK, STATUS };

    public TextMeshProUGUI text;
    public Unit unit;
    public TargetType targetType;
    public ActionType actionType;
    public Unit targets;

    void Start()
    {
        text.text = ActionName;
        damageText = damage.ToString();
    }

    public void OnActivated(){ Director.Instance.StartCoroutine(ExecuteAction()); }
    public virtual IEnumerator ExecuteAction() { yield break; }

    public virtual string GetDescription() { return ""; }

  
}
