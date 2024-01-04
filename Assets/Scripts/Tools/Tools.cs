using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static ActionTypeButton;
using static UnityEngine.UI.CanvasScaler;

public class Tools : MonoBehaviour
{
    public static UnityEngine.RaycastHit GetMousePos()
    {
        Ray ray = LabCamera.Instance.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        return hit;
    }

    public static void GiveItem(string itemName, Unit unit)
    {
        var item = Director.Instance.itemDatabase.Where(obj => obj.itemName == itemName).SingleOrDefault();
        /*if(!RunTracker.Instance.itemsCollected.Contains(item))
        {
            RunTracker.Instance.itemsCollected.Add(item);
        }
        */
        unit.inventory.Add(item);
        item.OnPickup(unit);
    }


    public static Vector3 GetMouseWorldPos()
    {
        var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000);
        return LabCamera.Instance.GetComponent<Camera>().ScreenToWorldPoint(mousePos);
    }

    public static Unit[] GetAllUnits()
    {
        Unit[] units = UnityEngine.Object.FindObjectsOfType<Unit>();
        return units;
    }

    public static void AddItemToInventory(Unit unit, string itemName)
    {
        Item item = Director.Instance.itemDatabase.Where(obj => obj.itemName == itemName).SingleOrDefault();
        unit.inventory.Add(item);
        item.OnPickup(unit);
        /* if (!RunTracker.Instance.itemsCollected.Contains(item))
         {
             RunTracker.Instance.itemsCollected.Add(item);
         }
        */

    }


    public static void SetImageColorAlphaToZero(Image image)
    {
        var tempColor = image.color;
        tempColor.a = 0;
        image.color = tempColor;
    }

    public static void SetTextColorAlphaToZero(TextMeshProUGUI text)
    {
        var tempColor = text.color;
        tempColor.a = 0;
        text.color = tempColor;
    }


    public static void FadeUI(Image image, bool fadeOut)
    {
        for (int i = 0; i < 100; i++)
        {
            if (image != null)
            {
                if (fadeOut)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - i);
                }
                else
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + i);
                }
            }
            else
                break;
        }
    }

    public static IEnumerator FadeText(TextMeshProUGUI gameObject, float delay, bool FadeIn, bool SetInactiveAtEnd = true)
    {
        if (!gameObject.gameObject.activeSelf)
        {
            gameObject.gameObject.SetActive(true);
        }
        if (gameObject != null && !FadeIn)
        {
            while (gameObject.color.a > 0 && gameObject != null)
            {
                gameObject.color = new Color(gameObject.color.r, gameObject.color.g, gameObject.color.b, gameObject.color.a - 0.01f);
                yield return new WaitForSeconds(delay);
            }
            if (SetInactiveAtEnd)
            {
                gameObject.gameObject.SetActive(false);
            }
        }
        else if (gameObject != null)
        {
            while (gameObject.color.a <= 1 && gameObject != null)
            {
                gameObject.color = new Color(gameObject.color.r, gameObject.color.g, gameObject.color.b, gameObject.color.a + 0.01f);
                yield return new WaitForSeconds(delay);
            }
        }

    }

    public static IEnumerator FadeObject(Image gameObject, float delay, bool FadeIn, bool SetInactiveAtEnd = true, bool DestroyAtEnd = false)
    {
        if (gameObject != null && !FadeIn)
        {
            while (gameObject.color.a > 0 && gameObject != null)
            {
                gameObject.color = new Color(gameObject.color.r, gameObject.color.g, gameObject.color.b, gameObject.color.a - 0.01f);
                yield return new WaitForSeconds(delay);
            }
            if (SetInactiveAtEnd)
            {
                gameObject.gameObject.SetActive(false);
            }
            if (DestroyAtEnd)
            {
                Destroy(gameObject.gameObject);
            }
        }
        else if (gameObject != null)
        {
            while (gameObject.color.a < 1)
            {
                gameObject.color = new Color(gameObject.color.r, gameObject.color.g, gameObject.color.b, gameObject.color.a + 0.01f);
                yield return new WaitForSeconds(delay);
            }
        }

    }

    public static Material ReturnMaterialCopy(GameObject obj)
    {
        Material matToReturn = null;

        if (obj.GetComponent<Renderer>() != null)
        {
            obj.GetComponent<Renderer>().material = Instantiate<Material>(obj.GetComponent<Renderer>().material);
            matToReturn = obj.GetComponent<Renderer>().material;
        }
        
        else if(obj.GetComponent<SpriteRenderer>() != null)
        {
            obj.GetComponent<SpriteRenderer>().material = matToReturn;
            matToReturn = obj.GetComponent<SpriteRenderer>().material;
        }
       
        else if(obj.GetComponent<Image>() != null)
        {
            obj.GetComponent<Image>().material = matToReturn;
            matToReturn = obj.GetComponent<Image>().material;
        }
     

        return matToReturn;
    }

    public static void ClearAllEffectPopup()
    {
        foreach (EffectPopUp EP in FindObjectsOfType<EffectPopUp>())
        {
            Destroy(EP.gameObject);
        }

    }

    public static void ClearAllCharacterSlots()
    {
        foreach (var slot in FindObjectsOfType<CharacterSlot>())
        {
            Destroy(slot.gameObject);
        }
    }

    public static IEnumerator SmoothMove(GameObject obj, float delay, int AmountofTimesToMove, float x = 0, float y = 0, float z = 0, bool destroy = false)
    {
        for (int i = 0; i < AmountofTimesToMove; i++)
        {
            if (obj != null)
            {
                obj.transform.position = new Vector3(obj.gameObject.transform.position.x + x, obj.gameObject.transform.position.y + y, obj.gameObject.transform.position.z + z);
                yield return new WaitForSeconds(delay);
            }
        }
        if (destroy)
        {
            Destroy(obj);
        }
    }

    public static void ToggleUiBlocker(bool disable, bool DirectorBlocker = false, bool CantBeSeen = false)
    {
        var img = OptionsManager.Instance.blackScreen;
        if (DirectorBlocker)
        {
            img.gameObject.SetActive(false);
            img = Director.Instance.blackScreen;
            Director.Instance.blackScreen.transform.SetAsFirstSibling();
        }


        if (disable)
        {
            img.gameObject.SetActive(false);
            img.color = new Color(0, 0, 0, 0);
            img.raycastTarget = false;
        }
        else
        {
            img.gameObject.SetActive(true);
            img.color = new Color(0, 0, 0, 0.5f);
            img.raycastTarget = true;
        }


        if (CantBeSeen)
        {
            img.color = new Color(0, 0, 0, 0);
        }


    }

    public static bool CheckUiBlockers()
    {
        bool exists = false;
        if (OptionsManager.Instance.blackScreen.gameObject.activeSelf || Director.Instance.blackScreen.gameObject.activeSelf)
            exists = true;
        return exists;
    }



    public static IEnumerator SmoothMoveObject(Transform obj, float transformPointX, float transformPointY, float delay, bool destroy = false, float transformPointZ = 0, float fallbackTicks = 10000, float TimeDivider = 1)
    {
        float SmoothTime = 0f;
        int i = 0;
        if (transformPointZ == 0)
        {
            transformPointZ = obj.position.z;
        }
        while (new Vector3((obj.position.x), (obj.position.y), (obj.position.z)) != new Vector3((transformPointX), transformPointY, transformPointZ) || i != fallbackTicks)
        {
            obj.position = Vector3.Lerp(obj.position, new Vector3(transformPointX, transformPointY, transformPointZ), SmoothTime);
            SmoothTime += Time.deltaTime / TimeDivider;
            i++;
            yield return new WaitForSeconds(delay);
        }
        if (destroy)
        {
            DestroyImmediate(obj.gameObject);
        }
        yield break;

    }

    public static IEnumerator SmoothMoveObjectVertically(Transform transform, float distanceToMove, float moveSpeed)
    {
        float initialY = transform.position.y;
        float targetY = initialY + distanceToMove;
        float SmoothTime = 0f;

        while (transform.position.y < targetY)
        {
            float newY = transform.position.y + moveSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), SmoothTime);
            SmoothTime += Time.deltaTime * moveSpeed;
            yield return null; // Wait for the next frame
        }

        // Ensure the object reaches the exact target position
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
    }
    public static IEnumerator SmoothMoveUI(RectTransform obj, float transformPointX, float transformPointY, float delay)
    {
        float SmoothTime = 0f;
        while (new Vector2((int)(obj.anchoredPosition.x), (int)(obj.anchoredPosition.y)) != new Vector2(transformPointX, transformPointY))
        {
            if (obj != null)
            {
                obj.anchoredPosition = Vector3.Lerp(obj.anchoredPosition, new Vector2(transformPointX, transformPointY), SmoothTime);
                SmoothTime += Time.deltaTime;
                yield return new WaitForSeconds(delay);
            }
            else
                yield break;
        }
        yield break;

    }

    public static IEnumerator SmoothScale(RectTransform obj, Vector3 scaleToGoTo, float delay)
    {
        float SmoothTime = 0f;
        while (obj.localScale != scaleToGoTo)
        {
            if (obj != null)
            {
                obj.localScale = Vector3.Lerp(obj.localScale, scaleToGoTo, SmoothTime);
                SmoothTime += Time.deltaTime;
                yield return new WaitForSeconds(delay);
            }
            else
                yield break;
        }
        yield break;

    }

    public static IEnumerator SmoothScaleObj(Transform obj, Vector3 scaleToGoTo, float delay)
    {
        float SmoothTime = 0f;
        while (obj.localScale != scaleToGoTo)
        {
            if (obj != null)
            {
                obj.localScale = Vector3.Lerp(obj.localScale, scaleToGoTo, SmoothTime);
                SmoothTime += Time.deltaTime;
                yield return new WaitForSeconds(delay);
            }
            else
                yield break;
        }
        yield break;

    }

    public static IEnumerator SmoothMoveLine(LineRenderer line, Vector3 posToGoTo, float delay)
    {
        float SmoothTime = 0f;
        while (new Vector2((int)(line.GetPosition(1).x), (int)(line.GetPosition(1).y)) != new Vector2(posToGoTo.x, posToGoTo.y))
        {
            if (line != null)
            {
                line.SetPosition(1, Vector3.Lerp(line.GetPosition(1), posToGoTo, SmoothTime));
                SmoothTime += Time.deltaTime;
                yield return new WaitForSeconds(delay);
            }
            else
                yield break;
        }
        yield break;

    }


    public static Vector3 GetGameObjectPositionAsVector3(GameObject obj)
    {
        return new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
    }
   
    public static void AddNewActionToUnit(Unit unit, string actionName, bool ShownAsNew = true)
    {
        var oldAction = Director.Instance.actionDatabase.Where(obj => obj.ActionName == actionName).SingleOrDefault();
        unit.actionList.Add(oldAction);
        if (ShownAsNew)
            unit.actionList[unit.actionList.Count - 1].New = true;
    }

    public static IEnumerator StopTime(float duration)
    {
        var previousTime = Time.timeScale;
        Time.timeScale = 0f;
        Director.print(Time.timeScale);
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = previousTime;
        Director.print(Time.timeScale);
    }


    public static IEnumerator ChangeObjectEmissionToMaxIntensity(GameObject gameObjectToChange, Color newColor, float delay)
    {
        var sprite = gameObjectToChange.GetComponent<SpriteRenderer>();
        float intensity = 1;
        while (intensity < 10)
        {
            sprite.material.SetColor("_CharacterEmission", newColor * intensity);
            intensity++;
            print(intensity);
            yield return new WaitForSeconds(delay);
        }
    }

    public static IEnumerator ChangeObjectEmissionToMinIntensity(GameObject gameObjectToChange, float delay)
    {
        var sprite = gameObjectToChange.GetComponent<SpriteRenderer>();
        float alpha = 1;
        float subtract = 0.01f;
        if (gameObjectToChange != null)
        {
                while (sprite.material.GetColor("_CharacterEmission").r > 0 && sprite != null && sprite.material.GetColor("_CharacterEmission").g > 0 && sprite.material.GetColor("_CharacterEmission").b > 0)
                {
                    Debug.LogError("Is this even running?");
                    sprite.material.SetColor("_CharacterEmission", new Color(sprite.color.r - subtract, sprite.color.g - subtract, sprite.color.b - subtract));
                    yield return new WaitForSeconds(delay);
                }
        }
    }
    public IEnumerator ChangeLightIntensity(Light light, float desiredIntensity, float amountToRaiseBy, float delay = 0)
    {
        if (desiredIntensity != 0)
        {
            while (Math.Ceiling(light.intensity) <= Math.Ceiling(desiredIntensity))
            {
                light.intensity += amountToRaiseBy;
                yield return new WaitForSeconds(delay);
            }
        }
        else
        {
            while (Math.Ceiling(light.intensity) > Math.Ceiling(desiredIntensity))
            {
                light.intensity += amountToRaiseBy;
                yield return new WaitForSeconds(delay);
            }
        }

    }

    public static IEnumerator ChangeLightIntensityTimed(Light light, float desiredIntensity, float amountToRaiseBy, float delay = 0, float stagnantDelay = 0)
    {

        while (light.intensity < desiredIntensity)
        {
            light.intensity += amountToRaiseBy;
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(stagnantDelay);
        while (light.intensity > 0)
        {
            light.intensity -= amountToRaiseBy;
            yield return new WaitForSeconds(delay);
        }

    }

    public static IEnumerator ApplyLaunchToGameObject(GameObject spaceChimp, float launchForce, float delay)
    {
        spaceChimp.transform.position = new Vector3(spaceChimp.transform.position.x, spaceChimp.transform.position.y + 0.1f, spaceChimp.transform.position.z);
        var rigidbody = spaceChimp.GetComponent<Rigidbody2D>();
        rigidbody.AddForce(new Vector2(0f, launchForce));
        yield break;
    }


    public static void StartAndCheckCoroutine(IEnumerator originalIEnumerator, IEnumerator newIEnumerator)
    {
        if (originalIEnumerator != null)
        {
            Director.Instance.StopCoroutine(originalIEnumerator);
        }
        originalIEnumerator = newIEnumerator;
        Director.Instance.StartCoroutine(originalIEnumerator);
    }

    public static IEnumerator LateUnpause()
    {
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        yield break;
    }

    public static IEnumerator UpdateParentLayoutGroup(GameObject gameObject)
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    public static float RandomExcept(float min, float max, float exceptMin, float exceptMax)
    {
        float random = UnityEngine.Random.Range(min, max);
        while (random >= exceptMin && random <= exceptMax)
        {
            random = UnityEngine.Random.Range(min, max);
        }
        return random;
    }

    public static string ReturnDamageTypeSpriteName(DamageType damageType)
    {
        string stringToReturn = "";
       switch(damageType)
        {
            case DamageType.STRIKE:
                {
                    stringToReturn = "STRIKE2";
                }
            break;
                case DamageType.PIERCE:
                {
                    stringToReturn = "PIERCE2";
                }
           break;
            case DamageType.HEAT:
                {
                    stringToReturn = "HEAT2";
                }
                break;
                break;
            case DamageType.DARK:
                {
                    stringToReturn = "DARK2";
                }
                break;
            default:
                {
                    stringToReturn = damageType.ToString();
                }
                break;
        }     
        return stringToReturn;
    }


}
