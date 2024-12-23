﻿using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CutsceneTools : MonoBehaviour
{
    private IEnumerator vignetteIEnumerator;
    public static void ZoomOnUnit(string unitName)
    {
        LabCamera.Instance.MoveToUnit(CombatTools.CheckAndReturnNamedUnit(unitName), Vector3.zero, 0, 8, -40, 0.5f, false);
        AudioManager.QuickPlay("ui_woosh_002");
    }

    public static void ChangeTimeDelay(float time)
    {
        LabCamera.Instance.MovingTimeDivider = time;
    }
    public static void MoveToUnit(string unitName)
    {
        LabCamera.Instance.MoveToUnit(CombatTools.CheckAndReturnNamedUnit(unitName), Vector3.zero, 0, 0, 0, 1, true);
        AudioManager.QuickPlay("ui_woosh_002");
    }

    public static void ResetCam()
    {
        LabCamera.Instance.ResetPosition();
        AudioManager.QuickPlay("ui_woosh_002");
    }

    public static void ResetRotation()
    {
        LabCamera.Instance.ResetRotation();
        //Director.Instance.StartCoroutine(CutsceneTools.CheckIfRotationIsDone());


    }

    public static IEnumerator CheckIfRotationIsDone()
    {
        while (LabCamera.Instance.state != LabCamera.CameraState.SWAY)
        {
            Tools.ToggleUiBlocker(false, true, true);
            yield return null;
        }
       // Tools.ToggleUiBlocker(true, true, true);


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
    public void ChangeVignetteIntensity(string DesiredValue)
    {
        if (DesiredValue == "Reset")
            Tools.StartAndCheckCoroutine(vignetteIEnumerator, ChangeVignetteIntensityCoroutine(BattleSystem.Instance.DefaultVignetteIntensity));
        else
            Tools.StartAndCheckCoroutine(vignetteIEnumerator, ChangeVignetteIntensityCoroutine(float.Parse(DesiredValue)));
    }

    public void StartMusicTrack(string TrackToPlay)
    {
        AudioManager.Instance.Play(TrackToPlay);
    }

    public static void ChangeMusicTrackVolume(float TargetVolume)
    {
        Director.Instance.StartCoroutine(AudioManager.Instance.Fade(TargetVolume, AudioManager.Instance.currentMusicTrack, 1.5f, false));
    }

    public void DoBossIntro()
    {
        Tools.ToggleUiBlocker(false, false, true);
        OptionsManager.Instance.StartCoroutine(Director.Instance.bossIntro.DoIntro());
    }

    public void QuickPlay(string AudioName)
    {
        AudioManager.QuickPlay(AudioName);
    }

    public void TurnOffCutsceneBlocker(float delay)
    {
        StartCoroutine(TurnOffCutsceneBlockerCoroutine(delay));
    }

    private IEnumerator TurnOffCutsceneBlockerCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Director.Instance.CutsceneUiBlocker.gameObject.SetActive(false);
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
