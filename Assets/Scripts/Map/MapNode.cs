using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MapNode : MonoBehaviour
{
    public string NodeName;
    bool IsHighlighted = true;
    private Vector3 newScaleSize = new Vector3(3, 3, 3);
    private Vector3 oldScaleSize;
    private IEnumerator scaler;
    public bool NodeIsCompleted = false;
    public bool IsStartingNode;
    public bool IsEnabled;


    private void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        oldScaleSize = transform.localScale;
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
    }

    public void DisableNode()
    {
        var button = this.GetComponent<Button>();
        MapController.Instance.StartingPosition = this.transform.position;
        button.interactable = false;
        this.gameObject.transform.localScale = oldScaleSize;
        print(gameObject.transform.localScale);
        if(scaler!= null)
        {
            StopCoroutine(scaler);
        }
        NodeIsCompleted = true;
    }

    public void ToggleHighlight()
    {

        if (IsHighlighted && GetComponent<Button>().interactable)
        {
            gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
            gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
            if (scaler != null)
            {
                StopCoroutine(scaler);
            }

            scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), newScaleSize, 0.01f);
            StartCoroutine(scaler);
            int i = 0;
            foreach (var MM in MapController.Instance.mapCanvas.GetComponentsInChildren<MiniMapIcon>())
            {
                StartCoroutine(MM.Move(this.transform.position.x - i * 2, transform.position.y + 2f, transform.position.z));
                i++;
            }
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
}
