using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;
using static UnityEditor.Progress;
using static UnityEngine.UI.CanvasScaler;


public class ActionRewardManager : MonoBehaviour
{
    private Action parentAction;
    private List<Action> actionRewards = new List<Action>();
    private void GetRewards()
    {

        if (Director.Instance.party.Count == 0)
        {
            print("No units in party, how tf did you get here?");
            return;
        }
        else if (Director.Instance.party.Count == 1)
        {
            GetRandomAction(Director.Instance.party[0]);
        }
        else if (Director.Instance.party.Count == 2)
        {
            foreach (var unit in Director.Instance.party)
            {

                if (unit != null && RewardTracker(unit) < 2)
                {
                    var action = GetRandomAction(unit);
                    if (action != null)
                    {
                        actionRewards.Add(action);
                    }
                }
            }

        }
        else //instances where there are 3 or more units in the party
        {

            foreach (var unit in Director.Instance.party)
            {
                if (unit != null && !actionRewards.Any(x => x.unit == unit))
                {
                    var action = GetRandomAction(unit);
                    if (action != null)
                    {
                        actionRewards.Add(action);
                    }
                }
            }
        }

    }
    private int RewardTracker(Unit unit)
    {
        int count = 0;
        foreach (var action in actionRewards)
        {
            if (action.unit == unit)
            {
                count++;
            }
        }
        return count;
    }

    private Action GetRandomAction(Unit unit)
    {
        Action action = unit.ActionPool[UnityEngine.Random.Range(0, unit.ActionPool.Count)];
        foreach (var actionScriptable in unit.ActionPool)
        {
            if (actionScriptable != null && actionScriptable.New)
            {
                action = actionScriptable;
                break;
            }
        }
        return action;
    }
}
