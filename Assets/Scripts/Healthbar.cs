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
         try
        {
            slider.maxValue = unit.maxHP;
            unit.namePlate.DEF_icon.SetActive(false);
            unit.armor = 0;
            backSlider.maxValue = slider.maxValue;
            slider.value = unit.currentHP;
            backSlider.value = unit.currentHP;
            text.text = $"{unit.currentHP} / {unit.maxHP}";
        }
        catch
        {

        }
       

    }
    void Update()
    {
        if (unit != null)
        {
            slider.value = unit.currentHP;
            text.text = $"{unit.currentHP} / {unit.maxHP}";
            if(unit.namePlate != null && unit.armor > 0)
            {
                healthbarImageComponent.sprite = ArmorSprite;
            }
            else
            {
                healthbarImageComponent.sprite = NormalSprite;
            }
        }
    }

    public void TakeDamage(int damage, Unit DamageSource, DamageType damageType, Action.ActionStyle actionStyle,bool IgnoresDEF = false)
    {
        //RunTracker.Instance.slayer = DamageSource;
        if (unit != null)
        {
            int TrueDamage = 0;
            backSlider.value = slider.value;

            if(unit.armor < 0)
            {
                unit.armor = 0;
            }

            damage = (int)(damage * unit.health.DamageModifier);


            if (IgnoresDEF)
                TrueDamage = damage;
            else
                TrueDamage = damage - unit.armor;

            if (TrueDamage < 0)
                TrueDamage = 0;

            unit.currentHP -= TrueDamage;

            if (unit.armor > 0)
            unit.armor -= damage;

            unit.namePlate.UpdateArmor(unit.armor);
            if (this != null)
            {
                this.gameObject.SetActive(true);
                StartCoroutine(DamagePopUp(TrueDamage, damageType, actionStyle));
                StartCoroutine(HandleSlider());
            }
        }
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

    private void HandleTypeDamage(DamageType damageType, TextMeshProUGUI number, int damage, Action.ActionStyle actionStyle)
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
            if (!TL.CanClear)
            {
                if (CombatTools.ReturnTypeMultiplier(unit, damageType) > 1 || actionStyle != Action.ActionStyle.STANDARD) //effective
                {
                    if (!unit.statusEffects.Contains(unit.statusEffects.Where(obj => obj.iconName == "STALWART").SingleOrDefault()) && Director.Instance.timeline.ReturnTimelineChild(unit) != null) //Applies Stun
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
                            if(actionStyle == Action.ActionStyle.LIGHT)
                            {
                                number.outlineColor = new Color(0, 0.635f, 0.749f);
                            }
                            else if (actionStyle == Action.ActionStyle.HEAVY)
                            {
                                number.outlineColor = new Color(1, 0.011f, 0);
                            }
                        }

                        if (TL.value <= 0)
                        {
                            Director.Instance.timeline.RemoveTimelineChild(unit);
                            BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "Stun", Color.yellow, Color.yellow, new Vector3(0, 2, 0f), Quaternion.identity, 15f, 0, true, 0, 2));
                            BattleSystem.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "Stun", Color.yellow, Color.yellow, new Vector3(0, 2, 0f), new Quaternion(0, 180, Quaternion.identity.z, Quaternion.identity.w),15f, 0, true, 0, 2));
                            BattleSystem.Instance.SetTempEffect(unit, "STALWART", false);
                        }
                    }
                    else if (unit.statusEffects.Contains(unit.statusEffects.Where(obj => obj.iconName == "STALWART").SingleOrDefault())) //Removes Stalwart
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
        }
       
    }

    private IEnumerator DamagePopUp(int damage,DamageType damageType, Action.ActionStyle actionStyle)
    {
        if (unit != null)
        {
            unit.DoOnDamaged();
            //Death
            if (unit.currentHP < 1)
            {
                var popup = Instantiate(damagePopUp, new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x, unit.GetComponent<SpriteRenderer>().bounds.center.y + 2, unit.transform.position.z), Quaternion.identity);
                var number = popup.GetComponentInChildren<TextMeshProUGUI>();
                try
                {
                    HandleTypeDamage(damageType, number, damage, actionStyle);

                }
                catch
                {
                    print("text isn't being found?");
                }
                StartCoroutine(popup.Pop());
                unit.Dying = true;
                if (unit.IsPlayerControlled)
                {
                    DeathPaused = true;
                    LabCamera.Instance.state = LabCamera.CameraState.IDLE;
                    BattleSystem.Instance.BattlePhasePause = true;
                    yield return new WaitForSeconds(1f);
                    Director.Instance.StartCoroutine(popup.DestroyPopUp());
                    unit.DoDeathQuote();
                    LabCamera.Instance.MoveToUnit(unit, Vector3.zero,0,8, -40, 0.5f);
                }
                else
                {
                    LabCamera.Instance.state = LabCamera.CameraState.IDLE;
                    LabCamera.Instance.MoveToUnit(unit, Vector3.zero,0,8, -40, 0.5f);
                }
                unit.DoOnPreDeath();
                if(unit.unitName == "Dusty" && BattleSystem.Instance.enemyUnits.Where(obj => obj.unitName == "Matriarch").SingleOrDefault())
                 LabCamera.Instance.uicam.gameObject.SetActive(false);
                yield return new WaitUntil(() => !DeathPaused);
                if (unit.unitName == "Dusty" && BattleSystem.Instance.enemyUnits.Where(obj => obj.unitName == "Matriarch").SingleOrDefault())
                    LabCamera.Instance.uicam.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.5f);
                Director.Instance.StartCoroutine(Tools.ChangeObjectEmissionToMaxIntensity(unit.gameObject, Color.yellow, 0.07f));
                unit.ChangeUnitsLight(unit.spotLight, 150, 15, Color.yellow, 0.04f, 0.1f);
                yield return new WaitForSeconds(0.7f);
                LabCamera.Instance.Shake(0.5f, 1f);
                Director.Instance.StartCoroutine(CombatTools.PlayVFX(unit.gameObject, "DeathBurst", Color.yellow, Color.yellow, Vector3.zero,  Quaternion.identity ,10, 0, false));
                yield return new WaitForSeconds(0.03f);
                if (popup != null)
                    Director.Instance.StartCoroutine(popup.DestroyPopUp());

                Director.Instance.StartCoroutine(Die());
            }
            else
            {

                unit.GetComponent<SpriteRenderer>().material.SetColor("_CharacterEmission", new Color(1f, 1f, 1f));
                var popup = Instantiate(damagePopUp, new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x, unit.GetComponent<SpriteRenderer>().bounds.center.y + 2, unit.transform.position.z), Quaternion.identity);
                var number = popup.GetComponentInChildren<TextMeshProUGUI>();
                try
                {
                    HandleTypeDamage(damageType, number, damage, actionStyle);

                }
                catch
                {
                    print("text isn't being found?");
                }
                StartCoroutine(popup.Pop());
                yield return new WaitForSeconds(0.2f);
                unit.GetComponent<SpriteRenderer>().material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f));
                yield return new WaitForSeconds(1f);
                Director.Instance.StartCoroutine(popup.DestroyPopUp());
            }
        }
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
            LabCamera.Instance.ResetPosition(true);
            print("Player should be dead");
        }
        else
        {
            BattleSystem.Instance.enemyUnits.Remove(unit);
            print("Enemy should be dead");
        }
        BattleSystem.Instance.numOfUnits.Remove(unit);
        Destroy(unit.ActionLayout);
        Destroy(unit.gameObject);
        CombatTools.TurnOffCriticalUI(unit);
        BattleSystem.Instance.BattlePhasePause = false;
        Director.Instance.StartCoroutine(Tools.LateUnpause());
    }


}
