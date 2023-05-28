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
                var sprite = unit.GetComponent<SpriteRenderer>();
                yield return new WaitForSeconds(0.5f);
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
