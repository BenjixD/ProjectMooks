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

    public void SetHP(int hp, int maxHP) {
        this.SetTextValueFraction(hpText, hp, maxHP);
        this.SetSliderValue(this.hpBar, hp, maxHP);
    }


    // TODO: Trigger status ailment icon using these functions upon receiving the status effect
    public void AddStatusAilmentIcon(StatusAilmentIcon prefab, StatusAilment ailment) {
        StatusAilmentIcon statusIcon = Instantiate<StatusAilmentIcon>(prefab, statusConditionParent);
        statusIcon.SetStatusAilment(ailment);
        this.statusConditionIcons.Add(statusIcon);
    }

    public void RemoveStatusAilmentIcon(StatusAilment ailment) {
        for (int i = 0; i < statusConditionIcons.Count; i++) {
            if (statusConditionIcons[i].statusAilment == ailment) {
                Destroy(statusConditionIcons[i].gameObject);
                this.statusConditionIcons.RemoveAt(i);
                break;
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
