using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSceneUI : MonoBehaviour
{
    public Transform rewardParent;

    public BasicText rewardUIPrefab;

    public void InitializeRewards(List<PlayerReward> rewardDescriptions) {
        foreach (PlayerReward rewardDescription in rewardDescriptions) {
            BasicText rewardUI = Instantiate<BasicText>(rewardUIPrefab, rewardParent);
            rewardUI.text.SetText(rewardDescription.description);
            // TODO: set name as well...
        }
    }
}
