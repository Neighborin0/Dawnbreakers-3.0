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
    //public TextMeshProUGUI num;
    public Vector2 PositionToMoveTo;
    public TextMeshProUGUI stamina;
    public GameObject TimelineChildChild;
    public GameObject playerPoint;
    public GameObject EnemyPoint;
    public bool UnitIsHighlighted;
    public bool HighlightedIsBeingOverwritten = false;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    private void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0);
        gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);

        TimelineChildChild.GetComponent<Image>().material = Instantiate<Material>(TimelineChildChild.GetComponent<Image>().material);
        TimelineChildChild.GetComponent<Image>().material.SetFloat("OutlineThickness", 0);
        TimelineChildChild.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);


        EnemyPoint.GetComponent<Image>().material = Instantiate<Material>(EnemyPoint.GetComponent<Image>().material);
        EnemyPoint.GetComponent<Image>().material.SetFloat("OutlineThickness", 0);
        EnemyPoint.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);


        playerPoint.GetComponent<Image>().material = Instantiate<Material>(playerPoint.GetComponent<Image>().material);
        playerPoint.GetComponent<Image>().material.SetFloat("OutlineThickness", 0);
        playerPoint.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);
    }
    void LateUpdate()
    {
        if(CanMove)
        {        
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, new Vector3(unit.stamina.slider.value * -11.89f, rectTransform.anchoredPosition.y), Director.Instance.timelinespeedDelay);
            stamina.text = Mathf.Round(unit.stamina.slider.value).ToString();
        }
        TimelineChildChild.GetComponent<Image>().color = GetComponent<Image>().color;
        /*if(unit.IsHighlighted)
        {
            UnitIsHighlighted = true;
        }
        else
        {
            UnitIsHighlighted = false;
        }
        */

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
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);

            TimelineChildChild.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
            TimelineChildChild.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);

            EnemyPoint.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
            EnemyPoint.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);

            playerPoint.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
            playerPoint.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
        }

    }
    public void Return()
    {
        if(gameObject != null)
        {
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);

            TimelineChildChild.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
            TimelineChildChild.GetComponent<Image>().material.SetColor("OutlineColor", Color.black);

            EnemyPoint.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
            EnemyPoint.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);

            playerPoint.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
            playerPoint.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
        }
       
    }


    public void ToggleHightlightOnUnit()
    {
        if(!UnitIsHighlighted && BattleSystem.Instance.state != BattleStates.BATTLE && BattleSystem.Instance.state != BattleStates.START && BattleSystem.Instance.state != BattleStates.WON && BattleSystem.Instance.state != BattleStates.DEAD)
        {
            unit.IsHighlighted = true;
            UnitIsHighlighted = true;
            HighlightedIsBeingOverwritten = true;
            BattleLog.Instance.DisplayEnemyCharacterStats(unit);
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
