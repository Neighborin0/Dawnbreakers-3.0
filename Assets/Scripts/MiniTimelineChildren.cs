using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniTimelineChildren : MonoBehaviour
{
    public Image portrait;
    public Unit unit;
    public bool CanMove = true;
    public RectTransform rectTransform;
    public bool CanClear = false;
    public Image childImage;
    public bool UnitIsHighlighted;
    public bool HighlightedIsBeingOverwritten = false;
    public bool CanBeHighlighted = true;
    public TimeLineChild parent;
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


    public void Shift(Unit unit)
    {

        if (gameObject != null )
        {
            transform.SetAsLastSibling();
            childImage.material.SetFloat("OutlineThickness", 1f);
            childImage.material.SetColor("OutlineColor", Color.white);

            parent.Return();

        }

    }
    public void Return()
    {
        if(gameObject != null)
        {
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
                if(unit.IsPlayerControlled && BattleSystem.Instance.state != BattleStates.DECISION_PHASE)
                    BattleLog.Instance.DisplayCharacterStats(unit);
                else if(!unit.IsPlayerControlled)
                    BattleLog.Instance.DisplayCharacterStats(unit);

                transform.SetAsLastSibling();
                Shift(unit);
                parent.Return();
                parent.unit.IsHighlighted = false;

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
