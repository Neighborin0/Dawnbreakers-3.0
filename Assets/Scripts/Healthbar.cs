using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.UI.CanvasScaler;
using System.Buffers;
using UnityEditor.Rendering;
using static Action;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public Unit unit;
    public TextMeshProUGUI text;
    public LabPopup damagePopUp;
    public Slider backSlider;
    public bool DeathPaused = false;
    public float DamageModifier = 1;
    [SerializeField]
    private Image healthbarImageComponent;
    [SerializeField]
    private Sprite ArmorSprite;
    [SerializeField]
    private Sprite NormalSprite;

    void Start()
    {
        if (unit != null)
        {
            try
            {
                InitializeHealthbar();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in Healthbar Start(): {e.Message}");
            }
        }
    }

    void Update()
    {
        UpdateHealthbar();
    }

    private void InitializeHealthbar()
    {
        slider.maxValue = unit.maxHP;
        unit.namePlate.DEF_icon.SetActive(false);
        unit.armor = 0;
        backSlider.maxValue = slider.maxValue;
        slider.value = unit.currentHP;
        backSlider.value = unit.currentHP;
        text.text = $"{unit.currentHP} / {unit.maxHP}";
    }

    private void UpdateHealthbar()
    {
        if (unit != null)
        {
            slider.value = unit.currentHP;
            text.text = $"{unit.currentHP} / {unit.maxHP}";
            UpdateHealthbarImage();
        }
    }

    private void UpdateHealthbarImage()
    {
        if (unit.namePlate != null && unit.armor > 0)
        {
            healthbarImageComponent.sprite = ArmorSprite;
        }
        else
        {
            healthbarImageComponent.sprite = NormalSprite;
        }
    }

    public void TakeDamage(int damage, Unit damageSource, DamageType damageType, Action.ActionStyle actionStyle, bool ignoresDEF = false, bool appliesStun = false)
    {
        if (unit == null)
        {
            return;
        }

        int trueDamage = CalculateTrueDamage(damage, ignoresDEF);
        UpdateUnitHealth(trueDamage, damage);
        UpdateArmor();
        StartCoroutine(ShowDamagePopup(trueDamage, damageType, actionStyle, appliesStun));
        StartCoroutine(HandleSlider());
    }

    private int CalculateTrueDamage(int damage, bool ignoresDEF)
    {
        int trueDamage = ignoresDEF ? damage : Mathf.Max(damage - unit.armor, 0);
        return trueDamage;
    }

    private void UpdateUnitHealth(int trueDamage, int damage)
    {
        unit.currentHP -= trueDamage;
        unit.currentHP = Mathf.Max(unit.currentHP, 0);
        unit.armor -= damage;
        unit.armor = Mathf.Max(unit.armor, 0);
    }

    private void UpdateArmor()
    {
        unit.namePlate?.UpdateArmor(unit.armor);
    }

    private IEnumerator ShowDamagePopup(int trueDamage, DamageType damageType, Action.ActionStyle actionStyle, bool appliesStun)
    {
        gameObject.SetActive(true);
        yield return StartCoroutine(DamagePopUp(trueDamage, damageType, actionStyle, appliesStun));
    }

    private IEnumerator HandleSlider()
    {
        yield return new WaitForSeconds(0.5f);
        while (backSlider.value > slider.value && this != null)
        {
            backSlider.value -= 2f;
            yield return new WaitForSeconds(0.01f);
        }
    }

  
    private IEnumerator LateRemove()
    {
        if (unit != null)
        {
            yield return new WaitForSeconds(1f);
            Director.Instance.timeline.RemoveTimelineChild(unit);
        }
        yield break;
    }

    private void HandleTypeDamage(DamageType damageType, TextMeshProUGUI number, int damage, Action.ActionStyle actionStyle, bool AppliesStun = false)
    {
        number.SetText(damage.ToString());

        if (CombatTools.ReturnTypeMultiplier(unit, damageType) < 1)
        {
            number.faceColor = new Color(0.5754717f, 0.4533197f, 0.4533197f);
            Debug.LogWarning(number.color.ToString());
            Debug.LogWarning("NOT EFFECTIVE");
        }
        else if (damage == 0 && unit.armor > 0)
        {
            number.color = new Color(0.04313726f, 0.4431373f, 0.5450981f);
            Debug.LogWarning(number.color.ToString());
            Debug.LogWarning("ARMOR");
            AudioManager.QuickPlay("armor_hit_001");
        }
        else
        {
            number.color = new Color(1, 0.3647059f, 0.3647059f);
            Debug.LogWarning("NORMAL");

        }
        number.outlineColor = Color.black;
        number.outlineWidth = 0.2f;

        if (Director.Instance.timeline.ReturnTimelineChild(unit) != null)
        {
            var TL = Director.Instance.timeline.ReturnTimelineChild(unit);
            var action = Director.Instance.timeline.ReturnTimeChildAction(unit);
            var prevModifier = unit.knockbackModifider;
            if (!TL.CanClear)
            {
                if (CombatTools.ReturnTypeMultiplier(unit, damageType) > 1 || actionStyle != Action.ActionStyle.STANDARD || AppliesStun) //effective
                {
                    if (CombatTools.ReturnIconStatus(unit, "STALWART") && CombatTools.ReturnIconStatus(unit, "INDOMITABLE") && Director.Instance.timeline.ReturnTimelineChild(unit) != null) //Applies Stun
                    {
                        if (CombatTools.ReturnTypeMultiplier(unit, damageType) > 1)
                        {
                            TL.value -= Director.Instance.TimelineReduction;
                            action.cost += Director.Instance.TimelineReduction;
                            number.color = Color.red;
                        }

                        if (actionStyle != Action.ActionStyle.STANDARD)
                        {
                            TL.value -= 10;
                            action.cost += 10;
                            if (actionStyle == Action.ActionStyle.LIGHT)
                            {
                                number.outlineColor = new Color(0, 0.635f, 0.749f);
                            }
                            else if (actionStyle == Action.ActionStyle.HEAVY)
                            {
                                number.outlineColor = new Color(1, 0.011f, 0);
                            }
                        }


                        if (AppliesStun)
                        {
                            unit.knockbackModifider = 100;
                        }


                        if (unit.knockbackModifider != 0)
                        {
                            if(AppliesStun)
                            {
                                TL.value = -1;
                                action.cost = 101;
                            }
                            else
                            {
                                TL.value -= unit.knockbackModifider;
                                action.cost += unit.knockbackModifider;
                            }

                           
                        }

                        if (TL.value <= 0)
                        {
                            TL.CanClear = true;
                            TL.GetComponent<LabUIInteractable>().CanHover = false;
                            BattleSystem.Instance.StartCoroutine(LateRemove());
                            BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "Stun", Color.yellow, Color.yellow, new Vector3(0, 2, 0f), Quaternion.identity, 15f, 0, true, 0, 2));
                            BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "Stun", Color.yellow, Color.yellow, new Vector3(0, 2, 0f), new Quaternion(0, 180, Quaternion.identity.z, Quaternion.identity.w), 15f, 0, true, 0, 2));
                            AudioManager.QuickPlay("stun_001");
                            BattleSystem.Instance.DoTextPopup(unit, "Stun", Color.yellow);
                            BattleSystem.Instance.SetTempEffect(unit, "STALWART", false);
                            if (!unit.IsPlayerControlled)
                            {
                                unit.behavior.turn--;
                            }
                        }
                    }
                    else if (CombatTools.ReturnIconStatus(unit, "INDOMITABLE"))
                    {
                        if (TL.value <= Director.Instance.TimelineReduction)
                        {
                            TL.value = 0;
                            action.cost = 100;
                        }
                    }
                    else if (CombatTools.ReturnIconStatus(unit, "STALWART")) //Removes Stalwart
                    {
                        if (TL.value <= Director.Instance.TimelineReduction)
                        {
                            TL.value = 0;
                            action.cost = 100;
                            var stalwart = unit.statusEffects.Where(obj => obj.iconName == "STALWART").SingleOrDefault();
                            unit.statusEffects.Remove(unit.statusEffects.Where(obj => obj.iconName == "STALWART").SingleOrDefault());
                            Destroy(stalwart.gameObject);
                        }

                    }
                }
            }

            if (AppliesStun)
            {
                unit.knockbackModifider = prevModifier;
            }
        }

    }

    private IEnumerator DamagePopUp(int damage, DamageType damageType, Action.ActionStyle actionStyle, bool AppliesStun = false)
    {
        if (unit == null)
            yield break;

        unit.DoOnDamaged();

        if (unit.currentHP < 1)
        {
            var popup = Instantiate(damagePopUp, new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x, unit.GetComponent<SpriteRenderer>().bounds.center.y + 2, unit.transform.position.z), Quaternion.identity);
            var number = popup.GetComponentInChildren<TextMeshProUGUI>();

            try
            {
                HandleTypeDamage(damageType, number, damage, actionStyle, AppliesStun);
            }
            catch
            {
                Debug.LogError("Text isn't being found?");
            }

            StartCoroutine(popup.Pop());
            Director.Instance.StartCoroutine(FlashWhite());

            unit.Dying = true;
            BattleSystem.Instance.BattlePhasePause = true;

            if (unit.IsPlayerControlled)
            {
                DeathPaused = true;
                LabCamera.Instance.state = LabCamera.CameraState.IDLE;
                yield return new WaitForSeconds(1f);
                Director.Instance.StartCoroutine(popup.DestroyPopUp());
                unit.DoDeathQuote();
                LabCamera.Instance.MoveToUnit(unit, Vector3.zero, 0, 8, -40, 0.5f);
                StartCoroutine(AudioManager.Instance.Fade(0, AudioManager.Instance.currentMusicTrack, 0.5f, false));
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
                LabCamera.Instance.MoveToUnit(unit, Vector3.zero, 0, 8, -40, 0.5f);
            }

            unit.DoOnPreDeath();

            if (unit.unitName == "Dusty" && BattleSystem.Instance.enemyUnits.Any(obj => obj.unitName.Contains("Matriarch")))
            {
                BattleSystem.Instance.DustyIsDead = true;
                LabCamera.Instance.uicam.gameObject.SetActive(false);
            }

            yield return new WaitUntil(() => !DeathPaused);

            if (unit.unitName == "Dusty" && BattleSystem.Instance.enemyUnits.Any(obj => obj.unitName.Contains("Matriarch")))
                LabCamera.Instance.uicam.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            AudioManager.QuickPlay("rising_light_001");
            Director.Instance.StartCoroutine(Tools.ChangeObjectEmissionToMaxIntensity(unit.gameObject, Color.yellow, 0.02f));
            unit.ChangeUnitsLight(unit.spotLight, 150, 15, Color.yellow, 0.04f, 0.1f);

            yield return new WaitForSeconds(0.8f);
            LabCamera.Instance.Shake(0.5f, 1.5f);
            AudioManager.QuickPlay("light_burst_001");
            Director.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "DeathBurst", Color.yellow, Color.yellow, Vector3.zero, Quaternion.identity, 10, 0, false));
            yield return new WaitForSeconds(0.03f);

            if (popup != null)
                Director.Instance.StartCoroutine(popup.DestroyPopUp());

            Director.Instance.StartCoroutine(Die());
        }
        else
        {
            var popup = Instantiate(damagePopUp, new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x, unit.GetComponent<SpriteRenderer>().bounds.center.y + 2, unit.transform.position.z), Quaternion.identity);
            var number = popup.GetComponentInChildren<TextMeshProUGUI>();

            try
            {
                HandleTypeDamage(damageType, number, damage, actionStyle, AppliesStun);
            }
            catch
            {
                //Debug.LogError("Text isn't being found?");
            }

            StartCoroutine(popup.Pop());
            Director.Instance.StartCoroutine(FlashWhite());
            yield return new WaitForSeconds(1.2f);
            Director.Instance.StartCoroutine(popup.DestroyPopUp());
        }
    }


    public IEnumerator FlashWhite()
    {
        yield return new WaitForSeconds(0.1f);
        unit.GetComponent<SpriteRenderer>().material.SetColor("_CharacterEmission", new Color(1f, 1f, 1f));
        unit.HitEmissionChanged = true;
        yield return new WaitForSeconds(0.1f);
        unit.GetComponent<SpriteRenderer>().material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f));
        unit.HitEmissionChanged = false;
    }
    public IEnumerator Die()
    {
        var BattleSpawnPoint = unit.GetComponentInParent<BattleSpawnPoint>();
        BattleSpawnPoint.Occupied = false;
        BattleSpawnPoint.unit = null;
        unit.GetComponent<SpriteRenderer>().enabled = false;

        if (unit.IsPlayerControlled)
        {
            LabCamera.Instance.uicam.gameObject.SetActive(false);
            BattleSystem.Instance.playerUnits.Remove(unit);
            yield return new WaitForSeconds(0.7f);

            foreach (var x in Tools.GetAllUnits())
            {
                x.DoOnPlayerUnitDeath();
            }

            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => BattleLog.Instance.characterdialog.IsActive());
            yield return new WaitUntil(() => !BattleLog.Instance.characterdialog.IsActive());
            Director.Instance.StartCoroutine(AudioManager.Instance.Fade(1, AudioManager.Instance.currentMusicTrack, 1f, false));
            Debug.Log("Player should be dead");
        }
        else
        {
            BattleSystem.Instance.enemyUnits.Remove(unit);
            Debug.Log("Enemy should be dead");
        }

        BattleSystem.Instance.numOfUnits.Remove(unit);
        Destroy(unit.ActionLayout);
        CombatTools.TurnOffCriticalUI(unit);
        yield return new WaitForSeconds(0.2f);

        if (unit.unitName != "Dusty")
        {
            yield return new WaitForSeconds(0.5f);
            Director.Instance.StartCoroutine(Tools.LateUnpause(true));
        }
        else
        {
            Director.Instance.StartCoroutine(Tools.LateUnpause());
        }

        Destroy(unit.gameObject);
    }



}
