using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.UI.CanvasScaler;

public class ActionRewardManager : MonoBehaviour
{
    public List<Action> actionRewards = new List<Action>();
    public ActionRewardTab actionRewardTabPrefab;
    public List<ActionRewardTab> actionRewardTabDisplay = new List<ActionRewardTab>();
    private Coroutine moveRewardsCoroutine;
    public CharacterTab replacerTabPrefab;
    public Transform replacerTabParent;

    public void GetRewards()
    {
        replacerTabParent = Director.Instance.canvas.transform;
        Tools.ToggleUiBlocker(false, true);
        Director.Instance.CharacterSlotEnable(true);
        BattleLog.Instance.ClearAllBattleLogText();
        Director.Instance.LevelUpText.GetComponent<MoveableObject>().Move(false);

        ClearRewards();

        for (int i = 0; i < 3; i++)
        {
            ActionRewardTab tab = Instantiate(actionRewardTabPrefab, Director.Instance.canvas.transform);

            float xPosition = -550f + (i * 520f);

            RectTransform tabRect = tab.GetComponent<RectTransform>();

            tabRect.anchoredPosition = new Vector2(xPosition, -1000f);

            MoveableObject moveableObject = tab.GetComponent<MoveableObject>();

            if (moveableObject != null)
            {
                moveableObject.PositionDownX = xPosition;
                moveableObject.PositionUpX = xPosition;
            }

            actionRewardTabDisplay.Add(tab);
        }

        //Setting the inital position of the replacer tab to be off-screen, so it can slide into view later.
        CharacterTab replacerTab = Instantiate(Director.Instance.characterTab, Director.Instance.canvas.transform);
        float replacerXPosition = -30;
        RectTransform replacerTabRect = replacerTab.GetComponent<RectTransform>();
        replacerTabRect.anchoredPosition = new Vector2(replacerXPosition, -1000f);
        MoveableObject replacerMoveableObject = replacerTab.GetComponent<MoveableObject>();
        if (replacerMoveableObject != null)
        {
            replacerMoveableObject.PositionDownX = replacerXPosition;
            replacerMoveableObject.PositionUpX = replacerXPosition;
        }
        //


        List<Unit> validPartyUnits = Director.Instance.party.Where(unit => unit != null).ToList();

        if (validPartyUnits.Count == 0)
        {
            Debug.LogError("Action rewards could not be generated.");
            return;
        }

        /*
         * Generate three rewards.
         *
         * One party member:
         * Unit 1, Unit 1, Unit 1
         *
         * Two party members:
         * Unit 1, Unit 2, Unit 1
         *
         * Three party members:
         * Unit 1, Unit 2, Unit 3
         */
        for (int i = 0; i < 3; i++)
        {
            Action reward = null;
            for (int unitOffset = 0; unitOffset < validPartyUnits.Count; unitOffset++)
            {
                int unitIndex =  (i + unitOffset) % validPartyUnits.Count;

                Unit rewardUnit =  validPartyUnits[unitIndex];

                reward = GetRandomAction(rewardUnit);

                if (reward != null)
                {
                    break;
                }
            }

            if (reward != null)
            {
                actionRewards.Add(reward);

                Debug.Log($"Generated reward '{reward.ActionName}' " +$"for {reward.unit.unitName}.");
            }
        }

        DisplayRewards();
    }

private Action GetRandomAction(Unit unit)
    {
        if (unit == null || unit.ActionPool == null ||unit.ActionPool.Count == 0)
        {
            return null;
        }

        HashSet<string> selectedActionNames = actionRewards.Where(action =>action != null && action.unit == unit).Select(action =>action.ActionName).ToHashSet();

        HashSet<string> learnedActionNames =unit.actionList.Where(action => action != null).Select(action => action.ActionName).ToHashSet();

        List<Action> possibleActions = unit.ActionPool.Where(action => action != null && !selectedActionNames.Contains(action.ActionName) &&!learnedActionNames.Contains(action.ActionName)).ToList();

        if (possibleActions.Count == 0)
        {
            Debug.LogWarning($"No valid action rewards remain for {unit.unitName}.");
            return null;
        }

        List<Action> newActions = possibleActions.Where(action => action.New).ToList();
        List<Action> rewardPool = newActions.Count > 0 ? newActions : possibleActions;
        Action actionScriptable = rewardPool[UnityEngine.Random.Range(0,rewardPool.Count)];

        Action action = Instantiate(actionScriptable);
        action.unit = unit;
        action.GetDescription();

        return action;
    }



    private void DisplayRewards()
    {
        Director.Instance.LevelUpText.gameObject.SetActive(true);

        for (int i = 0;
             i < actionRewardTabDisplay.Count;
             i++)
        {
            ActionRewardTab tab = actionRewardTabDisplay[i];

            if (i < actionRewards.Count)
            {
                tab.gameObject.SetActive(true);
                tab.Initalize(actionRewards[i], this);

                Button button = tab.GetComponent<Button>();

                if (button != null)
                {
                    button.interactable = false;
                }

                HighlightedObject highlightedObject = tab.GetComponent<HighlightedObject>();

                if (highlightedObject != null)
                {
                    highlightedObject.disabled = true;
                }
            }
            else
            {
                tab.gameObject.SetActive(false);
            }
        }

        MoveRewards(true);
    }


    public void MoveRewards(bool MoveUp)
    {
        if (moveRewardsCoroutine != null)
        {
            StopCoroutine(moveRewardsCoroutine);
        }

        moveRewardsCoroutine = StartCoroutine(MoveRewardDisplaysTabs(MoveUp));
    }

    private IEnumerator MoveRewardDisplaysTabs(bool MoveUp)
    {
        foreach (ActionRewardTab tab in actionRewardTabDisplay)
        {
            if (tab == null ||!tab.gameObject.activeSelf)
            {
                continue;
            }

            Button button = tab.GetComponent<Button>();

            if (button != null)
            {
                button.interactable = false;
            }

            HighlightedObject highlightedObject = tab.GetComponent<HighlightedObject>();

            if (highlightedObject != null)
            {
                highlightedObject.disabled = true;
            }

            if (MoveUp)
            {
                tab.Chosen = false;
            }
        }

        foreach (ActionRewardTab tab in actionRewardTabDisplay)
        {
            if (tab == null ||
                !tab.gameObject.activeSelf)
            {
                continue;
            }

            MoveableObject moveableObject = tab.GetComponent<MoveableObject>();

            if (moveableObject != null)
            {
                moveableObject.Move(MoveUp);
            }

            yield return new WaitForSeconds(0.02f);
        }

        if (MoveUp)
        {
            foreach (ActionRewardTab tab in actionRewardTabDisplay)
            {
                if (tab == null ||
                    !tab.gameObject.activeSelf)
                {
                    continue;
                }

                Button button =
                    tab.GetComponent<Button>();

                if (button != null)
                {
                    button.interactable = true;
                }

                HighlightedObject highlightedObject =
                    tab.GetComponent<HighlightedObject>();

                if (highlightedObject != null)
                {
                    highlightedObject.disabled = false;
                }
            }
        }

        moveRewardsCoroutine = null;
    }



    public void ClearRewards()
    {
        if (moveRewardsCoroutine != null)
        {
            StopCoroutine(moveRewardsCoroutine);
            moveRewardsCoroutine = null;
        }

        foreach (ActionRewardTab tab in actionRewardTabDisplay)
        {
            if (tab != null)
            {
                Destroy(tab.gameObject);
            }
        }

        foreach (Action action in actionRewards)
        {
            if (action != null)
            {
                Destroy(action);
            }
        }

        actionRewardTabDisplay.Clear();
        actionRewards.Clear();
    }

}

