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
    public GameObject damagePopUp;
    //public TextMeshProUGUI namePlate;
    public Slider backSlider;
    //public Image DEF_icon;
    //public TextMeshProUGUI defText;
    //public StaminaBar stamina;
    

     void Start()
    {
        slider.maxValue = unit.maxHP;
        backSlider.maxValue = slider.maxValue;
        slider.value = unit.currentHP;
        slider.value = unit.currentHP;
        text.text = $"{unit.currentHP} / {unit.maxHP}";
        //namePlate.text = unit.unitName;
          if (!unit.IsPlayerControlled)
            {
                //DEF_icon.gameObject.SetActive(true);
                //defText.text = $"{unit.defenseStat}";
            }
            else
            {
                //DEF_icon.gameObject.SetActive(false);
            }
        
    }
    void Update()
    {
        if (unit != null)
        {
            slider.value = unit.currentHP;
            text.text = $"{unit.currentHP} / {unit.maxHP}";
            //namePlate.text = unit.unitName;

            /*if (DEF_icon != null)
            {
                defText.text = $"{unit.defenseStat}";
            }
            */
           
        }
    }


    public void TakeDamage(int damage)
    {
        if (unit != null)
        {
            var truedamage = damage - unit.defenseStat;
            if (truedamage < 1)
            {
                truedamage = 1;
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
        var BSP = unit.GetComponentInParent<BattleSpawnPoint>();
        BSP.Occupied = false;
        BSP.unit = null;
        if (unit.IsPlayerControlled)
        {         
            battlesystem.playerUnits.Remove(unit);
            print("Player should be dead");
        }
        else
        {
            battlesystem.enemyUnits.Remove(unit);
            print("Enemy should be dead");
        }
        battlesystem.numOfUnits.Remove(unit);
        Tools.TurnOffCriticalUI(unit);
        LabCamera.Instance.ReadjustCam();
        Destroy(unit.gameObject);
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
            if (unit.anim != null)
                unit.anim.Play("Hurt");
          
            if (unit.currentHP < 1)
            {
                var popup = Instantiate(damagePopUp, new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x - 1.5f, unit.GetComponent<SpriteRenderer>().bounds.max.y + 4, unit.transform.position.z), Quaternion.identity);
                var img = popup.GetComponentInChildren<Image>();
                img.gameObject.SetActive(false);
                var number = popup.GetComponentInChildren<TextMeshProUGUI>();
                try
                {
                    number.SetText(damage.ToString());
                    number.outlineColor = Color.black;
                    number.outlineWidth = 0.2f;

                }
                catch
                {
                    print("text isn't being found?");
                }

                StartCoroutine(Tools.SmoothMove(popup, 0.001f, 20, 0, 0.005f));
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(Tools.SmoothMove(popup, 0.001f, 40, 0, -0.005f));

                var sprite = unit.GetComponent<SpriteRenderer>();
                sprite.forceRenderingOff = true;
                yield return new WaitForSeconds(0.1f);
                sprite.forceRenderingOff = false;
                yield return new WaitForSeconds(0.1f);
                sprite.forceRenderingOff = true;
                yield return new WaitForSeconds(0.1f);
                sprite.forceRenderingOff = false;
                yield return new WaitForSeconds(0.1f);
                sprite.forceRenderingOff = true;
                yield return new WaitForSeconds(0.1f);
                sprite.forceRenderingOff = false;
                yield return new WaitForSeconds(0.1f);
                sprite.forceRenderingOff = true;
                yield return new WaitForSeconds(0.1f);
                Die();
                Destroy(popup);
            }
            else
            {

                unit.GetComponent<SpriteRenderer>().material.SetColor("_CharacterEmission", new Color(1f, 1f, 1f));     
                var popup = Instantiate(damagePopUp, new Vector3(unit.GetComponent<SpriteRenderer>().bounds.center.x - 1.5f, unit.GetComponent<SpriteRenderer>().bounds.max.y + 4, unit.transform.position.z), Quaternion.identity);
                var img = popup.GetComponentInChildren<Image>();
                img.gameObject.SetActive(false);
                var number = popup.GetComponentInChildren<TextMeshProUGUI>();
                try
                {
                    number.SetText(damage.ToString());
                    number.outlineColor = Color.black;
                    number.outlineWidth = 0.2f;

                }
                catch
                {
                    print("text isn't being found?");
                }
                StartCoroutine(Tools.SmoothMove(popup, 0.001f, 20, 0, 0.005f));
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(Tools.SmoothMove(popup, 0.001f, 40, 0, -0.005f));
                unit.GetComponent<SpriteRenderer>().material.SetColor("_CharacterEmission", new Color(0f, 0f, 0f));
                yield return new WaitForSeconds(1.4f);
                Destroy(popup);
            }
        }
    }


}
