using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rat : Unit
{
    private int[] ActionVariance01 = { 40, 45, 50, 55, 60 };
    private int[] ActionVariance02 = { 20, 25, 30, 35, 40 };
    [SerializeField]
    private GameObject TutorialIcon4;
    void Awake()
    {
        unitName = "Vermin";
        maxHP = 22;
        attackStat = UnityEngine.Random.Range(0, 2);
        defenseStat = UnityEngine.Random.Range(0, 2);
        //speedStat = UnityEngine.Random.Range(9, 11);
        currentHP = maxHP;
        IsPlayerControlled = false;
        //behavior = this.gameObject.AddComponent<VerminBehavior>();
        behavior = this.gameObject.AddComponent<TutorialVerminBehavior>();
        resistances = new DamageType[] { DamageType.STRIKE };
        weaknesses = new DamageType[] { DamageType.SLASH, DamageType.PIERCE };
        /*CombatTools.ModifyAction(this, "Strike", 0, ActionVariance01[UnityEngine.Random.Range(0, ActionVariance01.Length)]);
        CombatTools.ModifyAction(this, "Enrage", 1, ActionVariance02[UnityEngine.Random.Range(0, ActionVariance02.Length)]);
        CombatTools.ModifyAction(this, "Screech", 2, ActionVariance02[UnityEngine.Random.Range(0, ActionVariance02.Length)]);
        */

    }


    public class VerminBehavior : EnemyBehavior
    {
        private int[] ActionVariance01 = { 40, 45, 50, 55, 60 };
        private int[] ActionVariance02 = { 20, 25, 30, 35, 40 };
        private Action lastAction = null;
        public override void DoBehavior(Unit baseUnit)
        {
            var battlesystem = BattleSystem.Instance;
            int move;

            CombatTools.ModifyAction(baseUnit, "Strike", 0, ActionVariance01[UnityEngine.Random.Range(0, ActionVariance01.Length)]);
            CombatTools.ModifyAction(baseUnit, "Enrage", 1, ActionVariance02[UnityEngine.Random.Range(0, ActionVariance02.Length)]);
            CombatTools.ModifyAction(baseUnit, "Screech", 2, ActionVariance02[UnityEngine.Random.Range(0, ActionVariance02.Length)]);
            if (turn == 0)
            {
                move = UnityEngine.Random.Range(0, baseUnit.actionList.Count);
                lastAction = baseUnit.actionList[move];
                CombatTools.SetupEnemyAction(baseUnit, move);
                turn++;
            }
            else if (lastAction.actionType == Action.ActionType.STATUS)
            {
                move = 0;
                lastAction = baseUnit.actionList[move];
                CombatTools.SetupEnemyAction(baseUnit, move, null);
            }
            else if (lastAction.actionType == Action.ActionType.ATTACK)
            {
                move = UnityEngine.Random.Range(1, baseUnit.actionList.Count);
                CombatTools.SetupEnemyAction(baseUnit, move, null);
            }
        }
    }

    public class TutorialVerminBehavior : EnemyBehavior
    {
        public override void DoBehavior(Unit baseUnit)
        {
            if (turn == 0)
            {
                baseUnit.BattlePhaseEnd += WulfricSpeakYourShit;
            }

            if (turn == 1)
            {
                CombatTools.ReturnPipCounter().pipCount = 0;
                foreach (Transform pip in CombatTools.ReturnPipCounter().gameObject.transform)
                {
                    Destroy(pip.gameObject);
                }
                CombatTools.ReturnPipCounter().AddPip();
                CombatTools.ReturnPipCounter().AddPip();
            }
            
            CombatTools.SetupEnemyAction(baseUnit, turn);
            turn++;
        }

        private void WulfricSpeakYourShit(Unit obj)
        {
            obj.BattlePhaseEnd -= WulfricSpeakYourShit;
            BattleSystem.Instance.canvas.gameObject.SetActive(false);
            BattleSystem.Instance.BattlePhasePause = true;
            StartCoroutine(WulfricSpeakYourShitCoroutine());
        }

        private IEnumerator WulfricSpeakYourShitCoroutine()
        {
            BattleLog.Instance.CharacterDialog(Director.Instance.FindObjectFromDialogueDatabase("VerminDusty"), true, false, false, false, true, false);
            foreach (var popup in GameObject.FindObjectsOfType<LabPopup>())
            {
                Destroy(popup.gameObject);
            }
            yield return new WaitUntil(() => BattleLog.Instance.state != BattleLogStates.TALKING);
            yield return new WaitForSeconds(0.1f);
            foreach (var unit in BattleSystem.Instance.playerUnits)
            {
                foreach (var skill in unit.skillUIs)
                {
                    var actionContainer = skill.GetComponent<ActionContainer>();
                    if (actionContainer.action != null && actionContainer.action.actionType != Action.ActionType.ATTACK)
                    {
                        actionContainer.Disabled = true;
                        actionContainer.button.interactable = false;
                    }
                }
            }
            BattleSystem.Instance.BattlePhasePause = false;
            yield return new WaitForSeconds(0.1f);
           
            BattleSystem.Instance.canvas.gameObject.SetActive(true);
            Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
            var tutorialIcon = Instantiate(GetComponent<Rat>().TutorialIcon4, Director.Instance.canvas.transform);
            tutorialIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-5000, 0, 0f);
            tutorialIcon.GetComponent<MoveableObject>().Move(true);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0.5f);
            Director.Instance.blackScreen.gameObject.SetActive(true);
        }
    }
}
