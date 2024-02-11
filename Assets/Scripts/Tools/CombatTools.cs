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

public class CombatTools : MonoBehaviour
{


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
        AudioManager.Instance.Play("timeline_fill");
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

    public static bool CheckIfAnyUnitsAreDying()
    {
        bool result = false;
        foreach (var unit in Tools.GetAllUnits())
        {
            if (unit.Dying)
                result = true;
        }
        return result;

    }

    public static bool CheckIfAllUnitsAreReady()
    {
        bool result = true;
        if (BattleSystem.Instance != null)
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

    public static string CheckNames(Unit unit)
    {
        string nameToReturn = "";
        for (int i = 0; i < BattleSystem.Instance.enemyUnits.Count; i++)
        {
            if (BattleSystem.Instance.enemyUnits[i].unitName == unit.unitName)
            {
                nameToReturn = $"{unit.unitName}({i + 1})";
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
    public static void ModifyAction(Unit unit, string actionToChange, int slotToChange, float newCost = 0f)
    {
        var oldAction = unit.actionList.Where(obj => obj.ActionName == actionToChange).SingleOrDefault();
        var newAction = Instantiate(oldAction);
        newAction.cost = newCost;
        unit.actionList[slotToChange] = newAction;
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
                    baseUnit.actionList[HowIsThisDetermined].targets = CombatTools.GetRandomAlly(baseUnit);
                    break;
            }
        }
        else
        {
            baseUnit.actionList[HowIsThisDetermined].targets = overrideTarget;
        }

    }

    public static IEnumerator PlayVFX(GameObject parent, string VFXName, Color vfxColor, Color particleColor, Vector3 offset, Quaternion rotation, float duration = 1, float stopDuration = 0, bool ApplyChromaticAbberation = true, float ExtraDelay = 0, float intensityMultiplier = 10, float ChromaticDelay = 0.0001f, string AudioToPlay = "")
    {
        var VFX = Instantiate(Director.Instance.VFXList.Where(obj => obj.name == VFXName).SingleOrDefault(), Tools.GetGameObjectPositionAsVector3(parent) + offset, rotation);
        VFX.transform.parent = null;
        if (VFX != null)
        {
            if (VFX.GetComponent<SpriteRenderer>() != null)
            {
                VFX.GetComponent<SpriteRenderer>().material = Instantiate(VFX.GetComponent<SpriteRenderer>().material);
                var vfxMaterial = VFX.GetComponent<SpriteRenderer>().material;
                vfxMaterial.SetColor("_BaseColor", vfxColor * intensityMultiplier);
                vfxMaterial.SetColor("_EmissionColor", vfxColor * intensityMultiplier);
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
                if (anim.HasState(0, Animator.StringToHash(VFXName)))
                    anim.Play(VFXName);
            }
            if (VFX.GetComponent<ParticleSystem>() != null)
            {
                var particleSystem = VFX.GetComponent<ParticleSystem>();
                particleSystem.GetComponent<ParticleSystemRenderer>().material = Instantiate(particleSystem.GetComponent<ParticleSystemRenderer>().material);
                var particleMaterial = particleSystem.GetComponent<ParticleSystemRenderer>().material;
                particleMaterial.SetColor("_Color", particleColor * intensityMultiplier);
                particleMaterial.SetColor("_EmissionColor", particleColor * intensityMultiplier);
            }

            if (ApplyChromaticAbberation)
                Director.Instance.StartCoroutine(CombatTools.ApplyAndReduceChromaticAbberation(ChromaticDelay));

            if (VFX.GetComponent<Animator>() != null && VFX.GetComponent<Animator>().GetBool("Done"))
            {
                yield return new WaitUntil(() => VFX.GetComponent<Animator>().GetBool("Done") == true);
                if (stopDuration > 0)
                    Director.Instance.StartCoroutine(Tools.StopTime(stopDuration));
            }
            if (AudioToPlay != "")
            {
                AudioManager.QuickPlay(AudioToPlay);
            }

            yield return new WaitForSeconds(duration);
            if (VFX != null && VFX.GetComponent<ParticleSystem>() != null)
            {
                var particleSystem = VFX.GetComponent<ParticleSystem>();
                particleSystem.Stop();
            }
            if (AudioToPlay != "")
            {
                var currentSFX = AudioManager.Instance.ReturnSound(AudioToPlay);
                if (currentSFX.loop)
                    Director.Instance.StartCoroutine(AudioManager.Instance.Fade(0, AudioToPlay, 1f, true));
                else
                    AudioManager.Instance.Stop(AudioToPlay);
            }
            if (VFX != null && VFX.GetComponent<SpriteRenderer>() != null)
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
            if (VFX != null)
                Destroy(VFX);
        }

    }

    public static IEnumerator StopAndDestroyVFX(float delay)
    {
        var statUpObject = GameObject.FindGameObjectsWithTag("StatUp");
        for (int i = 0; i < statUpObject.Count(); i++)
        {
            statUpObject[i].GetComponent<ParticleSystem>().Stop();
            yield return new WaitForSeconds(delay);
            Destroy(statUpObject[i]);
        }
    }


    public static IEnumerator ApplyAndReduceChromaticAbberation(float delay = 0.0001f)
    {
        if (BattleSystem.Instance.effectsSetting.sharedProfile.TryGet<ChromaticAberration>(out var CA))
        {
            CA.intensity.value = 1f;
            while (CA.intensity.value != 0)
            {
                CA.intensity.value -= 0.04f;
                yield return new WaitForSeconds(delay);
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
        if (BattleSystem.Instance != null && BattleSystem.Instance.lablights != null)
        {
            foreach (var light in BattleSystem.Instance.lablights.ToList())
            {
                float currentIntensity = light.lightComponent.intensity;
                float startIntensity = light.startIntensity;
                float currentTime = 0;
                float TargetIntensity = startIntensity / 40;
                while (light.lightComponent.intensity > TargetIntensity)
                {
                    currentTime += Time.deltaTime * delay;
                    light.lightComponent.intensity = Mathf.Lerp(currentIntensity, TargetIntensity, currentTime);
                    yield return null;
                }
            }
        }
    }
    public static IEnumerator TurnOnDirectionalLight(float delay = 0.0001f)
    {
        if (BattleSystem.Instance != null && BattleSystem.Instance.lablights != null)
        {
            foreach (var light in BattleSystem.Instance.lablights.ToList())
            {
                float currentIntensity = light.lightComponent.intensity;
                float startIntensity = light.startIntensity;
                float currentTime = 0;
                float TargetIntensity = startIntensity;
                while (light.lightComponent.intensity < TargetIntensity)
                {
                    currentTime += Time.deltaTime * delay;
                    light.lightComponent.intensity = Mathf.Lerp(currentIntensity, TargetIntensity, currentTime);
                    yield return null;
                }
            }
        }
    }

    public static void CheckIfActionWasFatalAndResetCam(Action action, int targetHP)
    {
        action.Done = true;
        if (targetHP > 0)
        {
            LabCamera.Instance.ResetPosition();
            AudioManager.QuickPlay("ui_woosh_002");
        }
    }

    public static bool ReturnIconStatus(Unit unit, string IconName)
    {
        var boolToReturn = false;
        if (!unit.statusEffects.Contains(unit.statusEffects.Where(obj => obj.iconName == IconName).SingleOrDefault()))
        {
            boolToReturn = true;
        }
        return boolToReturn;
    }

    public static Unit CheckAndReturnNamedUnit(string unitName)
    {
        var unit = new Unit();
        foreach (var x in Tools.GetAllUnits())
        {
            if (x.unitName.Contains(unitName))
            {
                unit = x;
                break;
            }
        }
        return unit;
    }

    public static void SetupEnemyAction(Unit baseUnit, int DecidingNum, Unit overrideTarget = null)
    {
       
        DetermineActionData(baseUnit, DecidingNum, overrideTarget);
        BattleSystem.Instance.DisplayIntent(baseUnit.actionList[DecidingNum], baseUnit);
        DetermineActionData(baseUnit, DecidingNum, overrideTarget);
        baseUnit.state = PlayerState.READY;
        var newAction = UnityEngine.Object.Instantiate(baseUnit.actionList[DecidingNum]);
        Director.Instance.timeline.DoCost(CombatTools.DetermineTrueCost(newAction), baseUnit);
        BattleSystem.Instance.AddAction(newAction);
    }

    public static float DetermineTrueCost(Action action)
    {
        float floatToReturn = 0;
        if (action != null)
        {
            switch (action.actionStyle)
            {
                case Action.ActionStyle.LIGHT:
                    {
                        floatToReturn = action.lightCost;
                    }
                    break;
                case Action.ActionStyle.HEAVY:
                    {
                        floatToReturn = action.heavyCost;
                    }
                    break;
                case Action.ActionStyle.STANDARD:
                    {
                        floatToReturn = action.cost;
                    }
                    break;
            }
        }
        return floatToReturn;
    }

    public static int DetermineTrueActionValue(Action action)
    {
        int valueToReturn = 0;
        if (action.actionType == Action.ActionType.STATUS)
        {
            switch (action.actionStyle)
            {
                case Action.ActionStyle.LIGHT:
                    {
                        valueToReturn = action.lightStatAmount;
                    }
                    break;
                case Action.ActionStyle.HEAVY:
                    {
                        valueToReturn = action.heavyStatAmount;
                    }
                    break;
                case Action.ActionStyle.STANDARD:
                    {
                        valueToReturn = action.statAmount;
                    }
                    break;
            }
        }
        else
        {
            switch (action.actionStyle)
            {
                case Action.ActionStyle.LIGHT:
                    {
                        valueToReturn = action.lightDamage;
                    }
                    break;
                case Action.ActionStyle.HEAVY:
                    {
                        valueToReturn = action.heavyDamage;
                    }
                    break;
                case Action.ActionStyle.STANDARD:
                    {
                        valueToReturn = action.damage;
                    }
                    break;
            }

        }

        return valueToReturn;
    }

    public static float ReturnTypeMultiplier(Unit target, DamageType damageType)
    {
        float typeMultiplier = 1;
        if (target != null)
        {
            foreach (var weakness in target.weaknesses)
            {
                if (weakness == damageType)
                {
                    typeMultiplier *= Director.Instance.WeaknessMultiplier;
                }
            }
            foreach (var resistance in target.resistances)
            {
                if (resistance == damageType)
                {
                    typeMultiplier *= Director.Instance.ResistanceMultiplier;
                }
            }
        }
        return typeMultiplier;
    }
    public static PipCounter ReturnPipCounter()
    {
        if (Director.Instance.UnlockedPipSystem)
            return Director.Instance.timeline.pipCounter;
        else
            return new PipCounter();
    }


}
