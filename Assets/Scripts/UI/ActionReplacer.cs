using UnityEngine;
using UnityEngine.UI;

public class ActionReplacer : MonoBehaviour
{
    private ActionRewardTab rewardTab;
    private CharacterTab characterTab;

    private bool hasSelectedReplacement = false;

    public bool HasSelectedReplacement =>
        hasSelectedReplacement;

    public void Init(
        ActionRewardTab sourceRewardTab,
        CharacterTab sourceCharacterTab)
    {
        rewardTab = sourceRewardTab;
        characterTab = sourceCharacterTab;

        if (rewardTab == null ||
            characterTab == null ||
            characterTab.unit == null)
        {
            Debug.LogError(
                "ActionReplacer was initialized with invalid data.",
                this
            );

            return;
        }

        if (characterTab.actionDisplay == null)
        {
            Debug.LogError(
                "Replacement CharacterTab has no actionDisplay assigned.",
                characterTab
            );

            return;
        }

        ActionContainer[] actionContainers =
            characterTab.actionDisplay
                .GetComponentsInChildren<ActionContainer>(true);

        if (actionContainers.Length == 0)
        {
            Debug.LogError(
                "Replacement CharacterTab has no ActionContainers. " +
                "CharacterTab.Init() did not build the action list.",
                characterTab
            );

            return;
        }

        foreach (ActionContainer container in actionContainers)
        {
            if (container == null ||
                container.action == null ||
                container.button == null)
            {
                continue;
            }

            int slotIndex =
                characterTab.unit.actionList
                    .IndexOf(container.action);

            if (slotIndex < 0)
            {
                Debug.LogWarning(
                    $"Could not find action {container.action.ActionName} " +
                    $"inside {characterTab.unit.unitName}'s action list.",
                    container
                );

                continue;
            }

            int capturedIndex = slotIndex;

            container.button.onClick =
                new Button.ButtonClickedEvent();

            container.button.interactable = true;

            container.button.onClick.AddListener(
                () => SelectReplacement(capturedIndex)
            );
        }
    }

    private void SelectReplacement(int index)
    {
        if (hasSelectedReplacement)
            return;

        hasSelectedReplacement = true;

        rewardTab.ReplaceActionAtIndex(index);

        Destroy(this);
    }

    public void CancelReplacement()
    {
        if (hasSelectedReplacement)
            return;

        if (rewardTab != null)
        {
            rewardTab.pendingAction = null;

            if (rewardTab.activeReplacementTab != null)
            {
                MoveableObject movement =
                    rewardTab.activeReplacementTab
                        .GetComponent<MoveableObject>();

                if (movement != null)
                {
                    movement.Move(false);
                }

                Destroy(
                    rewardTab.activeReplacementTab.gameObject,
                    0.4f
                );

                rewardTab.activeReplacementTab = null;
            }

            if (rewardTab.rewardManager != null)
            {
                foreach (ActionRewardTab tab in
                         rewardTab.rewardManager.actionRewardTabDisplay)
                {
                    if (tab == null)
                        continue;

                    Button button =
                        tab.GetComponent<Button>();

                    if (button != null)
                    {
                        button.interactable = true;
                    }
                }

                rewardTab.rewardManager.MoveRewards(true);
            }
        }

        Destroy(this);
    }
}