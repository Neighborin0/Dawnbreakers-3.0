using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class ItemDisplayBackButton : MonoBehaviour
{
    public void GoBack()
    {
        foreach (var child in GameObject.FindObjectsOfType<ItemDisplay>())
        {
            child.Move(false);
            foreach (var CT in GameObject.FindObjectsOfType<CharacterTab>())
            {
                CT.OnInteracted -= child.GrantItem;
            }
        }
        Director.Instance.DisableCharacterTab();
        Director.Instance.CharacterSlotEnable(true);
        Tools.ToggleUiBlocker(false);
        this.gameObject.SetActive(false);
    }
}
