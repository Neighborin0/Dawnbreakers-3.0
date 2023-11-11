using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using static UnityEngine.UI.CanvasScaler;

public class RunTracker : MonoBehaviour
{
    //public static RunTracker Instance { get; private set; }
    public Image TrackerMenu;
    public Button QuitButton;
    public List<Unit> partyMembersCollected;
    public int TimeSpentOnRun;
    public Unit slayer;
    public List<Item> itemsCollected;
    public PortraitDisplayPrefab portraitDisplayprefab;
    public Sprite unknownSymbol;

    public GameObject partyMemberDisplay;
    public GameObject itemDisplay;
    public Image slayerPortrait;
    public TextMeshProUGUI slayerText;
    void Awake()
    {
        /*if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        */
    }

    public void DisplayStats()
    {
        Tools.ToggleUiBlocker(this.gameObject, true);
        TrackerMenu.GetComponent<MoveableObject>().Move(true);
        foreach(var unit in partyMembersCollected)
        {
            var PDP = Instantiate(portraitDisplayprefab, partyMemberDisplay.transform);
            PDP.portrait.sprite = unit.charPortraits[0];
        }
        foreach (var item in itemsCollected)
        {
            var x = Instantiate(BattleLog.Instance.itemImage);
            x.image.sprite = item.sprite;
            x.GetComponent<ItemText>().item = item;
            x.transform.SetParent(itemDisplay.transform);
            x.transform.localScale = new Vector3(1, 1, 1);
            x.GetComponent<Button>().interactable = false;
            x.GetComponent<HighlightedObject>().disabled = true;
        }
        if (slayer != null)
        {
            slayerPortrait.sprite = slayer.charPortraits[0];
            slayerText.text = $"Defeated By: {slayer.unitName}";
        }
        else
        {
            slayerPortrait.sprite = unknownSymbol;
            slayerText.text = "The Land...";
        }
       
    }


}
