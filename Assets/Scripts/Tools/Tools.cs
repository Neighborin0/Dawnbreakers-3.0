using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
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
       
    }

    public static int GetNumberOfPlayerUnits()
    {
        Unit[] units = UnityEngine.Object.FindObjectsOfType<Unit>();
        int i = 0;
        foreach(var x in units)
        {
            if(x.IsPlayerControlled)
            {
                i++;
            }
        }
        return i;
    }

    public static int GetNumberOfEnemyUnits()
    {
        Unit[] units = UnityEngine.Object.FindObjectsOfType<Unit>();
        int i = 0;
        foreach (var x in units)
        {
            if (!x.IsPlayerControlled)
            {
                i++;
            }
        }
        return i;
    }

    public static void PauseAllStaminaTimers()
    {
        //print("ALL STAMINA TIMERS ARE PAUSED");
        foreach (var SB in FindObjectsOfType<StaminaBar>())
        {
            SB.Paused = true;
        }
        foreach (var EI in FindObjectsOfType<EffectIcon>())
        {
            EI.isPaused = true;
        }
    }

    public static void UnpauseAllStaminaTimers()
    {
        //print("ALL STAMINA TIMERS ARE UNPAUSED");
        foreach (var SB in FindObjectsOfType<StaminaBar>())
        {
            SB.Paused = false;
        }
        foreach (var EI in FindObjectsOfType<EffectIcon>())
        {
            EI.isPaused = false;
        }
    }

    public static void EndAllTempEffectTimers()
    {
        foreach (var act in FindObjectsOfType<EffectIcon>())
        {
            Destroy(act);
        }
    }
    public static void FadeUI(Image image, bool fadeOut)
    {
        for (int i = 0; i < 100; i++)
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
    }
    public static string CheckNames(Unit unit)
    {
        string nameToReturn = "";
        for (int i = 0; i <= BattleSystem.Instance.enemyUnits.Count - 1; i++)
        {
            if (BattleSystem.Instance.enemyUnits[i].unitName == unit.unitName)
            {
                nameToReturn =  $"{(i + 2)}";
            }
        }
        return nameToReturn;
    }

    public static void TurnOffCriticalUI(Unit unit)
    {
        if (unit.intentUI != null)
        {
            unit.intentUI.gameObject.SetActive(false);
        }
        Destroy(unit.namePlate.gameObject);
        foreach (var bar in GameObject.FindObjectsOfType<BattleBar>())
        {
            if (bar.unit == unit)
            {
                Destroy(bar.gameObject);
                break;
            }
        }
        foreach (TimeLineChild child in Director.Instance.timeline.children)
        {
            if (child.unit == unit)
            {
                Director.Instance.timeline.children.Remove(child);
                Destroy(child.gameObject);               
                break;
            }
        }
        if(unit.GetComponentInChildren<TargetLine>() != null)
        {
            var line = unit.GetComponentInChildren<TargetLine>();
            Destroy(line.gameObject);
        }
        Destroy(unit.health.gameObject);
       
    }

    public static List<Unit> DetermineAllies(Unit baseUnit)
    {
        var allies = new List<Unit>();
        if (baseUnit.IsPlayerControlled)
        {
            foreach (var x in BattleSystem.Instance.playerUnits)
            {
                allies.Add(x);
            }
        }
        else
        {
            foreach (var x in BattleSystem.Instance.enemyUnits)
            {
                allies.Add(x);
            }
        }
        return allies;
    }
    public static List<Unit> DetermineEnemies(Unit baseUnit)
    {
        var enemies = new List<Unit>();
        if (baseUnit.IsPlayerControlled)
        {
            foreach (var x in BattleSystem.Instance.enemyUnits)
            {
                enemies.Add(x);
            }
        }
        else
        {
            foreach (var x in BattleSystem.Instance.playerUnits)
            {
                enemies.Add(x);
            }
        }
        return enemies;
    }
    public static void DetermineActionData(Unit baseUnit, int HowIsThisDetermined, int TargetNum)
    {
        var battlesystem = BattleSystem.Instance;
        baseUnit.actionList[HowIsThisDetermined].unit = baseUnit;
        switch (baseUnit.actionList[HowIsThisDetermined].targetType)
        {
            case Action.TargetType.ANY:
                baseUnit.actionList[HowIsThisDetermined].targets = battlesystem.numOfUnits[TargetNum];
                break;
            case Action.TargetType.SELF:
                baseUnit.actionList[HowIsThisDetermined].targets = baseUnit;
                break;
            case Action.TargetType.ALL_ENEMIES:
                baseUnit.actionList[HowIsThisDetermined].targets = baseUnit;
                break;
            case Action.TargetType.ALLIES:
                baseUnit.actionList[HowIsThisDetermined].targets = baseUnit;
                break;
        }
    }
    public static IEnumerator RepeatBehavior(Unit unit)
    {
        unit.StartCoroutine(unit.behavior.DoBehavior(unit));
        yield break;
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

    public static void ToggleUiBlocker(bool disable)
    {
        if(disable)
        {
            Director.Instance.blackScreen.gameObject.SetActive(false);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0);
            Director.Instance.blackScreen.raycastTarget = false;
        }
        else
        {
            Director.Instance.blackScreen.gameObject.SetActive(true);
            Director.Instance.blackScreen.color = new Color(0, 0, 0, 0.5f);
            Director.Instance.blackScreen.raycastTarget = true;
        }
     
    }

   

     public static IEnumerator SmoothMoveObject(Transform obj, float transformPointX, float transformPointY, float delay, bool destroy = false, float transformPointZ = 0, float fallbackTicks = 10000)
    {
        float SmoothTime = 0f;
        int i = 0;
        if(transformPointZ == 0)
        {
            transformPointZ = obj.position.z;
        }
        while (new Vector3((obj.position.x), (obj.position.y), (obj.position.z)) != new Vector3((transformPointX), transformPointY, transformPointZ) || i != fallbackTicks)
        {
            obj.position = Vector3.Lerp(obj.position, new Vector3(transformPointX, transformPointY, transformPointZ), SmoothTime);
            SmoothTime += Time.deltaTime;
            i++;
            yield return new WaitForSeconds(delay);
        }
        if(destroy)
        {
            DestroyImmediate(obj.gameObject);
        }
        yield break;

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

    public static IEnumerator SmoothMoveLine(LineRenderer line, Vector3 posToGoTo, float delay)
    {
        float SmoothTime = 0f;
        while (new Vector2((int)(line.GetPosition(1).x), (int)(line.GetPosition(1).y)) != new Vector2(posToGoTo.x, posToGoTo.y))
        {
            if (line != null)
            {
                line.SetPosition(1 , Vector3.Lerp(line.GetPosition(1), posToGoTo, SmoothTime));
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
    public static IEnumerator PlayVFX(GameObject parent, string VFXName, Color vfxColor, Vector3 offset, float duration = 1, float stopDuration = 0)
    {
        var VFX = Instantiate(Director.Instance.VFXList.Where(obj => obj.name == VFXName).SingleOrDefault(), Tools.GetGameObjectPositionAsVector3(parent) + offset, Quaternion.identity);
        VFX.GetComponent<SpriteRenderer>().material = Instantiate(VFX.GetComponent<SpriteRenderer>().material);
        var vfxMaterial = VFX.GetComponent<SpriteRenderer>().material;
        vfxMaterial.SetColor("_EmissionColor", vfxColor * 10);
        var anim = VFX.GetComponent<Animator>();
        anim.Play(VFXName);
        Director.Instance.StartCoroutine(Tools.ApplyAndReduceChromaticAbberation());
       yield return new WaitUntil(() => anim.GetBool("Done") == true);
        if(stopDuration > 0)
        Director.Instance.StartCoroutine(Tools.StopTime(stopDuration));
        yield return new WaitForSeconds(duration);
        Destroy(VFX);

    }

    public static void AddNewActionToUnit(Unit unit, string actionName)
    {
        unit.actionList.Add(Director.Instance.actionDatabase.Where(obj => obj.name == actionName).SingleOrDefault());
        unit.actionList[unit.actionList.Count - 1].New = true;
    }

    public static IEnumerator ApplyAndReduceChromaticAbberation()
    {
        if (BattleSystem.Instance.effectsSetting.sharedProfile.TryGet<ChromaticAberration>(out var CA))
        {
            CA.intensity.value = 1f;
           while(CA.intensity.value != 0)
            {
                CA.intensity.value -= 0.04f;
                yield return new WaitForSeconds(0.0001f);
            }
            
        }
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

    public static IEnumerator TurnOffDirectionalLight(float delay = 0.0001f)
    {
        if (BattleSystem.Instance.mainLight != null)
        {
            while (BattleSystem.Instance.mainLight.intensity != 0)
            {
                BattleSystem.Instance.mainLight.intensity -= 0.01f;
                yield return new WaitForSeconds(delay);
            }

        }
    }

    public static IEnumerator TurnOnDirectionalLight(float delay = 0.0001f)
    {
        if (BattleSystem.Instance.mainLight != null)
        {
            while (BattleSystem.Instance.mainLight.intensity <= BattleSystem.Instance.mainLightValue)
            {
                BattleSystem.Instance.mainLight.intensity += 0.01f;
                yield return new WaitForSeconds(0.0001f);
            }

        }
    }
    public static IEnumerator ChangeLightIntensity(Light light, float desiredIntensity, float amountToRaiseBy, float delay = 0)
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

    public static IEnumerator FadeObject(Image gameObject, float delay, bool FadeIn)
    {
        if(!gameObject.gameObject.activeSelf)
        {
            gameObject.gameObject.SetActive(true);
        }
        for (int i = 0; i < 101; i++)
        {
            if (gameObject != null && !FadeIn)
            {
                gameObject.color = new Color(0, 0, 0, 1 - i / 100f);
                yield return new WaitForSeconds(delay);
                if (gameObject.color == new Color(0, 0, 0, 0))
                {
                    gameObject.gameObject.SetActive(false);
                }
            }
            else if(gameObject != null)
            {
                gameObject.color = new Color(0, 0, 0, i / 100f);
                print(i / 100f);
                yield return new WaitForSeconds(delay);
            }
        }
    }

    public static void ModifyAction(Unit  unit, string actionToChange, int slotToChange , float newCost = 0f)
    {
        var oldAction = unit.actionList.Where(obj => obj.ActionName == actionToChange).SingleOrDefault();
        var newAction = Instantiate(oldAction);
        newAction.cost = newCost;
        unit.actionList[slotToChange] = newAction;
    }


}
