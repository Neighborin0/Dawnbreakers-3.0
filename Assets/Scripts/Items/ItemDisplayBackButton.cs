using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class ItemDisplayBackButton : MonoBehaviour
{
    public void GoBack()
    {
        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        Director.Instance.backButton.GetComponent<MoveableObject>().Move(false);
        Director.Instance.DisableCharacterTab();
        Director.Instance.CharacterSlotEnable(true);
        Tools.ToggleUiBlocker(false, false);
        yield return new WaitForSeconds(0.3f);
        foreach (var ID in GameObject.FindObjectsOfType<ItemDisplay>())
        {
            ID.GetComponent<MoveableObject>().Move(true);
            ID.GetComponent<Button>().interactable = true;
            ID.GetComponent<HighlightedObject>().disabled = false;
            foreach (var CT in GameObject.FindObjectsOfType<CharacterTab>())
            {
                CT.OnInteracted -= ID.GrantItem;             
            }
        }
        Director.Instance.chooseYourItemText.gameObject.SetActive(true);
        Tools.ToggleUiBlocker(false, true);

    }
}
