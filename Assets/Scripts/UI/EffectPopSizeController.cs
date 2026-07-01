using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class EffectPopSizeController : UIBehaviour, ILayoutSelfController
{
    
    public float maxWidth = 400f;
    public float maxHeight = 400f;

    private RectTransform rectTransform;

    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetLayoutHorizontal()
    {
        // Intercepts Unity's auto-layout sizing right after Content Size Fitter runs
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        Vector2 size = rectTransform.sizeDelta;
        if (size.x > maxWidth)
        {
            rectTransform.sizeDelta = new Vector2(maxWidth, size.y);
        }
    }

    public void SetLayoutVertical()
    {// Intercepts Unity's auto-layout sizing right after Content Size Fitter runs
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        Vector2 size = rectTransform.sizeDelta;
        if (size.y > maxWidth)
        {
            rectTransform.sizeDelta = new Vector2(size.x, maxHeight);
        }
    }
}