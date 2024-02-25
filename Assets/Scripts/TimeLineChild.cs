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
    public bool PortraitHasBeenReplaced = false;
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
            staminaText.text = Mathf.Round(-value + 100).ToString();
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
            transform.SetAsLastSibling();
            childImage.material.SetFloat("OutlineThickness", 1f);
            childImage.material.SetColor("OutlineColor", Color.white);

            foreach (var TL in Director.Instance.timeline.children)
            {
                if (TL.miniChild != null && TL.miniChild.unit == unit)
                {
                    TL.miniChild.Shift(unit);
                    break;
                }
            }
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

   
    public void ToggleHightlightOnUnit()
    {
        if (CanBeHighlighted)
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

   
}