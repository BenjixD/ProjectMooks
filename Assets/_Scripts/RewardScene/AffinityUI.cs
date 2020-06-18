using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AffinityUI : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI nameText;
    
    private Action<RewardAffinity> callback;
    
    private RewardAffinity affinity;

    public void Initialize(RewardAffinity affinity, Action<RewardAffinity> callback) {
        nameText.SetText(AffinityUI.AffinityToString(affinity));
        this.callback = callback;
        this.affinity = affinity;
    }

    public void SelectAffinity() {
        this.callback(this.affinity);
    }

    public static string AffinityToString(RewardAffinity affinity) {

        string affin = "";
        switch (affinity) {
            case RewardAffinity.DEFAULT:
                affin = "Default";
                break;
            case RewardAffinity.ABILITY_FOCUS:
                affin = "Ability";
                break;
            case RewardAffinity.MOOK_FOCUS:
                affin = "Mooks";
                break;
            case RewardAffinity.SPECIAL_FOCUS:
                affin = "Special";
                break;
            case RewardAffinity.STAT_FOCUS:
                affin = "Stat";
                break;
            default:
                affin = "Default";
                break;
        }

        return affin;
    }
}
