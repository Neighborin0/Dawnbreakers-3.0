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
        if(CanMove)
        {        
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, new Vector3(value * offset, rectTransform.anchoredPosition.y), Director.Instance.timelinespeedDelay);
            staminaText.text = Mathf.Round(value).ToString();
        }

    }
    public void MoveToNewPosition(Vector2 pos) 
    {
        PositionToMoveTo = pos;
    }

    public void Shift(Unit unit)
    {

        if (gameObject != null )
        {
            transform.SetAsLastSibling();
            childImage.material.SetFloat("OutlineThickness", 1f);
            childImage.material.SetColor("OutlineColor", Color.white);

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

    public IEnumerator FadeOut()
    {
        if (gameObject != null)
        {
            while (childImage.color.a > 0 && gameObject != null)
            {
                childImage.color = new Color(childImage.color.r, childImage.color.g, childImage.color.b, childImage.color.a - 0.1f);
                portrait.color = new Color(portrait.color.r, portrait.color.g, portrait.color.b, portrait.color.a - 0.1f);
                staminaText.color = new Color(staminaText.color.r, staminaText.color.g, staminaText.color.b, staminaText.color.a - 0.1f);
            }  
        }
        yield return new WaitUntil(() => childImage.color.a <= 0);
        if (gameObject != null)
            Destroy(gameObject);
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
