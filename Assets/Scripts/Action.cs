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
    public int accuracy;
    public string description;
    public int speed;
    public bool PriorityMove;
    public float cost;
    public float duration;
    public string damageText;
    public bool Done = false;
    public bool New = false;
    public int statAmount;
  
    public enum TargetType { ANY, SELF, ALL_ENEMIES, ALLIES };
    public enum ActionType { ATTACK, STATUS };

    public TextMeshProUGUI text;
   // public GameObject targettingbutton;
    public Unit unit;
    public TargetType targetType;
    public ActionType actionType;
    public Unit targets;

    void Start()
    {
        text.text = ActionName;
        damageText = damage.ToString();
        //Init();
    }


    //sorry to whoever reads this have an amoungus ඞ
    public void OnActivated(){ Director.Instance.StartCoroutine(ExecuteAction()); }
    public virtual IEnumerator ExecuteAction() { yield break; }

    public virtual string GetDescription() { return ""; }
    public virtual void OnEnded(Unit unit = null, float storedValue = 0, bool DoFancyStatChanges = false) { }

  
}
