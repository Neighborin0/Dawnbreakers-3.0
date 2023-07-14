using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ItemDisplay : MonoBehaviour
{
    public Image displayImage;
    public TextMeshProUGUI itemDesc;
    public TextMeshProUGUI itemName;
    public Item item;
    public void TransitionIn()
    {
        displayImage.sprite = item.sprite;
        itemDesc.text = item.itemDescription;
        itemName.text = item.itemName;
    }
    public void OnInteracted()
    {
        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        Director.Instance.chooseYourItemText.gameObject.SetActive(false);
        foreach (var ID in GameObject.FindObjectsOfType<ItemDisplay>())
        {
            ID.GetComponent<MoveableObject>().Move(false, true);
            ID.GetComponent<Button>().interactable = false;
        }
        yield return new WaitForSeconds(0.5f);
        Director.Instance.DisplayCharacterTab(false, true);
        foreach (var CT in GameObject.FindObjectsOfType<CharacterTab>())
        {
            CT.OnInteracted += GrantItem;
            CT.GetComponent<HighlightedObject>().disabled = false;
            if (!CT.inventoryDisplay.gameObject.activeSelf)
                CT.SwitchDetailedStates();
        }
        Director.Instance.CharacterSlotEnable(true);
        Director.Instance.backButton.GetComponent<MoveableObject>().Move(true);
        Director.Instance.backButton.gameObject.SetActive(true);
    }


    public void GrantItem(CharacterTab characterTab)
    {
        Tools.GiveItem(item.itemName, characterTab.unit);
        foreach (var CS in Director.Instance.characterSlotpos.transform.GetComponentsInChildren<CharacterSlot>())
        {
            CS.ResetStats();
        }
        Director.Instance.backButton.gameObject.SetActive(false);
        GetComponent<MoveableObject>().Move(false, true);
        Director.Instance.DisableCharacterTab();
        Director.Instance.CharacterSlotEnable();
        Tools.ToggleUiBlocker(true, true);
        Tools.ToggleUiBlocker(true, false);
        MapController.Instance.ReEnteredMap += ReEntered;
        MapController.Instance.StartCoroutine(MapController.Instance.DoReEnteredMap(false));
        foreach (var ID in GameObject.FindObjectsOfType<ItemDisplay>())
        {
            Destroy(ID.gameObject);
        }

    }

    public void ReEntered(MapController mc)
    {
        Director.Instance.CharacterSlotEnable();
        MapController.Instance.ReEnteredMap -= ReEntered;

    }

}
