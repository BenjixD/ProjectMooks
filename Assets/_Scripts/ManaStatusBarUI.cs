using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatusBar : StatusBarUI
{

    public Slider manaBar;
    public Text manaText;

    public void SetMana(int mana, int maxMana) {
        this.SetSliderValue(this.manaBar, mana, maxMana);
        this.SetTextValueFraction(this.manaText, mana, maxMana);
    }

}
