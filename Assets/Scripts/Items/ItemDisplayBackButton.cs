using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplayBackButton : MonoBehaviour
{
    [SerializeField]
    private float backButtonMoveTime = 0.4f;

    private bool backButtonTransitioning = false;

    public void GoBack()
    {
        if (backButtonTransitioning)
            return;

        ActionReplacer activeReplacer =
            FindObjectsOfType<ActionReplacer>(true)
                .FirstOrDefault(
                    replacer =>
                        replacer != null &&
                        replacer.isActiveAndEnabled &&
                        !replacer.HasSelectedReplacement
                );

        if (activeReplacer != null)
        {
            StartCoroutine(ReplacementBack(activeReplacer));
            return;
        }

        ActionRewardManager rewardManager =
            Director.Instance.actionRewardManager;

        bool actionRewardScreenOpen =
            rewardManager != null &&
            rewardManager.actionRewardTabDisplay.Any(
                tab =>
                    tab != null &&
                    tab.gameObject.activeInHierarchy
            );

        if (actionRewardScreenOpen)
        {
            StartCoroutine(ActionRewardBack());
        }
        else
        {
            StartCoroutine(Transition());
        }
    }

    private void SetBackButtonInteractable(bool interactable)
    {
        Button backButton =
            GetComponent<Button>();

        if (backButton != null)
        {
            backButton.interactable = interactable;
        }
    }

    /*private void MoveBackButton(bool moveIn)
    {
        MoveableObject backButtonMovement =
            GetComponent<MoveableObject>();

        if (backButtonMovement != null)
        {
            backButtonMovement.Move(moveIn);
        }
    }
    */

    private IEnumerator ReplacementBack(
        ActionReplacer activeReplacer)
    {
        backButtonTransitioning = true;

        SetBackButtonInteractable(false);
        GetComponent<MoveableObject>().Move(false);
        if (activeReplacer != null)
        {
            activeReplacer.CancelReplacement();
        }

        yield return new WaitForSeconds(backButtonMoveTime);

        backButtonTransitioning = false;
    }

    //This coroutine handles the transition back to the item selection screen.
    private IEnumerator Transition()
    {
        backButtonTransitioning = true;

        SetBackButtonInteractable(false);
        GetComponent<MoveableObject>().Move(true);

        Director.Instance.DisableCharacterTab();
        Director.Instance.CharacterSlotEnable(true);

        Tools.ToggleUiBlocker(false, false);

        yield return new WaitForSeconds(0.3f);

        foreach (ItemDisplay ID in
                 FindObjectsOfType<ItemDisplay>())
        {
            MoveableObject itemMovement =
                ID.GetComponent<MoveableObject>();

            if (itemMovement != null)
            {
                itemMovement.Move(true);
            }

            Button itemButton =
                ID.GetComponent<Button>();

            if (itemButton != null)
            {
                itemButton.interactable = true;
            }

            HighlightedObject highlightedObject =
                ID.GetComponent<HighlightedObject>();

            if (highlightedObject != null)
            {
                highlightedObject.disabled = false;
            }

            foreach (CharacterTab CT in
                     FindObjectsOfType<CharacterTab>())
            {
                CT.OnInteracted -= ID.GrantItem;
            }
        }

        Director.Instance.chooseYourItemText
            .gameObject.SetActive(true);

        Tools.ToggleUiBlocker(false, true);

        /*
         * The back button is now offscreen/out, so it should stay
         * non-interactable until another system moves it back in.
         */
        backButtonTransitioning = false;
    }

    //This coroutine handles the transition back to the action reward screen.
    private IEnumerator ActionRewardBack()
    {
        backButtonTransitioning = true;

        SetBackButtonInteractable(false);
        GetComponent<MoveableObject>().Move(false);

        ActionRewardManager rewardManager =
            Director.Instance.actionRewardManager;

        if (rewardManager == null)
        {
            Debug.LogError(
                "Cannot return to action rewards: " +
                "ActionRewardManager is missing."
            );

            backButtonTransitioning = false;
            yield break;
        }

        Tools.ToggleUiBlocker(false, false, false);

        Button confirmButton =
            Director.Instance.ConfirmButton
                .GetComponent<Button>();

        if (confirmButton != null)
        {
            confirmButton.interactable = false;
        }

        MoveableObject confirmButtonMovement =
            Director.Instance.ConfirmButton
                .GetComponent<MoveableObject>();

        if (confirmButtonMovement != null)
        {
            confirmButtonMovement.Move(false);
        }
        else
        {
            Debug.LogError(
                "ConfirmButton is missing its MoveableObject."
            );
        }

        ConfirmButton confirmButtonScript =
            confirmButton != null
                ? confirmButton.GetComponent<ConfirmButton>()
                : null;

        if (confirmButtonScript != null)
        {
            confirmButtonScript.SetOutline(0, Color.white);
        }

        MoveableObject levelUpTextMovement =
            Director.Instance.LevelUpText
                .GetComponent<MoveableObject>();

        if (levelUpTextMovement != null)
        {
            levelUpTextMovement.Move(true);

            TextMeshProUGUI levelUpText =
                Director.Instance.LevelUpText
                    .GetComponentInChildren<TextMeshProUGUI>();

            if (levelUpText != null)
            {
                levelUpText.text = "Memories Return";
            }
        }

        foreach (ActionRewardTab ART in
                 rewardManager.actionRewardTabDisplay)
        {
            if (ART == null || !ART.Chosen)
                continue;

            /*
             * Replacement undo.
             */
            if (ART.replacedAction != null &&
                ART.grantedAction != null &&
                ART.replacementIndex >= 0)
            {
                Unit unit = ART.grantedAction.unit;

                if (unit == null)
                {
                    unit = ART.replacedAction.unit;
                }

                if (unit == null)
                {
                    Debug.LogWarning(
                        "Could not undo replacement because both actions " +
                        "had null units."
                    );

                    ART.grantedAction = null;
                    ART.replacedAction = null;
                    ART.pendingAction = null;
                    ART.replacementIndex = -1;
                    ART.Chosen = false;

                    continue;
                }

                bool restored = false;

                if (ART.replacementIndex < unit.actionList.Count &&
                    unit.actionList[ART.replacementIndex] ==
                        ART.grantedAction)
                {
                    Action grantedAction =
                        ART.grantedAction;

                    unit.actionList[ART.replacementIndex] =
                        ART.replacedAction;

                    ART.replacedAction.unit = unit;

                    Destroy(grantedAction);

                    restored = true;
                }
                else
                {
                    /*
                     * Fallback: remove the granted action wherever it ended up,
                     * then restore the old action to its original index.
                     */
                    unit.actionList.Remove(ART.grantedAction);

                    int insertIndex =
                        Mathf.Clamp(
                            ART.replacementIndex,
                            0,
                            unit.actionList.Count
                        );

                    if (!unit.actionList.Contains(ART.replacedAction))
                    {
                        unit.actionList.Insert(
                            insertIndex,
                            ART.replacedAction
                        );
                    }

                    ART.replacedAction.unit = unit;

                    Destroy(ART.grantedAction);

                    restored = true;
                }

                if (restored)
                {
                    Debug.Log(
                        $"{unit.unitName} restored " +
                        $"{ART.replacedAction.ActionName}."
                    );
                }

                ART.grantedAction = null;
                ART.replacedAction = null;
                ART.pendingAction = null;
                ART.replacementIndex = -1;
                ART.Chosen = false;

                if (ART.activeReplacementTab != null)
                {
                    Destroy(ART.activeReplacementTab.gameObject);
                    ART.activeReplacementTab = null;
                }
                GetComponent<MoveableObject>().Move(false);
                continue;

                
            }

            /*
             * Normal reward undo.
             */
            if (ART.grantedAction == null)
            {
                Debug.LogWarning(
                    $"{ART.name} was marked as chosen, but it has " +
                    "no grantedAction reference."
                );

                ART.Chosen = false;
                continue;
            }

            Unit grantedUnit =
                ART.grantedAction.unit;

            if (grantedUnit == null)
            {
                Debug.LogWarning(
                    $"The granted action '{ART.grantedAction.ActionName}' " +
                    "does not have an owning unit."
                );

                ART.grantedAction = null;
                ART.Chosen = false;
                GetComponent<MoveableObject>().Move(false);
                continue;
            }

            bool removed =
                grantedUnit.actionList.Remove(
                    ART.grantedAction
                );

            if (removed)
            {
                string removedActionName =
                    ART.grantedAction.ActionName;

                Destroy(ART.grantedAction);

                Debug.Log(
                    $"{grantedUnit.unitName} has forgotten " +
                    $"{removedActionName}."
                );
            }
            else
            {
                Debug.LogWarning(
                    "The runtime action " +
                    $"'{ART.grantedAction.ActionName}' was not found " +
                    $"in {grantedUnit.unitName}'s action list."
                );
            }

            ART.grantedAction = null;
            ART.Chosen = false;
        }

        Director.Instance.DisableCharacterTab(false);

        rewardManager.MoveRewards(true);

        GetComponent<MoveableObject>().Move(false);

        MoveableObject levelUpTextMove =
            Director.Instance.LevelUpText
                .GetComponent<MoveableObject>();

        if (levelUpTextMove != null)
        {
            levelUpTextMove.Move(false);
        }

        /*
         * The button is moving back into view, so keep it disabled
         * until the movement finishes.
         */
        yield return new WaitForSeconds(backButtonMoveTime);

        SetBackButtonInteractable(true);

        backButtonTransitioning = false;

        Tools.ToggleUiBlocker(false, true, true);
    }

   /* public void forceMoveDown()
    {
       this.GetComponent<MoveableObject>().Move(false);
    }
   */
}
