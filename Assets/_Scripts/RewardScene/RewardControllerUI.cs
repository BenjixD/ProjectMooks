using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RewardControllerUI : MonoBehaviour
{
    public Transform rewardParent;

    public RewardTextUI rewardUIPrefab;

    public BasicText selectedRewardText;

    public BasicText continueText;

    public void InitializeRewards(List<PlayerReward> rewardDescriptions, Action<PlayerReward> callback) {
        foreach (PlayerReward rewardDescription in rewardDescriptions) {
            RewardTextUI rewardUI = Instantiate<RewardTextUI>(rewardUIPrefab, rewardParent);
            string header = rewardDescription.name + ":";
            string body = rewardDescription.description;
            rewardUI.Initialize(header, body, rewardDescription, callback);
        }

        this.selectedRewardText.transform.SetAsLastSibling();
    }


    public void OnSelectRewardUI(PlayerReward reward) {
        this.selectedRewardText.text.SetText("Reward Selected: " + reward.name);
        this.continueText.text.SetText("Press Z to continue");
    }
}
