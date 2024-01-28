using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeLineChild : MonoBehaviour
{
    public Image portrait;
    public Unit unit;
    public bool CanMove = true;
    public RectTransform rectTransform;
    public bool CanClear = false;
    public Vector2 PositionToMoveTo;
    public TextMeshProUGUI staminaText;
    public Image childImage;
    public bool UnitIsHighlighted;
    public bool HighlightedIsBeingOverwritten = false;
    public bool CanBeHighlighted = true;
    public float value;
    public float offset = -12.13f;

    public MiniTimelineChildren PlayerMiniChild;
    public MiniTimelineChildren EnemyMiniChild;
    public MiniTimelineChildren miniChild;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    private void Start()
    {
        childImage.material = Instantiate<Material>(childImage.material);
        childImage.material.SetFloat("OutlineThickness", 0);
        childImage.material.SetColor("OutlineColor", Color.black);

    }
    void LateUpdate()
    {
        if (CanMove)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, new Vector3(value * offset, rectTransform.anchoredPosition.y), 0.1f);
            staminaText.text = Mathf.Round(value).ToString();
            foreach (var TL in Director.Instance.timeline.children)
            {
                if (!TL.CanClear)
                {


                    if (TL != null && TL.unit != null && unit != null)
                    {
                        if (unit.IsPlayerControlled && TL.unit.IsPlayerControlled && TL.unit != unit)
                        {
                            if (value == TL.value)
                            {
                                SetupMiniChild(TL.unit);
                                TL.gameObject.SetActive(false);
                                break;
                            }

                            else
                            {
                                TL.gameObject.SetActive(true);
                                PlayerMiniChild.gameObject.SetActive(false);
                            }
                        }
                        else if (!unit.IsPlayerControlled && !TL.unit.IsPlayerControlled && TL.unit != unit)
                        {
                            if (value == TL.value)
                            {
                                SetupMiniChild(TL.unit);
                                TL.gameObject.SetActive(false);
                                break;
                            }

                            else
                            {
                                TL.gameObject.SetActive(true);
                                PlayerMiniChild.gameObject.SetActive(false);
                            }
                        }

                    }
                }
            }
        }

    }
    public void MoveToNewPosition(Vector2 pos)
    {
        PositionToMoveTo = pos;
    }

    public void Shift(Unit unit)
    {

        if (gameObject != null)
        {
            foreach (var TL in Director.Instance.timeline.children)
            {
                if (TL.miniChild != null && TL.miniChild.unit == unit)
                {
                    TL.miniChild.Shift(unit);
                    break;
                }
            }
            transform.SetAsLastSibling();
            childImage.material.SetFloat("OutlineThickness", 1f);
            childImage.material.SetColor("OutlineColor", Color.white);

        }

    }
    public void Return()
    {
        if (gameObject != null)
        {
            foreach (var TL in Director.Instance.timeline.children)
            {
                if (TL.miniChild != null && TL.miniChild.unit == unit)
                {
                    TL.miniChild.Return();
                    break;
                }
            }
            childImage.material.SetFloat("OutlineThickness", 0);
            childImage.material.SetColor("OutlineColor", Color.black);
        }

    }


    public void ToggleHightlightOnUnit(bool ForceOff = false)
    {
        if (CanBeHighlighted && !ForceOff)
        {
            if (!UnitIsHighlighted && BattleSystem.Instance.state != BattleStates.BATTLE && BattleSystem.Instance.state != BattleStates.START && BattleSystem.Instance.state != BattleStates.WON && BattleSystem.Instance.state != BattleStates.DEAD)
            {
                unit.IsHighlighted = true;
                UnitIsHighlighted = true;
                HighlightedIsBeingOverwritten = true;
                if (unit.IsPlayerControlled && BattleSystem.Instance.state != BattleStates.DECISION_PHASE)
                    BattleLog.Instance.DisplayCharacterStats(unit);
                else if (!unit.IsPlayerControlled)
                    BattleLog.Instance.DisplayCharacterStats(unit);

                transform.SetAsLastSibling();
                Shift(unit);
            }
            else
            {
                unit.IsHighlighted = false;
                HighlightedIsBeingOverwritten = false;
                UnitIsHighlighted = false;
                Return();
            }
        }
    }

    public void SetupMiniChild(Unit TargetUnit)
    {
        var miniChild = ReturnMiniChild(TargetUnit);

        miniChild.unit = TargetUnit;
        miniChild.portrait.sprite = TargetUnit.charPortraits[0];
        miniChild.parent = this;
        miniChild.gameObject.SetActive(true);
        TargetUnit.HasMiniTimelineChild = true;
    }

    public MiniTimelineChildren ReturnMiniChild(Unit TargetUnit)
    {
        if (TargetUnit.IsPlayerControlled)
            miniChild = PlayerMiniChild;
        else
            miniChild = EnemyMiniChild;


        return miniChild;
    }
    public void RemoveMiniChild(Unit TargetUnit)
    {
        var miniChild = ReturnMiniChild(TargetUnit);
        miniChild.gameObject.SetActive(false);
        TargetUnit.HasMiniTimelineChild = false;
    }

}
