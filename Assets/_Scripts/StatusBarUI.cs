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
        // Replace any existing icons with passed in icons
        int iconsToReplace = Mathf.Min(ailments.Count, statusConditionIcons.Count);
        for (int i = 0; i < iconsToReplace; i++) {
            statusConditionIcons[i].SetStatusAilment(ailments[i]);
        }

        if (ailments.Count < statusConditionIcons.Count) {
            // Remove remaining icons
            for (int i = statusConditionIcons.Count - 1; i >= ailments.Count; i--) {
                Destroy(statusConditionIcons[i].gameObject);
                statusConditionIcons.RemoveAt(i);
            }
        } else if (ailments.Count > statusConditionIcons.Count) {
            // Create new icons as needed
            for (int i = iconsToReplace; i < ailments.Count; i++) {
                StatusAilmentIcon statusIcon = Instantiate<StatusAilmentIcon>(statusIconPrefab, statusConditionParent);
                statusIcon.SetStatusAilment(ailments[i]);
                statusConditionIcons.Add(statusIcon);
            }
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
