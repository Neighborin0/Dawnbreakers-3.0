using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CutsceneTools : MonoBehaviour
{
    private IEnumerator vignetteIEnumerator;
    public static void ZoomOnUnit(string unitName)
    {
        foreach (var unit in Tools.GetAllUnits())
        {
            if (unit.unitName == unitName)
            {
                LabCamera.Instance.MoveToUnit(unit, 0, 8, -50, false, 0.5f, false);

            }
        }
    }

    public static void MoveToUnit(string unitName)
    {
        foreach (var unit in Tools.GetAllUnits())
        {
            if (unit.unitName == unitName)
            {
                LabCamera.Instance.MoveToUnit(unit, 0, 0, 0, false, 1, true);

            }
        }
    }

    public static void ResetCam()
    {
       LabCamera.Instance.ResetPosition();
    }

    public static void ResetRotation()
    {
        LabCamera.Instance.ResetRotation();
    }

    public static void RevealUnit(string unitName)
    {
        foreach (var unit in Tools.GetAllUnits())
        {
            if (unit.unitName == unitName)
            {
                unit.IsHidden = false;
            }
        }
    }
    public void ChangeVignetteIntensity(float DesiredValue)
    {
        vignetteIEnumerator = ChangeVignetteIntensityCoroutine(DesiredValue);
        Tools.StartAndCheckCoroutine(vignetteIEnumerator);
    }

    private IEnumerator ChangeVignetteIntensityCoroutine(float DesiredValue)
    {
        if (BattleSystem.Instance.effectsSetting.sharedProfile.TryGet<Vignette>(out var vignette))
        {
            if (vignette.intensity.value > DesiredValue)
            {
                while (vignette.intensity.value > DesiredValue)
                {
                    vignette.intensity.value -= 0.01f;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            if (vignette.intensity.value < DesiredValue)
            {
                while (vignette.intensity.value < DesiredValue)
                {
                    vignette.intensity.value += 0.01f;
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }

   
}
