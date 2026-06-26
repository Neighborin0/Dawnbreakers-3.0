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
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, new Vector3(value * offset, rectTransform.anchoredPosition.y), 8 * Time.deltaTime);
            staminaText.text = Mathf.Round(-value + 100).ToString();
        }
       
    }

    public void MoveToNewPosition(Vector2 pos)
    {
        PositionToMoveTo = pos;
    }

    public void Shift(Unit targetUnit)
    {
        if (targetUnit == null ||
            Director.Instance == null ||
            Director.Instance.timeline == null)
        {
            return;
        }

        transform.SetAsLastSibling();

        if (childImage != null &&
            childImage.material != null)
        {
            childImage.material.SetFloat(
                "OutlineThickness",
                1f
            );

            childImage.material.SetColor(
                "OutlineColor",
                Color.white
            );
        }

        TimeLine timeline = Director.Instance.timeline;

        timeline.PruneDestroyedChildren();

        foreach (TimeLineChild timelineChild
                 in timeline.children)
        {
            if (timelineChild == null)
                continue;

            MiniTimelineChildren miniChild =
                timelineChild.miniChild;

            if (miniChild == null ||
                miniChild.unit == null)
            {
                continue;
            }

            if (miniChild.unit != targetUnit)
                continue;

            miniChild.Shift(targetUnit);
            break;
        }
    }
    public void Return()
    {
        if (Director.Instance == null ||
            Director.Instance.timeline == null)
        {
            return;
        }

        TimeLine timeline = Director.Instance.timeline;

        timeline.PruneDestroyedChildren();

        foreach (TimeLineChild timelineChild in timeline.children)
        {
            if (timelineChild == null)
                continue;

            MiniTimelineChildren miniChild = timelineChild.miniChild;

            if (miniChild == null || miniChild.unit == null || unit == null)
            {
                continue;
            }

            if (miniChild.unit != unit)
                continue;

            miniChild.Return();
            break;
        }

        if (childImage == null ||
            childImage.material == null)
        {
            return;
        }

        childImage.material.SetFloat(
            "OutlineThickness",
            0f
        );

        childImage.material.SetColor(
            "OutlineColor",
            Color.black
        );
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

    private bool hasCleanedUp;

    private void OnDestroy()
    {
        CleanupReferences();
    }

    public void CleanupReferences()
    {
        if (hasCleanedUp)
            return;

        hasCleanedUp = true;

        //Remove this child from the timeline collection.
        if (Director.Instance != null &&
            Director.Instance.timeline != null)
        {
            Director.Instance.timeline.children.Remove(this);
        }

        //Clear the Unit's direct reference to this child.
        if (unit != null &&
            unit.timelinechild == this)
        {
            unit.timelinechild = null;
        }

        //Break the mini-child relationship before this parent becomes a destroyed Unity object.
        if (miniChild != null)
        {
            if (miniChild.parent == this)
            {
                miniChild.parent = null;
            }

            if (miniChild.unit != null)
            {
                miniChild.unit.HasMiniTimelineChild = false;
            }

            miniChild = null;
        }
    }
}


