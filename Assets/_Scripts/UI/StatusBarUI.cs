using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusBarUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public Slider hpBar;
    

    public Transform statusConditionParent;

    protected FightingEntity fighter;

    public StatusAilmentIcon statusIconPrefab;
    protected List<StatusAilmentIcon> statusConditionIcons = new List<StatusAilmentIcon>();

    public virtual void SetFighter(FightingEntity fighter) {
        this.fighter = fighter;
    }

    // TODO: Instead of setting the max amount each time, maybe consider making an initialize function instead
    public void SetName(string name) {
        if (nameText == null) {
            return;
        }

        nameText.SetText(name);
    }

    public string GetName() {
        if (nameText == null) {
            return "";
        }

        return nameText.text;
    }

    public void SetNameColor(Color color) {
        if (nameText == null) {
            return;
        }

        nameText.color = color;
    }

    public void SetHP(int hp, int maxHP) {
        this.SetTextValueFraction(hpText, hp, maxHP);
        this.SetSliderValue(this.hpBar, hp, maxHP);
    }

    public void SetStatusAilmentIcons(List<StatusAilment> ailments) {
        int iconsSet = 0;
        for (int i = 0; i < ailments.Count; i++) {
            if (ailments[i].icon != null) {
                // Replace existing icon if one exists, otherwise, create new icon
                if (iconsSet < statusConditionIcons.Count) {
                    statusConditionIcons[iconsSet].SetStatusAilment(ailments[i]);
                    iconsSet++;
                } else {
                    StatusAilmentIcon statusIcon = Instantiate<StatusAilmentIcon>(statusIconPrefab, statusConditionParent);
                    statusIcon.SetStatusAilment(ailments[i]);
                    statusConditionIcons.Add(statusIcon);
                    iconsSet++;
                }
            }
        }

        // Clean extraneous icons
        for (int i = statusConditionIcons.Count - 1; i >= iconsSet; i--) {
            Destroy(statusConditionIcons[i].gameObject);
            statusConditionIcons.RemoveAt(i);
        }
    }

    protected void SetSliderValue(Slider slider, float curValue, float maxValue) {
        if (slider != null) {
            slider.value = curValue / maxValue;
        }
    }

    protected void SetTextValueFraction(TextMeshProUGUI text, float curValue, float maxValue) {
        if (text != null) {
            text.SetText(curValue + "/" + maxValue);
        }
    }
}
