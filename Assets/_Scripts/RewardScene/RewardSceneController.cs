using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RewardRarity {
    COMMON,
    RARE,
    EPIC,
    LEGENDARY
};


// Wrappers to show properly in the inspector
[System.Serializable]
public class RewardRarityWrapper {
    public RewardRarity rewardRarity;
}

[System.Serializable]
public class  ValueWeightRewardRarity : ValueWeight<RewardRarityWrapper> {

}

[System.Serializable]
public class  ValueWeightReward : ValueWeight<PlayerReward> {

}


public class RewardSceneController : MonoBehaviour
{
    public List<ValueWeightRewardRarity> rewardRarityPool;
    public List<PlayerReward> rewardPool;
    // public List<ValueWeightReward> rewardPool;
    public RewardSceneUI ui;

    public SceneChanger sceneChanger;


    private List<PlayerReward> currentRewards;

    const int numRewards = 3;

    private PlayerReward selectedReward;

    private bool canContinue = false;


    void Start()
    {
        this.InitializeRewards();        
    }

    void Update()
    {
        
    }

    public void InitializeRewards() {
        this.currentRewards = new List<PlayerReward>();
        List<PlayerReward> rewards = rewardPool.CreateNRandom(numRewards);
        foreach (PlayerReward reward in rewards) {
            PlayerReward rewardInstance = Instantiate(reward);
            rewardInstance.Initialize();
            this.currentRewards.Add(rewardInstance);
        }

        this.ui.InitializeRewards(this.currentRewards, this.SelectReward);
    }

    public void SelectReward(PlayerReward reward) {

        this.selectedReward = reward;
        this.ui.OnSelectRewardUI(reward);
        if (!canContinue) {
            StartCoroutine(WaitForInput());
        }
    }

    public void ApplyReward(PlayerReward reward) {
        switch (reward.rewardType) {
            case PlayerRewardType.AILMENT:
                Player hero = GameManager.Instance.gameState.playerParty.GetHeroFighter();
                hero.AddPermanentReward(reward);
            break;

            default:
            break;
        }
    }


    private IEnumerator WaitForInput() {
        canContinue = true;
        while (true) {
            if (Input.GetKeyDown(KeyCode.Z)) {
                this.ApplyReward(this.selectedReward);
                this.sceneChanger.ChangeScene();
                yield break;
            }

            yield return null;
        }
    }

    
}
