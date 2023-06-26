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
    public bool IsHighlighted = true;
    public Vector3 newScaleSize = new Vector3(1000, 1000, 1000);
    private Vector3 oldScaleSize;
    float oldYpos;
    private IEnumerator scaler;
    IEnumerator generalCoruntine;


    private void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        oldScaleSize = transform.localScale;
        oldYpos = Director.Instance.ItemTabGrid.GetComponent<RectTransform>().anchoredPosition.y;
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
    }
    public void ToggleHighlight()
    {
        if (IsHighlighted && GetComponent<Button>().interactable)
        {
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 3f);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
            if (scaler != null)
            {
                StopCoroutine(scaler);
            }
            scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), newScaleSize, 0.01f);
            StartCoroutine(scaler);
            IsHighlighted = false;
        }
        else
        {
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
            if (scaler != null)
            {
                StopCoroutine(scaler);
            }
            scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), oldScaleSize, 0.01f);
            StartCoroutine(scaler);
            IsHighlighted = true;
        }
    }
    public void TransitionIn()
    {
        displayImage.sprite = item.sprite;
        itemDesc.text = item.itemDescription;
        itemName.text = item.itemName;
    }
    public void OnInteracted()
    {
        foreach (var ID in GameObject.FindObjectsOfType<ItemDisplay>())
        {
            ID.Move(true);
        }
        Director.Instance.DisplayCharacterTab(false, true);
        foreach (var CT in GameObject.FindObjectsOfType<CharacterTab>())
        {
            CT.OnInteracted += GrantItem;
        }
        Director.Instance.CharacterSlotEnable(true);
        Director.Instance.backButton.gameObject.SetActive(true);
    }

    public void Move(bool moveUp)
    {
        if (moveUp)
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(Director.Instance.ItemTabGrid.GetComponent<RectTransform>(), 0, 949, 0.01f);
            StartCoroutine(generalCoruntine);
        }
        else
        {
            if (generalCoruntine != null)
                StopCoroutine(generalCoruntine);

            generalCoruntine = Tools.SmoothMoveUI(Director.Instance.ItemTabGrid.GetComponent<RectTransform>(), 0, oldYpos, 0.01f);
            StartCoroutine(generalCoruntine);
        }

    }

    public void GrantItem(CharacterTab characterTab)
    {
        Tools.GiveItem(item.itemName, characterTab.unit);
        foreach (var CS in Director.Instance.characterSlotpos.transform.GetComponentsInChildren<CharacterSlot>())
        {
            CS.stats.text = $":{CS.unit.attackStat}\n:{CS.unit.defenseStat}\n:{CS.unit.speedStat}";
            CS.healthNumbers.text = $"{CS.unit.currentHP} / {CS.unit.maxHP}";
        }
        Director.Instance.backButton.gameObject.SetActive(false);
        Move(false);
        Director.Instance.DisableCharacterTab();
        Tools.ToggleUiBlocker(true, true);
        MapController.Instance.StartCoroutine(MapController.Instance.DoReEnteredMap(false));
        foreach (var ID in Director.Instance.ItemTabGrid.transform.GetComponentsInChildren<ItemDisplay>())
        {
            Destroy(ID.gameObject);
        }

    }


}
