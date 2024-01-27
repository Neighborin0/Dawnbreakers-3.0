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
    [NonSerialized]
    bool IsHighlighted = true;
    public Vector3 newScaleSize = new Vector3(3, 3, 3);
    public Vector3 oldScaleSize;
    [NonSerialized]
    private IEnumerator scaler;
    [NonSerialized]
    public bool NodeIsCompleted = false;
    [NonSerialized]
    public bool IsStartingNode;
    [NonSerialized]
    public bool IsEnabled;
    [NonSerialized]
    public GameObject mapline;
    [NonSerialized]
    public bool disabled = false;


    private void Start()
    {
        GetComponent<Image>().material = Instantiate<Material>(GetComponent<Image>().material);
        gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
    }

    public void DisableNode(bool ApplyDelays = true)
    {
        StartCoroutine(StartLoadingNode(ApplyDelays));
        if(SceneManager.GetActiveScene().name == "MAP2")
            Tools.ToggleUiBlocker(false, true, true);
    }

    public IEnumerator StartLoadingNode(bool ApplyDelays = true)
    {
        var button = this.GetComponent<Button>();
        button.interactable = false;
        int i = 0;
        disabled = true;
        if(LabCamera.Instance != null)
            LabCamera.Instance.followDisplacement = new Vector3(0, MapController.Instance.MinZoom, -MapController.Instance.MinZoom * 3.4f);
        if(ApplyDelays)
            yield return new WaitForSeconds(0.3f);


        foreach (var MM in FindObjectsOfType<MiniMapIcon>())
        {
            StartCoroutine(MM.Move((this.transform.position.x + 0.7f) + (i * 1.3f), transform.position.y + 1f - (i * 0.4f), transform.position.z - 1.5f));
            i++;
        }
        AudioManager.QuickPlay("ui_woosh_002");

        MapController.Instance.StartingPosition = this.transform.position;
        if (MapController.Instance.mapControlBar.activeSelf)
            MapController.Instance.mapControlBar.GetComponent<MoveableObject>().Move(false);
        this.gameObject.transform.localScale = oldScaleSize;
        print(gameObject.transform.localScale);
        if (scaler != null)
        {
            StopCoroutine(scaler);
            scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), oldScaleSize, 0.01f);
            StartCoroutine(scaler);
        }
        if(mapline != null) 
        {
            mapline.GetComponent<LineRenderer>().material = Instantiate<Material>(mapline.GetComponent<LineRenderer>().material);
            mapline.GetComponent<LineRenderer>().material.SetColor("_BaseColor", new Color(0, 0, 0, 0.5f));
        }
        NodeIsCompleted = true;
        if (ApplyDelays)
        {
          yield return new WaitUntil(() => MapController.Instance.grid.GetComponentsInChildren<MiniMapIcon>()[0].state == MiniMapIcon.MapIconState.IDLE);
          yield return new WaitForSeconds(1f);
        }
        this.OnInteracted();
    }
    
    public void ToggleHighlight()
    {
        if(gameObject.activeSelf)
        {
            if (IsHighlighted && GetComponent<Button>().interactable && !disabled)
            {
                gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 1f);
                gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
                if (scaler != null)
                {
                    Director.Instance.StopCoroutine(scaler);
                }

                scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), newScaleSize, 0.01f);
                try
                {
                    if (gameObject.activeSelf)
                        Director.Instance.StartCoroutine(scaler);
                }
                catch(Exception e)
                {

                }
             
                IsHighlighted = false;
            }
            else
            {
                gameObject.GetComponent<Image>().material.SetFloat("OutlineThickness", 0f);
                gameObject.GetComponent<Image>().material.SetColor("OutlineColor", Color.white);
                if (scaler != null)
                {
                    Director.Instance.StopCoroutine(scaler);
                }
                scaler = Tools.SmoothScale(gameObject.GetComponent<RectTransform>(), oldScaleSize, 0.01f);
                Director.Instance.StartCoroutine(scaler);
                IsHighlighted = true;
            }
        }
    }

    public virtual void OnInteracted() { }
}
