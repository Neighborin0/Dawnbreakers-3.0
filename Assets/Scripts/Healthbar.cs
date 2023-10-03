using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public Unit unit;
    public TextMeshProUGUI text;
    public LabPopup damagePopUp;
    public Slider backSlider;
    public bool DeathPaused = false;
    public float DamageModifier = 1;

    void Start()
    {
        slider.maxValue = unit.maxHP;
        backSlider.maxValue = slider.maxValue;
        slider.value = unit.currentHP;
        backSlider.value = unit.currentHP;
        text.text = $"{unit.currentHP} / {unit.maxHP}";

    }
    void Update()
    {
        if (unit != null)
        {
            slider.value = unit.currentHP;
            text.text = $"{unit.currentHP} / {unit.maxHP}";
        }
    }

    public void TakeDamage(int damage, Unit DamageSource)
    {
        //RunTracker.Instance.slayer = DamageSource;
        if (unit != null)
        {
            var truedamage = (int)Math.Round((damage - unit.defenseStat) * DamageModifier);
            if (truedamage < 1)
            {
                truedamage = 0;
            }
            print(truedamage);
            backSlider.value = slider.value;
            unit.currentHP -= truedamage;
            if (this != null)
            {
                this.gameObject.SetActive(true);
                StartCoroutine(DamagePopUp(truedamage));
                StartCoroutine(HandleSlider());
            }
        }
    }

    public void Die()
    {
        var battlesystem = BattleSystem.Instance;
        var BattleSpawnPoint = unit.GetComponentInParent<BattleSpawnPoint>();
        BattleSpawnPoint.Occupied = false;
        BattleSpawnPoint.unit = null;
        if (unit.IsPlayerControlled)
        {
            battlesystem.playerUnits.Remove(unit);
            LabCamera.Instance.ResetPosition(true);
            BattleLog.Instance.ResetBattleLog();
            foreach (var x in BattleSystem.Instance.playerUnits)
            {
                if (x.stamina.slider.value == x.stamina.slider.maxValue)
                {
                    BattleSystem.Instance.state = BattleStates.DECISION_PHASE;
                    x.StartDecision();
                    break;
                }

            }
            print("Player should be dead");
        }
        else
        {
            battlesystem.enemyUnits.Remove(unit);
            print("Enemy should be dead");
        }
        battlesystem.numOfUnits.Remove(unit);
        Tools.TurnOffCriticalUI(unit);
        Destroy(unit.ActionLayout);
        Destroy(unit.gameObject);
        if (battlesystem.playerUnits.Count == 0)
        {
            if (OptionsManager.Instance.IntensityLevel == 0)
            {
                Tools.PauseAllStaminaTimers();
                Destroy(MapController.Instance.gameObject);
                Director.Instance.StartCoroutine(OptionsManager.Instance.DoLoad("Prologue Ending"));
            }
            else
            {
                //RunTracker.Instance.DisplayStats();
                Tools.ToggleUiBlocker(false, false);
                Director.Instance.timeline.GetComponent<MoveableObject>().Move(true);
                Tools.PauseAllStaminaTimers();
            }
        }
        else
        {
            if (BattleSystem.Instance.state != BattleStates.DECISION_PHASE && BattleSystem.Instance.state != BattleStates.WON && BattleSystem.Instance.state != BattleStates.DEAD && BattleSystem.Instance.state != BattleStates.TALKING)
            {
                BattleSystem.Instance.state = BattleStates.IDLE;
                Tools.UnpauseAllStaminaTimers();
            }
        }
        if(unit.IsPlayerControlled)
        {
            foreach (var x in Tools.GetAllUnits())
            {
                x.DoOnPlayerUnitDeath();
            }
        }
        Destroy(this.gameObject);

    }

    private IEnumerator HandleSlider()
    {
        yield return new WaitForSeconds(0.5f);
        while (backSlider.value > slider.value && this != null)
        {
            backSlider.value -= 2f;
            yield return new WaitForSeconds(0.01f);
        }
        yield break;
    }
    private IEnumerator DamagePopUp(int damage)
    {
        if (unit != null)
        {
            unit.DoOnDamaged();
            if (unit.currentHP < 1)
            {
                var popup = Instantiate(damagePopUp, new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x, unit.GetComponent<SpriteRenderer>().bounds.center.y + 2, unit.transform.position.z), Quaternion.identity);
                var number = popup.GetComponentInChildren<TextMeshProUGUI>();
                try
                {
                    number.SetText(damage.ToString());
                    number.color = Color.red;
                    number.outlineColor = Color.black;
                    number.outlineWidth = 0.2f;

                }
                catch
                {
                    print("text isn't being found?");
                }
                StartCoroutine(popup.Pop());
                if (unit.IsPlayerControlled)
                {
                    DeathPaused = true;
                    LabCamera.Instance.state = LabCamera.CameraState.IDLE;
                    yield return new WaitForSeconds(1f);
                    unit.DoDeathQuote();
                    LabCamera.Instance.MoveToUnit(unit, 0, 8, -50, false, 0.5f);
                    yield return new WaitForSeconds(0.2f);
                    Director.Instance.StartCoroutine(popup.DestroyPopUp());
                }
                unit.DoOnPreDeath();
                yield return new WaitUntil(() => !DeathPaused);
                if (unit.IsPlayerControlled)
                    Tools.PauseAllStaminaTimers();
                BattleLog.Instance.ResetBattleLog();
                var sprite = unit.GetComponent<SpriteRenderer>();
                yield return new WaitForSeconds(0.5f);
                unit.Dying = true;
                Director.Instance.StartCoroutine(Tools.ChangeObjectEmissionToMaxIntensity(unit.gameObject, Color.yellow, 0.07f));
                unit.spotLight.color = Color.yellow;
                unit.ChangeUnitsLight(unit.spotLight, 150, 15, 0.04f, 0.1f);
                yield return new WaitForSeconds(0.7f);
                LabCamera.Instance.Shake(0.5f, 2f);
                Director.Instance.StartCoroutine(Tools.PlayVFX(unit.gameObject, "DeathBurst", Color.yellow, Color.yellow, Vector3.zero, 3, 0, false));
                yield return new WaitForSeconds(0.03f);
                if (popup != null)
                    Director.Instance.StartCoroutine(popup.DestroyPopUp());
                Die();

            }
            else
            {

                unit.GetComponent<SpriteRenderer>().material.SetColor("_CharacterEmission", new Color(1f, 1f, 1f));
                var popup = Instantiate(damagePopUp, new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x, unit.GetComponent<SpriteRenderer>().bounds.center.y + 2, unit.transform.position.z), Quaternion.identity);
                var number = popup.GetComponentInChildren<TextMeshProUGUI>();
                try
                {
                    number.SetText(damage.ToString());
                    number.color = Color.red;
                    number.outlineColor = Color.black;
                    number.outlineWidth = 0.2f;

                }
                catch
                {
                    print("text isn't being found?");
                }
                StartCoroutine(popup.Pop());
                yield return new WaitForSeconds(0.1f);
                unit.GetComponent<SpriteRenderer>().material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f));
                yield return new WaitForSeconds(1f);
                Director.Instance.StartCoroutine(popup.DestroyPopUp());
            }
        }
    }


}
