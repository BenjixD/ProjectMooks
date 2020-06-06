using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSceneUI : MonoBehaviour
{
    public Transform rewardParent;

    public DuoText rewardUIPrefab;

    public void InitializeRewards(List<PlayerReward> rewardDescriptions) {
        foreach (PlayerReward rewardDescription in rewardDescriptions) {
            DuoText rewardUI = Instantiate<DuoText>(rewardUIPrefab, rewardParent);
            rewardUI.header.SetText(rewardDescription.name + ":");
            rewardUI.body.SetText(rewardDescription.description);
        }
    }
}
