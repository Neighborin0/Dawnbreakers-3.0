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
    public ActionRewardManager rewardManager;
    public Action replacedAction;
    public int replacementIndex = -1;
    public CharacterTab activeReplacementTab;
    public CharacterTab replacerTabPrefab;
    public Transform replacerTabParent;

    public bool Chosen = false;

    //The exact runtime Action clone added to the unit. This is different from currentAction, which is the reward-display clone.
    public Action grantedAction;
    //Action clone which is pending to be added to the unit. This is used for replacement.
    public Action pendingAction;

    private bool transitioning = false;
    public bool Skipped = false;

    public void Initalize(Action action, ActionRewardManager actionRewardManager)
    {
        if (action == null)
        {
            Debug.LogError("ActionRewardTab received a null action.");
            return;
        }

        if (action.unit == null)
        {
            Debug.LogError($"The reward action '{action.ActionName}' " + "does not have a unit assigned.");
            return;
        }

        currentAction = action;
        grantedAction = null;
        pendingAction = null;
        replacedAction = null;
        replacementIndex = -1;
        activeReplacementTab = null;
        Chosen = false;

        if (actionRewardManager == null)
        {
            Debug.LogError(
                "ActionRewardTab was initialized without an ActionRewardManager.",
                this
            );

            return;
        }

        if (rewardManager != null && rewardManager != actionRewardManager)
        {
            Debug.LogWarning( "ActionRewardTab is being initialized with a different " +
                "ActionRewardManager than it was previously assigned.",
                this
            );
        }

        rewardManager = actionRewardManager;

        Chosen = false;

        if (currentAction.unit.charPortraits != null && currentAction.unit.charPortraits.Count > 0)
        {
            unitPortrait.sprite = currentAction.unit.charPortraits[0];
        }

        actionName.text = currentAction.ActionName;

        actionDesc.text = currentAction.GetDescription();

        actionCostText.text = CombatTools.DetermineTrueCost(currentAction).ToString();

        actionTypeText.text = currentAction.actionType.ToString();

        actionTargetTypeText.text = currentAction.targetType.ToString();

        actionDurationText.gameObject.SetActive(false);
        actionDamageTypeText.gameObject.SetActive(false);

        if (currentAction.actionType == ActionType.STATUS)
        {
            actionDurationText.gameObject.SetActive(true);

            actionDurationText.text = currentAction.duration.ToString();
        }
        else if (currentAction.actionType == ActionType.ATTACK)
        {
            actionDamageTypeText.gameObject.SetActive(true);

            actionDamageTypeText.text = currentAction.damageType.ToString();
        }
    }

    public void OnInteracted()
    {
        if (transitioning || Chosen || currentAction == null)
        {
            return;
        }
        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        transitioning = true;

        if (currentAction == null ||currentAction.unit == null)
        {
            Debug.LogError("The selected reward does not have a valid action or unit.");

            transitioning = false;
            yield break;
        }

        ActionRewardManager rewardManager = Director.Instance.actionRewardManager;

        if (rewardManager == null)
        {
            Debug.LogError("ActionRewardManager is missing.");
            transitioning = false;
            yield break;
        }

        //Move all reward cards off-screen.
        rewardManager.MoveRewards(false);

        yield return new WaitForSeconds(0.6f);

        bool alreadyKnown = currentAction.unit.actionList.Any(action => action != null && action.ActionName == currentAction.ActionName);

        if (alreadyKnown)
        {
            Debug.LogWarning($"{currentAction.unit.unitName} already knows " + $"{currentAction.ActionName}.");
            rewardManager.MoveRewards(true);
            transitioning = false;
            yield break;
        }

        //Replacement UI
        if (currentAction.unit.actionList.Count >= 4)
        {
            Debug.Log(
                $"{currentAction.unit.unitName} already has four actions. " +
                "Opening action replacement UI."
            );

            transitioning = false;
            HandleReplacement();

            yield break;
        }

        //Record the list count before calling the existing void method.
        int previousActionCount = currentAction.unit.actionList.Count;

        Tools.AddNewActionToUnit(currentAction.unit, currentAction.ActionName, true);

        //Confirm that the method actually added an action.
        if (currentAction.unit.actionList.Count <= previousActionCount)
        {
            Debug.LogError($"Failed to add '{currentAction.ActionName}' " + $"to {currentAction.unit.unitName}.");
            rewardManager.MoveRewards(true);
            transitioning = false;
            yield break;
        }

        //AddNewActionToUnit appends the new runtime clone. Capture that exact object so Back can remove it safely.

        grantedAction = currentAction.unit.actionList[currentAction.unit.actionList.Count - 1];

        if (grantedAction == null ||grantedAction.ActionName != currentAction.ActionName)
        {
            Debug.LogError($"The newly added action could not be identified for " + $"{currentAction.unit.unitName}.");
            grantedAction = null;
            rewardManager.MoveRewards(true);
            transitioning = false;
            yield break;
        }

        Chosen = true;

        Debug.Log( $"{currentAction.unit.unitName} has learned " + $"{currentAction.ActionName}."
        );

        Director.Instance.DisplayCharacterTab(false,false);

        rewardManager.SetConfirmButton(true);

        Director.Instance.backButton.gameObject.SetActive(true);

        MoveableObject backButtonMovement = Director.Instance.backButton.GetComponent<MoveableObject>();

        //button moves in
        if (backButtonMovement != null)
        {
            backButtonMovement.Move(true, setInteractableWhenDone: false);
        }

        MoveableObject levelUpTextMovement = Director.Instance.LevelUpText.GetComponent<MoveableObject>();

        if (levelUpTextMovement != null)
        {
            levelUpTextMovement.Move(true);
        }

        transitioning = false;
    }

    private void HandleReplacement()
    {
        if (rewardManager == null)
        {
            rewardManager = Director.Instance.actionRewardManager;
        }

        if (rewardManager == null)
        {
            Debug.LogError(
                "Cannot start replacement: ActionRewardManager is missing.",
                this
            );

            return;
        }

        if (rewardManager.replacerTabPrefab == null)
        {
            Debug.LogError(
                "Cannot start replacement: replacerTabPrefab is missing.",
                rewardManager
            );

            return;
        }

        if (currentAction == null || currentAction.unit == null)
        {
            Debug.LogError(
                "Cannot start replacement: currentAction or unit is null.",
                this
            );

            return;
        }

        pendingAction = currentAction;

        /*
         * Keep reward cards offscreen and prevent further reward clicks.
         */
        rewardManager.MoveRewards(false);

        foreach (ActionRewardTab tab in rewardManager.actionRewardTabDisplay)
        {
            if (tab == null)
                continue;

            Button tabButton = tab.GetComponent<Button>();

            if (tabButton != null)
            {
                tabButton.interactable = false;
            }
        }

        /*
         * Show replacement prompt immediately.
         */
        if (Director.Instance.LevelUpText != null)
        {
            TextMeshProUGUI promptText =
                Director.Instance.LevelUpText
                    .GetComponentInChildren<TextMeshProUGUI>();

            if (promptText != null)
            {
                promptText.text = "Select an action to replace.";
            }

            MoveableObject promptMovement =
                Director.Instance.LevelUpText
                    .GetComponent<MoveableObject>();

          

            if (promptMovement != null)
            {
                promptMovement.Move(false);
            }
        }

        /*
         * Spawn replacement tab on canvas, not TabGrid.
         */

        Transform parent =
            rewardManager.replacerTabParent != null
                ? rewardManager.replacerTabParent
                : Director.Instance.canvas.transform;

        activeReplacementTab =
            Instantiate(
                rewardManager.replacerTabPrefab,
                parent,
                false
            );

        activeReplacementTab.gameObject.SetActive(true);

     

        RectTransform rect =
            activeReplacementTab.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -1000f);
        }



        activeReplacementTab.Init(
            currentAction.unit,
            false,
            true
        );

        activeReplacementTab.detailedDisplay.SetActive(true);
        activeReplacementTab.actionDisplay.gameObject.SetActive(true);
        activeReplacementTab.inventoryDisplay.gameObject.SetActive(false);

        if (activeReplacementTab.characterTransfer != null)
        {
            activeReplacementTab.characterTransfer.interactable = false;
        }

        MoveableObject replacementMovement =
            activeReplacementTab.GetComponent<MoveableObject>();

        if (replacementMovement != null)
        {
            replacementMovement.Move(true);
        }

        ActionReplacer replacer =
            activeReplacementTab.GetComponent<ActionReplacer>();

        if (replacer == null)
        {
            replacer =
                activeReplacementTab.gameObject
                    .AddComponent<ActionReplacer>();
        }

        replacer.Init(
            this,
            activeReplacementTab
        );

        Director.Instance.backButton.gameObject.SetActive(true);

        MoveableObject backButtonMovement =
            Director.Instance.backButton
                .GetComponent<MoveableObject>();

        if (backButtonMovement != null)
        {
            backButtonMovement.Move(true);
        }

        Tools.ToggleUiBlocker(false, true);
    }

    public void ReplaceActionAtIndex(int index)
    {
        if (currentAction == null || currentAction.unit == null)
        {
            Debug.LogError(
                "Cannot replace action: currentAction or unit is null.",
                this
            );

            return;
        }

        Unit unit = currentAction.unit;

        if (index < 0 || index >= unit.actionList.Count)
        {
            Debug.LogError(
                $"Replacement index {index} is invalid for {unit.unitName}.",
                this
            );

            return;
        }

        if (Chosen)
        {
            Debug.LogWarning(
                "This reward has already been chosen.",
                this
            );

            return;
        }

        Action sourceAction =
            Director.Instance.actionDatabase.FirstOrDefault(
                action =>
                    action != null &&
                    action.ActionName == currentAction.ActionName
            );

        if (sourceAction == null)
        {
            Debug.LogError(
                $"Could not find '{currentAction.ActionName}' in action database.",
                this
            );

            return;
        }

        Action newRuntimeAction =
            Instantiate(sourceAction);

        newRuntimeAction.unit = unit;
        newRuntimeAction.New = true;

        replacedAction = unit.actionList[index];
        replacementIndex = index;
        grantedAction = newRuntimeAction;
        pendingAction = null;

        unit.actionList[index] = newRuntimeAction;

        Chosen = true;

        Debug.Log(
            $"{unit.unitName} replaced {replacedAction.ActionName} " +
            $"with {newRuntimeAction.ActionName}."
        );

        if (activeReplacementTab != null)
        {
            MoveableObject replacementMovement =
                activeReplacementTab.GetComponent<MoveableObject>();

            if (replacementMovement != null)
            {
                replacementMovement.Move(false);
            }
            else
            {
                activeReplacementTab.gameObject.SetActive(false);
            }
        }

        /*
         * Now show the normal post-reward character screen.
         */
        Director.Instance.DisplayCharacterTab(false, false);

        rewardManager.SetConfirmButton(true);

        Director.Instance.backButton.gameObject.SetActive(true);

        MoveableObject backButtonMovement =
            Director.Instance.backButton
                .GetComponent<MoveableObject>();

        if (backButtonMovement != null)
        {
            backButtonMovement.Move(true);
        }
    }
    public void CancelReplacementSelection()
    {
        pendingAction = null;

        if (activeReplacementTab != null)
        {
            MoveableObject tabMovement =
                activeReplacementTab.GetComponent<MoveableObject>();

            if (tabMovement != null)
            {
                tabMovement.Move(false);
            }

            Destroy(activeReplacementTab.gameObject, 0.4f);
            activeReplacementTab = null;
        }

        if (rewardManager != null)
        {
            foreach (ActionRewardTab tab in rewardManager.actionRewardTabDisplay)
            {
                if (tab == null)
                    continue;

                Button tabButton =
                    tab.GetComponent<Button>();

                if (tabButton != null)
                {
                    tabButton.interactable = true;
                }
            }

            rewardManager.MoveRewards(true);
        }

        MoveableObject levelUpTextMovement =
            Director.Instance.LevelUpText
                .GetComponent<MoveableObject>();

        if (levelUpTextMovement != null)
        {
            levelUpTextMovement.Move(false);
        }

        MoveableObject backButtonMovement =
            Director.Instance.backButton
                .GetComponent<MoveableObject>();

        if (backButtonMovement != null)
        {
            backButtonMovement.Move(false);
        }
    }


   
}