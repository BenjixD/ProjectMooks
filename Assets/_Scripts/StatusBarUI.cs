using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{

    public Text nameText;
    public Text hpText;
    public Slider hpBar;


    public void SetName(string name) {
        if (nameText == null) {
            return;
        }

        nameText.text = name;
    }


    public void SetHP(int hp, int maxHP) {
        this.SetTextValueFraction(hpText, hp, maxHP);
        this.SetSliderValue(this.hpBar, hp, maxHP);
    }

    protected void SetSliderValue(Slider slider, float curValue, float maxValue) {
        if (slider != null) {
            slider.value = curValue / maxValue;
        }
    }

    protected void SetTextValueFraction(Text text, float curValue, float maxValue) {
        if (text != null) {
            text.text = curValue + "/" + maxValue;
        }
    }
}
