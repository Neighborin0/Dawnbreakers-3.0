
using System.Collections;
using UnityEngine;
using TMPro;

public class LabPopup : MonoBehaviour
{
    public IEnumerator scaler;
    public IEnumerator dropper;

    public IEnumerator Pop()
    {
        scaler = Tools.SmoothScaleObj(
            transform,
            new Vector3(2f, 2f, 0.1f),
            0.001f
        );

        StartCoroutine(scaler);

        yield return new WaitForSeconds(0.2f);

        if (scaler != null)
            StopCoroutine(scaler);

        scaler = Tools.SmoothScaleObj(
            transform,
            new Vector3(1f, 1f, 0.1f),
            0.001f
        );

        StartCoroutine(scaler);
    }

    public IEnumerator Rise(float delay = 0.001f)
    {
        transform.localScale = Vector3.one;

        dropper = Tools.SmoothMoveObject(
            transform,
            transform.position.x,
            transform.position.y + 1f,
            delay
        );

        yield return StartCoroutine(dropper);
    }

    public IEnumerator RiseAndDrop()
    {
        transform.localScale = Vector3.one;

        dropper = Tools.SmoothMoveObject(
            transform,
            transform.position.x,
            transform.position.y + 2f,
            0.001f
        );

        yield return StartCoroutine(dropper);

        yield return new WaitForSeconds(0.3f);

        dropper = Tools.SmoothMoveObject(
            transform,
            transform.position.x,
            transform.position.y - 2f,
            0.001f
        );

        yield return StartCoroutine(dropper);
    }

    public IEnumerator PlayPopup(
        float holdDuration = 1.2f,
        float riseDelay = 0.01f)
    {
        StartCoroutine(Rise(riseDelay));

        yield return new WaitForSeconds(holdDuration);

        yield return StartCoroutine(DestroyPopUp());
    }

    public IEnumerator DestroyPopUp(float delay = 0f)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        TextMeshProUGUI number =
            GetComponentInChildren<TextMeshProUGUI>();

        if (scaler != null)
        {
            StopCoroutine(scaler);
            scaler = null;
        }

        if (dropper != null)
        {
            StopCoroutine(dropper);
            dropper = null;
        }

        if (number != null)
        {
            while (number.color.a > 0f)
            {
                Color currentColor = number.color;

                currentColor.a =
                    Mathf.Max(
                        0f,
                        currentColor.a - 0.1f
                    );

                number.color = currentColor;

                yield return new WaitForSeconds(0.01f);
            }
        }

        Destroy(gameObject);
    }
}

