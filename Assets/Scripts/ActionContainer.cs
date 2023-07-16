using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class ActionContainer : MonoBehaviour
{
    public Action action;
    public TextMeshProUGUI textMesh;
    public bool targetting = false;
    public Unit baseUnit;
    public Button button;
    //public Button overlayButton;
    public TextMeshProUGUI damageNums;
    public TextMeshProUGUI costNums;
    public TextMeshProUGUI durationNums;
    public GameObject damageParent;
    public GameObject costParent;
    public GameObject durationParent;
    private float newTimeLineSpeedDelay = 0.1f;
    public bool Disabled = false;
    public int numberofUses;
    public bool limited = false;

    void Awake()
    {
        if (BattleSystem.Instance != null)
        {
            if(!this.Disabled)
            button.interactable = true;
            Unit[] units = Tools.GetAllUnits();
            foreach (var unit in units)
            {
                unit.IsHighlighted = false;
                unit.isDarkened = false;
            }
        }
        numberofUses = action.numberofUses;
        limited = action.limited;
    }

    void Update()
    {
        var hit = Tools.GetMousePos();
        if (hit.collider != null && hit.collider.gameObject.GetComponent<BoxCollider>() != null && hit.collider.gameObject.GetComponent<Unit>() != null && action.targetType == Action.TargetType.ANY && action.actionType == Action.ActionType.ATTACK && hit.collider.gameObject.GetComponent<Unit>().IsHighlighted && !hit.collider.gameObject.GetComponent<Unit>().IsPlayerControlled)
        {
            var unit = hit.collider.gameObject.GetComponent<Unit>();
            unit.timelinechild.Shift(unit);
            damageNums.text = action.damage + baseUnit.attackStat - unit.defenseStat > 0 ?
                "<sprite name=\"ATK\">" + (action.damage + baseUnit.attackStat - unit.defenseStat).ToString()
                : "<sprite name=\"ATK\">" + "0";

            if (action.damage + baseUnit.attackStat - unit.defenseStat > action.damage + baseUnit.attackStat)
            {
                damageNums.color = Color.green;
            }
            else if (action.damage + baseUnit.attackStat - unit.defenseStat < action.damage + baseUnit.attackStat)
            {
                damageNums.color = Color.red;
            }
        }
        else if (action.targetType == Action.TargetType.ANY && action.actionType == Action.ActionType.ATTACK)
        {
            damageNums.text = "<sprite name=\"ATK\">" + (action.damage + baseUnit.attackStat).ToString();
            damageNums.color = new Color(1, 0.8705882f, 0.7058824f);
        }
        if (targetting && BattleSystem.Instance.state != BattleStates.WON)
        {

            

            switch (action.targetType)
            {
                case Action.TargetType.ANY:
                    foreach (var unit in Tools.GetAllUnits())
                    {
                        if (!unit.IsPlayerControlled)
                        {
                            unit.IsHighlighted = true;
                        }
                        else
                        {
                            unit.isDarkened = true;
                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {

                        if (hit.collider != null && hit.collider.gameObject != null && hit.collider != baseUnit.gameObject.GetComponent<BoxCollider>() && hit.collider.gameObject.GetComponent<Unit>() != null && !hit.collider.gameObject.GetComponent<Unit>().IsPlayerControlled)
                        {
                            baseUnit.state = PlayerState.READY;
                            var unit = hit.collider.GetComponent<Unit>();
                            RemoveDescription();
                            foreach (var z in Tools.GetAllUnits())
                            {
                                z.IsHighlighted = false;
                                z.isDarkened = false;
                            }
                            action.targets = unit;
                            action.unit = baseUnit;


                            baseUnit.Queue(action);
                            baseUnit.timelinechild.CanMove = true;
                            Director.Instance.timelinespeedDelay = newTimeLineSpeedDelay;
                            this.targetting = false;
                            LabCamera.Instance.ResetPosition();
                            BattleLog.Instance.ResetBattleLog();
                            SetActive();
                        }


                    }
                    break;
                case Action.TargetType.SELF:
                    foreach (var unit in Tools.GetAllUnits())
                    {
                        if (unit != baseUnit)
                        {
                            unit.isDarkened = true;
                        }
                        baseUnit.IsHighlighted = true;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        var bU = Tools.GetMousePos();
                        if (hit.collider != null && bU.collider.gameObject != null && bU.collider == baseUnit.gameObject.GetComponent<BoxCollider>())
                        {
                            baseUnit.state = PlayerState.READY;
                            action.targets = baseUnit;
                            action.unit = baseUnit;
                            baseUnit.Queue(action);
                            SetActive();
                            baseUnit.timelinechild.CanMove = true;
                            Director.Instance.timelinespeedDelay = newTimeLineSpeedDelay;
                            RemoveDescription();
                            foreach (var unit in Tools.GetAllUnits())
                            {
                                unit.IsHighlighted = false;
                                unit.isDarkened = false;
                            }

                        }
                    }
                    break;
                case Action.TargetType.ALLIES:
                    foreach (var unit in Tools.GetAllUnits())
                    {
                        if (!unit.IsPlayerControlled)
                        {
                            unit.isDarkened = true;
                        }
                        else
                        {
                            unit.IsHighlighted = true;    
                        }

                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<BoxCollider>() && hit.collider.gameObject.GetComponent<Unit>() != null && hit.collider.gameObject.GetComponent<Unit>().IsPlayerControlled)
                        {
                            baseUnit.state = PlayerState.READY;
                            BattleLog.Instance.ResetBattleLog();
                            this.targetting = false;
                            var unit = hit.collider.GetComponent<Unit>();
                            foreach (var z in Tools.GetAllUnits())
                            {
                                z.IsHighlighted = false;
                                z.isDarkened = false;
                            }
                            action.targets = unit;
                            action.unit = baseUnit;
                            baseUnit.Queue(action);
                            LabCamera.Instance.ResetPosition();
                            RemoveDescription();
                            baseUnit.timelinechild.CanMove = true;
                            Director.Instance.timelinespeedDelay = newTimeLineSpeedDelay;
                            SetActive();
                        }
                    }
                    break;
            }
        }
        else
        {
            if (damageNums != null)
                damageNums.color = new Color(1, 0.8705882f, 0.7058824f);
        }

    }

    public void SetDescription()
    {
       
        AudioManager.Instance.Play("ButtonHover");
        action.unit = baseUnit;
        if (Director.Instance.timeline.gameObject.activeSelf)
        {
            if(limited)
                BattleLog.Instance.DoBattleText($"{action.GetDescription()}\nUses: {numberofUses}.");
            else
                BattleLog.Instance.DoBattleText($"{action.GetDescription()}");
        }
        else
        {
            if(this.GetComponent<Button>().interactable)
            {
                BattleLog.Instance.itemText.gameObject.SetActive(false);
                BattleLog.Instance.ambientText.gameObject.SetActive(true);
                BattleLog.Instance.ambientText.text = $"{action.ActionName}\n{action.GetDescription()}";
                if (limited)
                    BattleLog.Instance.ambientText.text = $"{action.ActionName}\n{action.GetDescription()}\nUses: {numberofUses}.";
                else
                    BattleLog.Instance.ambientText.text = $"{action.ActionName}\n{action.GetDescription()}";
                BattleLog.Instance.GetComponent<MoveableObject>().Move(true);
            }
        
        }


    }


    public void RemoveDescription()
    {
        if(BattleSystem.Instance != null && targetting == false)
        {
            BattleLog.Instance.itemText.gameObject.SetActive(false);
        }
       
    }
    public void SetActive()
    {
        if(Director.Instance.timeline.gameObject.activeSelf)
        {
            damageNums.color = new Color(1, 0.8705882f, 0.7058824f);
            if (BattleSystem.Instance != null)
            {
                if (targetting == true)
                {
                    targetting = false;
                    BattleLog.Instance.DoBattleText("");
                    print("not targetting");
                    foreach (TimeLineChild child in Director.Instance.timeline.children)
                    {
                        if (child.CanClear)
                        {
                            Director.Instance.timeline.children.Remove(child);
                            Destroy(child.gameObject);
                            break;
                        }
                    }
                }
                else
                {
                    ActionContainer[] actionContainers = UnityEngine.Object.FindObjectsOfType<ActionContainer>();
                    foreach (var x in actionContainers)
                    {
                        if (x != this)
                        {
                            var button = x.GetComponent<Button>();
                            if (!x.Disabled)
                            {
                                if (x.limited)
                                {
                                    if (numberofUses > 0)
                                    {
                                        button.interactable = true;
                                    }
                                    else
                                    {
                                        button.interactable = false;
                                    }
                                }
                                else
                                    button.interactable = true;
                            } 
                            x.targetting = false;
                            foreach (var z in Tools.GetAllUnits())
                            {
                                z.IsHighlighted = false;
                                z.isDarkened = false;
                            }

                        }

                    }
                    foreach (TimeLineChild child in Director.Instance.timeline.children)
                    {
                        if (child.CanClear)
                        {
                            Director.Instance.timeline.children.Remove(child);
                            Destroy(child.gameObject);
                            break;
                        }
                    }

                    RemoveDescription();
                    var TL = Instantiate(Director.Instance.timeline.borderChildprefab, Director.Instance.timeline.startpoint);
                    TL.unit = baseUnit;
                    TL.portrait.sprite = baseUnit.charPortraits[0];
                    Director.Instance.timeline.children.Add(TL);
                    TL.CanMove = false;
                    TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 50);
                    TL.rectTransform.anchoredPosition = new Vector3((TL.unit.stamina.slider.maxValue - action.cost) * -11.89f, 85);
                    TL.stamina.text = (TL.unit.stamina.slider.maxValue - action.cost).ToString();
                    TL.CanClear = true;
                    TL.playerPoint.gameObject.SetActive(true);
                    TL.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                    TL.portrait.color = new Color(1, 1, 1, 0.5f);
                    targetting = true;
                    print("targetting");
                    SetDescription();
                    button.interactable = false;
                }
            }
            else
            {
                SetDescription();
            }

        }

    }

}
