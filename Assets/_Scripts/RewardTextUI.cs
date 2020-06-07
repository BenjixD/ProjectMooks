using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RewardTextUI : MonoBehaviour
{
    public TextMeshProUGUI header;
    public TextMeshProUGUI body;

    public PlayerReward reward {get; set;}

    private Action<PlayerReward> callback;

    public void Initialize(string header, string body, PlayerReward reward, Action<PlayerReward> callback) {
        this.header.SetText(header);
        this.body.SetText(body);
        this.reward = reward;
        this.callback = callback;
    }

    public void Select() {
        this.callback(this.reward);
    }
}
