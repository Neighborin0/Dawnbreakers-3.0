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
    public Vector3 newScaleSize = new Vector3(3, 3, 3);
    public Vector3 oldScaleSize;
    private IEnumerator scaler;
    public bool NodeIsCompleted = false;
    public bool IsStartingNode;
    public bool IsEnabled;
    public Light maplight;
    public GameObject mapline;


    private void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
        maplight = GetComponentInChildren<Light>();
    }

    public void DisableNode()
    {
        StartCoroutine(StartLoadingNode());
    }

    public IEnumerator StartLoadingNode()
    {
        var button = this.GetComponent<Button>();
        int i = 0;
        LabCamera.Instance.followDisplacement = new Vector3(0, MapController.Instance.MinZoom, -MapController.Instance.MinZoom * 3.4f);
        yield return new WaitForSeconds(0.3f);
        foreach (var MM in MapController.Instance.mapCanvas.GetComponentsInChildren<MiniMapIcon>())
        {
            StartCoroutine(MM.Move(this.transform.position.x - i * 2, transform.position.y + 1f, transform.position.z));
            i++;
        }
        MapController.Instance.StartingPosition = this.transform.position;
        button.interactable = false;
        this.gameObject.transform.localScale = oldScaleSize;
        print(gameObject.transform.localScale);
        if (scaler != null)
        {
            StopCoroutine(scaler);
        }
        if(mapline != null) 
        {
            mapline.GetComponent<LineRenderer>().material = Instantiate<Material>(mapline.GetComponent<LineRenderer>().material);
            mapline.GetComponent<LineRenderer>().material.SetColor("_BaseColor", new Color(0, 0, 0, 0.5f));
        }
        NodeIsCompleted = true;
        maplight.gameObject.SetActive(false);
        yield return new WaitUntil(() => MapController.Instance.mapCanvas.GetComponentsInChildren<MiniMapIcon>()[0].state == MiniMapIcon.MapIconState.IDLE);
        yield return new WaitForSeconds(0.8f);
        this.OnInteracted();
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

    public virtual void OnInteracted() { }
}
