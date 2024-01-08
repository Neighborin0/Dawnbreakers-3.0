using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CutsceneTools : MonoBehaviour
{
    private IEnumerator vignetteIEnumerator;
    public static void ZoomOnUnit(string unitName)
    {
        LabCamera.Instance.MoveToUnit(CombatTools.CheckAndReturnNamedUnit(unitName), Vector3.zero,0,8, -40, 0.5f, false);
    }

    public static void ChangeTimeDelay(float time)
    {
        LabCamera.Instance.MovingTimeDivider = time;
    }
    public static void MoveToUnit(string unitName)
    {
        LabCamera.Instance.MoveToUnit(CombatTools.CheckAndReturnNamedUnit(unitName), Vector3.zero, 0, 0, 0, 1, true);
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
        CombatTools.CheckAndReturnNamedUnit(unitName).IsHidden = false;
    }

    public static void SelectFirstUnit()
    {
        BattleSystem.Instance.playerUnits[0].StartDecision();
    }

    public static void EndBattlePhasePause()
    {
       BattleSystem.Instance.BattlePhasePause = false;
    }
    public void ChangeVignetteIntensity(float DesiredValue)
    {
        Tools.StartAndCheckCoroutine(vignetteIEnumerator, ChangeVignetteIntensityCoroutine(DesiredValue));
    }

    public void StartMusicTrack(string TrackToPlay)
    {
       AudioManager.Instance.Play(TrackToPlay);
    }

    public void ChangeMusicTrackVolume(float TargetVolume)
    {
        Director.Instance.StartCoroutine(AudioManager.Instance.Fade(TargetVolume, AudioManager.Instance.currentMusicTrack, 1.5f, false));
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
