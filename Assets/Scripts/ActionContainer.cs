﻿using System.Collections;
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


    void Awake()
    {
        if (BattleSystem.Instance != null)
        {
            button.interactable = true;
            Unit[] units = Tools.GetAllUnits();
            foreach (var unit in units)
            {
                unit.IsHighlighted = false;
                unit.isDarkened = false;
            }
        }
    }

    void Update()
    {
        var hit = Tools.GetMousePos();

        if (targetting && BattleSystem.Instance.state != BattleStates.WON)
        {

            if (hit.collider != null && hit.collider.gameObject.GetComponent<BoxCollider>() != null && hit.collider.gameObject.GetComponent<Unit>() != null && action.targetType == Action.TargetType.ANY && action.actionType == Action.ActionType.ATTACK && hit.collider.gameObject.GetComponent<Unit>().IsHighlighted)
            {
                damageNums.text = (action.damage + baseUnit.attackStat - hit.collider.gameObject.GetComponent<Unit>().defenseStat).ToString();
                if (action.damage + baseUnit.attackStat - hit.collider.gameObject.GetComponent<Unit>().defenseStat > action.damage + baseUnit.attackStat)
                {
                    damageNums.color = Color.green;
                }
                else if (action.damage + baseUnit.attackStat - hit.collider.gameObject.GetComponent<Unit>().defenseStat < action.damage + baseUnit.attackStat)
                {
                    damageNums.color = Color.red;
                }
            }
            else if (action.targetType == Action.TargetType.ANY && action.actionType == Action.ActionType.ATTACK)
            {
                damageNums.text = (action.damage + baseUnit.attackStat).ToString();
                damageNums.color = Color.white;
            }

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

                        if (hit.collider != null && hit.collider.gameObject != null && hit.collider != baseUnit.gameObject.GetComponent<BoxCollider>() && hit.collider.gameObject.GetComponent<Unit>() != null)
                        {
                            baseUnit.state = PlayerState.READY;
                            var unit = hit.collider.GetComponent<Unit>();
                            foreach (var z in Tools.GetAllUnits())
                            {
                                z.IsHighlighted = false;
                                z.isDarkened = false;
                            }
                            action.targets = unit;
                            action.unit = baseUnit;


                            baseUnit.Queue(action);
                            this.targetting = false;
                            LabCamera.Instance.ResetPosition();
                            BattleSystem.Instance.ResetBattleLog();
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
                            action.speed = action.unit.speedStat;
                            baseUnit.Queue(action);
                            SetActive();
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
                            unit.state = PlayerState.DECIDING;
                        }

                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<BoxCollider>() && hit.collider.gameObject.GetComponent<Unit>() != null && hit.collider.gameObject.GetComponent<Unit>().IsPlayerControlled)
                        {
                            baseUnit.state = PlayerState.READY;
                            Debug.LogError("IS ALLY SELECTION EVEN WORKING?");
                            var unit = hit.collider.GetComponent<Unit>();
                            foreach (var z in Tools.GetAllUnits())
                            {
                                z.IsHighlighted = false;
                                z.isDarkened = false;
                            }
                            action.targets = unit;
                            action.unit = baseUnit;
                            baseUnit.Queue(action);
                            this.targetting = false;
                            LabCamera.Instance.ResetPosition();
                            BattleSystem.Instance.ResetBattleLog();
                            SetActive();
                        }
                    }
                    break;
            }
        }
        else
        {
            if (damageNums != null)
                damageNums.color = Color.white;
        }

    }

    public void SetDescription()
    {
        if (button.interactable)
        {
            AudioManager.Instance.Play("ButtonHover");
            BattleLog.SetBattleText("");
            BattleLog.SetBattleText(action.description);
            BattleLog.Instance.Move(true);
        }
    }

    public void RemoveDescription()
    {
        if (button.interactable)
            BattleSystem.Instance.ResetBattleLog();
    }
    public void SetActive()
    {
        damageNums.color = Color.white;
        if (BattleSystem.Instance != null)
        {
            if (targetting == true)
            {
                targetting = false;
                BattleLog.SetBattleText("");
                //button.interactable = true;
                print("not targetting");
                ActionContainer[] actionContainers = UnityEngine.Object.FindObjectsOfType<ActionContainer>();
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
                        button.interactable = true;
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


                var TL = Instantiate(Director.Instance.timeline.borderChildprefab, Director.Instance.timeline.startpoint);
                TL.unit = baseUnit;
                TL.portrait.sprite = baseUnit.charPortraits[0];
                Director.Instance.timeline.children.Add(TL);
                TL.CanMove = false;
                TL.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 50);
                TL.rectTransform.anchoredPosition = new Vector3((TL.unit.stamina.slider.maxValue - action.cost) * -15.74f, TL.rectTransform.anchoredPosition.y);
                TL.CanClear = true;
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