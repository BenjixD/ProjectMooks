using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatRewardUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Button button;

    private PlayerStats playerStats;
    private PlayerStatWithModifiers theStat;
    private int value;
    private int cost;
    
    private Action<PlayerStatWithModifiers> callback; 

    public void Initialize(PlayerStats playerStats, Stat stat, Action<PlayerStatWithModifiers> callback) {
        this.playerStats = playerStats;
        this.theStat = (PlayerStatWithModifiers)playerStats.GetStat(stat);
        this.callback = callback;
        this.UpdateUIValues();
    }

    public void AddStat() {
        if (this.callback != null) {
            this.callback(theStat);
        }
    }


    public void UpdateUIValues() {
        this.value = theStat.GetBaseValue();
        this.cost = StatLevelHelpers.GetCostToLevelUpStat(theStat.GetBaseValue(), theStat.divisor);
        text.SetText(theStat.stat.ToString() + ": " + this.value + " | " + "COST: " + this.cost );

        if (this.playerStats.statPoints < this.cost) {
            this.button.interactable = false;
        } else {
            this.button.interactable = true;
        }
    }
}
