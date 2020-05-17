using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaStatusBarUI : StatusBarUI
{

    public Slider manaBar;
    public TextMeshProUGUI manaText;

    public void SetMana(int mana, int maxMana) {
        this.SetSliderValue(this.manaBar, mana, maxMana);
        this.SetTextValueFraction(this.manaText, mana, maxMana);
    }

}
