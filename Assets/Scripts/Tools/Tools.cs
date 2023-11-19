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

    public static int GetNumberOfPlayerUnits()
    {
        Unit[] units = UnityEngine.Object.FindObjectsOfType<Unit>();
        int i = 0;
        foreach (var x in units)
        {
            if (x.IsPlayerControlled)
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

    public static void PauseStaminaTimer()
    {
        Director.Instance.timeline.Paused = true;
    }

    public static void UnpauseStaminaTimer()
    {
        Director.Instance.timeline.Paused = false;
    }

    public static bool CheckIfAnyUnitIsDeciding()
    {
        bool result = false;
        foreach (var unit in Tools.GetAllUnits())
        {
            if (unit.state == PlayerState.DECIDING)
                result = true;
        }
        return result;

    }

    public static bool CheckIfAllUnitsAreReady()
    {
        bool result = true;
        if(BattleSystem.Instance != null)
        {
            foreach (var unit in BattleSystem.Instance.playerUnits)
            {
                if (unit.state != PlayerState.READY)
                {
                    result = false;
                    break;
                }
            }
        }       
        return result;

    }

    public static bool CheckIfAnyUnitIsTargetting()
    {
        bool result = false;
        foreach (var AC in GameObject.FindObjectsOfType<ActionContainer>())
        {
            if (AC.targetting)
                result = true;
        }
        return result;

    }

    public static Unit GetRandomEnemy(Unit unit)
    {
        Unit unitToReturn = new Unit();

        if (unit.IsPlayerControlled)
        {
            unitToReturn = BattleSystem.Instance.enemyUnits[UnityEngine.Random.Range(0, BattleSystem.Instance.enemyUnits.Count)];
        }
        else
        {
            unitToReturn = BattleSystem.Instance.playerUnits[UnityEngine.Random.Range(0, BattleSystem.Instance.playerUnits.Count)];
        }
        return unitToReturn;
    }

    public static Unit GetRandomAlly(Unit unit)
    {
        Unit unitToReturn = new Unit();

        if (unit.IsPlayerControlled)
        {
            unitToReturn = BattleSystem.Instance.playerUnits[UnityEngine.Random.Range(0, BattleSystem.Instance.playerUnits.Count)];
        }
        else
        {
            unitToReturn = BattleSystem.Instance.enemyUnits[UnityEngine.Random.Range(0, BattleSystem.Instance.enemyUnits.Count)];
        }
        return unitToReturn;
    }

    public static void EndAllTempEffectTimers()
    {
        foreach (var act in FindObjectsOfType<EffectIcon>())
        {
            act.DoFancyStatChanges = false;
            act.DestoryEffectIcon();
        }
    }

    public static void TickAllEffectIcons()
    {
        foreach (var act in FindObjectsOfType<EffectIcon>())
        {
            act.Tick();
        }
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

    public static IEnumerator FadeText(TextMeshProUGUI gameObject, float delay, bool FadeIn, bool SetInactiveAtEnd = true)
    {
        if (!gameObject.gameObject.activeSelf)
        {
            gameObject.gameObject.SetActive(true);
        }
        if (gameObject != null && !FadeIn)
        {
            while (gameObject.color.a > 0)
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
            while (gameObject.color.a < 1)
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
            while (gameObject.color.a > 0)
            {
                gameObject.color = new Color(gameObject.color.r, gameObject.color.g, gameObject.color.b, gameObject.color.a - 0.01f);
                yield return new WaitForSeconds(delay);
            }
            if (SetInactiveAtEnd)
            {
                gameObject.gameObject.SetActive(false);
            }
            if(DestroyAtEnd)
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
    public static string CheckNames(Unit unit)
    {
        string nameToReturn = "";
        for (int i = 0; i <= BattleSystem.Instance.enemyUnits.Count - 1; i++)
        {
            if (BattleSystem.Instance.enemyUnits[i].unitName == unit.unitName)
            {
                nameToReturn = $"{(i + 2)}";
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
        foreach (TimeLineChild child in Director.Instance.timeline.children)
        {
            if (child.unit.unitName == unit.unitName)
            {
                Director.Instance.timeline.children.Remove(child);
                Destroy(child.gameObject);
                break;
            }
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

  public static Unit FindDecidingUnit()
    {
        var DecidingUnit = new Unit();
        foreach (var unit in Tools.GetAllUnits())
        {
            if (unit.state == PlayerState.DECIDING)
            {
                DecidingUnit = unit;
                break;
            }
               
        }
        return DecidingUnit;
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
    public static void DetermineActionData(Unit baseUnit, int HowIsThisDetermined, Unit overrideTarget = null)
    {
        baseUnit.actionList[HowIsThisDetermined].unit = baseUnit;
        if (overrideTarget == null)
        {
            switch (baseUnit.actionList[HowIsThisDetermined].targetType)
            {
                case Action.TargetType.ENEMY:
                    {
                        baseUnit.actionList[HowIsThisDetermined].targets = GetRandomEnemy(baseUnit);
                    }
                    break;
                case Action.TargetType.SELF:
                    baseUnit.actionList[HowIsThisDetermined].targets = baseUnit;
                    break;
                case Action.TargetType.ALL_ENEMIES:
                    baseUnit.actionList[HowIsThisDetermined].targets = baseUnit;
                    break;
                case Action.TargetType.ALLY:
                    baseUnit.actionList[HowIsThisDetermined].targets = Tools.GetRandomAlly(baseUnit);
                    break;
            }
        }
        else
        {
            baseUnit.actionList[HowIsThisDetermined].targets = overrideTarget;
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
    public static IEnumerator PlayVFX(GameObject parent, string VFXName, Color vfxColor, Color particleColor, Vector3 offset, float duration = 1, float stopDuration = 0, bool ApplyChromaticAbberation = true, float ExtraDelay = 0, float intensityMultiplier = 10)
    {
        print("INTENSITY: " + intensityMultiplier);
        var VFX = Instantiate(Director.Instance.VFXList.Where(obj => obj.name == VFXName).SingleOrDefault(), Tools.GetGameObjectPositionAsVector3(parent) + offset, Quaternion.identity);
        VFX.transform.parent = null;
        if (VFX.GetComponent<SpriteRenderer>() != null)
        {
            VFX.GetComponent<SpriteRenderer>().material = Instantiate(VFX.GetComponent<SpriteRenderer>().material);
            var vfxMaterial = VFX.GetComponent<SpriteRenderer>().material;
            vfxMaterial.SetColor("_BaseColor", vfxColor * intensityMultiplier);
            vfxMaterial.SetColor("_EmissionColor", vfxColor);
        }
        if (VFX.GetComponent<MeshRenderer>() != null)
        {
            VFX.GetComponent<MeshRenderer>().material = Instantiate(VFX.GetComponent<MeshRenderer>().material);
            var vfxMaterial = VFX.GetComponent<MeshRenderer>().material;
            vfxMaterial.SetColor("_EmissionColor", vfxColor * (intensityMultiplier));
        }
        if (VFX.GetComponent<Animator>() != null)
        {
            var anim = VFX.GetComponent<Animator>();
            if(anim.HasState(0, Animator.StringToHash(VFXName)))
                anim.Play(VFXName);
        }
        if (VFX.GetComponent<ParticleSystem>() != null)
        {
            var particleSystem = VFX.GetComponent<ParticleSystem>();
            particleSystem.GetComponent<ParticleSystemRenderer>().material = Instantiate(particleSystem.GetComponent<ParticleSystemRenderer>().material);
            var particleMaterial = particleSystem.GetComponent<ParticleSystemRenderer>().material;
            particleMaterial.SetColor("_Color", particleColor * intensityMultiplier);
            particleMaterial.SetColor("_EmissionColor", particleColor *  intensityMultiplier);
        }

        if (ApplyChromaticAbberation)
            Director.Instance.StartCoroutine(Tools.ApplyAndReduceChromaticAbberation());

        if (VFX.GetComponent<Animator>() != null && VFX.GetComponent<Animator>().GetBool("Done"))
        {
            yield return new WaitUntil(() => VFX.GetComponent<Animator>().GetBool("Done") == true);
            if (stopDuration > 0)
                Director.Instance.StartCoroutine(Tools.StopTime(stopDuration));
        }
        yield return new WaitForSeconds(duration);
        if (VFX.GetComponent<ParticleSystem>() != null)
        {
            var particleSystem = VFX.GetComponent<ParticleSystem>();
            particleSystem.Stop();
        }
         if (VFX.GetComponent<SpriteRenderer>() != null)
         {
            VFX.GetComponent<SpriteRenderer>().material = Instantiate(VFX.GetComponent<SpriteRenderer>().material);
            var vfxMaterial = VFX.GetComponent<SpriteRenderer>().material;
            float alpha = vfxMaterial.GetColor("_BaseColor").a;
            while (vfxMaterial.GetColor("_BaseColor").a >= 0)
            {    
                 vfxMaterial.SetColor("_BaseColor", new Color(vfxColor.r, vfxColor.g, vfxColor.b, alpha));
                 alpha -= 0.05f;
                 yield return new WaitForSeconds(0.001f);
             }
         }
        yield return new WaitForSeconds(ExtraDelay);
        Destroy(VFX);

    }

    public static void AddNewActionToUnit(Unit unit, string actionName, bool ShownAsNew = true)
    {
        var oldAction = Director.Instance.actionDatabase.Where(obj => obj.ActionName == actionName).SingleOrDefault();
        unit.actionList.Add(oldAction);
        if(ShownAsNew)
        unit.actionList[unit.actionList.Count - 1].New = true;
    }

    public static IEnumerator ApplyAndReduceChromaticAbberation()
    {
        if (BattleSystem.Instance.effectsSetting.sharedProfile.TryGet<ChromaticAberration>(out var CA))
        {
            CA.intensity.value = 1f;
            while (CA.intensity.value != 0)
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
        while (sprite.material.color.a > 0)
        {
            alpha = 0.1f;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            yield return new WaitForSeconds(delay);
        }
        Destroy(gameObjectToChange);
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
        if(originalIEnumerator != null)
        {
            Director.Instance.StopCoroutine(originalIEnumerator);
        }
        originalIEnumerator = newIEnumerator;
        Director.Instance.StartCoroutine(originalIEnumerator);
    }


    public static void ModifyAction(Unit unit, string actionToChange, int slotToChange, float newCost = 0f)
    {
        var oldAction = unit.actionList.Where(obj => obj.ActionName == actionToChange).SingleOrDefault();
        var newAction = Instantiate(oldAction);
        newAction.cost = newCost;
        unit.actionList[slotToChange] = newAction;
    }

    public static void CheckIfActionWasFatalAndResetCam(Action action, int targetHP)
    {
        action.Done = true;
        if (targetHP >= 0)
        {
            LabCamera.Instance.ResetPosition();
        }
    }

    public static Unit CheckAndReturnNamedUnit(string unitName)
    {
        var unit = new Unit();
        foreach (var x in Tools.GetAllUnits())
        {
            if (x.unitName == unitName)
            {
                unit = x;
                break;
            }
        }
        return unit;
    }

    public static IEnumerator LateUnpause()
    {
        yield return new WaitForSeconds(0.5f);
        LabCamera.Instance.ResetPosition();
        yield break;
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

    public static void SetupEnemyAction(Unit baseUnit, int DecidingNum, Unit overrideTarget = null)
    {
        DetermineActionData(baseUnit, DecidingNum);
        BattleSystem.Instance.DisplayEnemyIntent(baseUnit.actionList[DecidingNum], baseUnit);
        DetermineActionData(baseUnit, DecidingNum);
        baseUnit.state = PlayerState.READY;
        Director.Instance.timeline.DoCost(baseUnit.actionList[DecidingNum].cost, baseUnit);
        BattleSystem.Instance.AddAction(baseUnit.actionList[DecidingNum]);
    }



}
