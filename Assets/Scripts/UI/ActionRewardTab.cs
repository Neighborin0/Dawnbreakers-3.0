using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Action;

public class ActionRewardTab : MonoBehaviour
{
    public Image unitPortrait;
    public TextMeshProUGUI actionDesc;
    public TextMeshProUGUI actionName;
    public Action currentAction;
    public TextMeshProUGUI actionCostText;
    public TextMeshProUGUI actionTypeText;
    public TextMeshProUGUI actionDurationText;
    public TextMeshProUGUI actionDamageTypeText;
    public TextMeshProUGUI actionTargetTypeText;

    public bool Chosen = false;

    /*
     * The exact runtime Action clone added to the unit.
     * This is different from currentAction, which is the reward-display clone.
     */
    public Action grantedAction;

    private bool transitioning = false;

    public void Initalize(Action action)
    {
        if (action == null)
        {
            Debug.LogError(
                "ActionRewardTab received a null action."
            );

            return;
        }

        if (action.unit == null)
        {
            Debug.LogError(
                $"The reward action '{action.ActionName}' " +
                "does not have a unit assigned."
            );

            return;
        }

        currentAction = action;
        grantedAction = null;
        Chosen = false;

        if (currentAction.unit.charPortraits != null &&
            currentAction.unit.charPortraits.Count > 0)
        {
            unitPortrait.sprite =
                currentAction.unit.charPortraits[0];
        }

        actionName.text =
            currentAction.ActionName;

        actionDesc.text =
            currentAction.GetDescription();

        actionCostText.text =
            CombatTools.DetermineTrueCost(
                currentAction
            ).ToString();

        actionTypeText.text =
            currentAction.actionType.ToString();

        actionTargetTypeText.text =
            currentAction.targetType.ToString();

        actionDurationText.gameObject.SetActive(false);
        actionDamageTypeText.gameObject.SetActive(false);

        if (currentAction.actionType == ActionType.STATUS)
        {
            actionDurationText.gameObject.SetActive(true);

            actionDurationText.text =
                currentAction.duration.ToString();
        }
        else if (currentAction.actionType == ActionType.ATTACK)
        {
            actionDamageTypeText.gameObject.SetActive(true);

            actionDamageTypeText.text =
                currentAction.damageType.ToString();
        }
    }

    public void OnInteracted()
    {
        if (transitioning ||
            Chosen ||
            currentAction == null)
        {
            return;
        }

        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        transitioning = true;

        if (currentAction == null ||
            currentAction.unit == null)
        {
            Debug.LogError(
                "The selected reward does not have a valid action or unit."
            );

            transitioning = false;
            yield break;
        }

        ActionRewardManager rewardManager =
            Director.Instance.actionRewardManager;

        if (rewardManager == null)
        {
            Debug.LogError(
                "ActionRewardManager is missing."
            );

            transitioning = false;
            yield break;
        }

        /*
         * Move all reward cards off-screen.
         */
        rewardManager.MoveRewards(false);

        yield return new WaitForSeconds(1.6f);

        bool alreadyKnown =
            currentAction.unit.actionList.Any(
                action =>
                    action != null &&
                    action.ActionName ==
                    currentAction.ActionName
            );

        if (alreadyKnown)
        {
            Debug.LogWarning(
                $"{currentAction.unit.unitName} already knows " +
                $"{currentAction.ActionName}."
            );

            rewardManager.MoveRewards(true);

            transitioning = false;
            yield break;
        }

        if (currentAction.unit.actionList.Count >= 4)
        {
            Debug.Log(
                $"{currentAction.unit.unitName} already has four actions."
            );

            /*
             * Until replacement is implemented, return to the rewards.
             */
            rewardManager.MoveRewards(true);

            transitioning = false;
            yield break;
        }

        /*
         * Record the list count before calling the existing void method.
         */
        int previousActionCount =
            currentAction.unit.actionList.Count;

        Tools.AddNewActionToUnit(
            currentAction.unit,
            currentAction.ActionName,
            true
        );

        /*
         * Confirm that the method actually added an action.
         */
        if (currentAction.unit.actionList.Count <=
            previousActionCount)
        {
            Debug.LogError(
                $"Failed to add '{currentAction.ActionName}' " +
                $"to {currentAction.unit.unitName}."
            );

            rewardManager.MoveRewards(true);

            transitioning = false;
            yield break;
        }

        /*
         * AddNewActionToUnit appends the new runtime clone.
         * Capture that exact object so Back can remove it safely.
         */
        grantedAction =
            currentAction.unit.actionList[
                currentAction.unit.actionList.Count - 1
            ];

        if (grantedAction == null ||
            grantedAction.ActionName != currentAction.ActionName)
        {
            Debug.LogError(
                $"The newly added action could not be identified for " +
                $"{currentAction.unit.unitName}."
            );

            grantedAction = null;

            rewardManager.MoveRewards(true);

            transitioning = false;
            yield break;
        }

        Chosen = true;

        Debug.Log(
            $"{currentAction.unit.unitName} has learned " +
            $"{currentAction.ActionName}."
        );

        Director.Instance.DisplayCharacterTab(
            false,
            false
        );

        SetConfirmButton(true);

        Director.Instance.backButton.gameObject.SetActive(true);

        MoveableObject backButtonMovement =
            Director.Instance.backButton
                .GetComponent<MoveableObject>();

        if (backButtonMovement != null)
        {
            backButtonMovement.Move(true);
        }

        MoveableObject levelUpTextMovement =
            Director.Instance.LevelUpText
                .GetComponent<MoveableObject>();

        if (levelUpTextMovement != null)
        {
            levelUpTextMovement.Move(true);
        }

        transitioning = false;
    }

    private void SetConfirmButton(bool SetActive)
    {
        if (Director.Instance.ConfirmButton == null)
            return;

        MoveableObject confirmMovement =
            Director.Instance.ConfirmButton
                .GetComponent<MoveableObject>();

        Button confirmButton =
            Director.Instance.ConfirmButton
                .GetComponent<Button>();

        Image confirmImage =
            Director.Instance.ConfirmButton
                .GetComponent<Image>();

        if (SetActive)
        {
            Director.Instance.ConfirmButton.gameObject.SetActive(true);

            if (confirmMovement != null)
            {
                confirmMovement.Move(true);
            }

            if (confirmButton != null)
            {
                confirmButton.interactable = true;
            }

            if (confirmImage != null)
            {
                confirmImage.material =
                    Instantiate(confirmImage.material);

                confirmImage.material.SetFloat(
                    "OutlineThickness",
                    1f
                );

                confirmImage.material.SetColor(
                    "OutlineColor",
                    Color.white
                );
            }
        }
        else
        {
            if (confirmMovement != null)
            {
                confirmMovement.Move(false);
            }

            if (confirmButton != null)
            {
                confirmButton.interactable = false;
            }

            if (confirmImage != null)
            {
                confirmImage.material =
                    Instantiate(confirmImage.material);

                confirmImage.material.SetFloat(
                    "OutlineThickness",
                    0f
                );

                confirmImage.material.SetColor(
                    "OutlineColor",
                    Color.white
                );
            }
        }
    }
}