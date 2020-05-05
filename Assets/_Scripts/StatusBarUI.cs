using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{

    public Text nameText;
    public Text hpText;
    public Text energyText;

    //TODO: Progress bar

    public void SetName(string name) {
        if (nameText == null) {
            return;
        }

        nameText.text = name;
    }


    public void SetHP(int hp, int maxHP) {
        if (hpText == null) {
            return;
        }

        hpText.text = hp + "/" + maxHP;
    }

    public void SetEnergy(int energy, int maxEnergy) {
        if (energyText == null) {
            return;
        }

        energyText.text = energy + "/" + maxEnergy;
    }
}
