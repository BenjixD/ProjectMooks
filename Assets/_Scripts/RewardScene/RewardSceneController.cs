﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum RewardRarity {
    COMMON,
    RARE,
    EPIC,
    LEGENDARY
};

public enum RewardAffinity {
    DEFAULT,
    STAT_FOCUS,
    ABILITY_FOCUS,
    SPECIAL_FOCUS,
    MOOK_FOCUS
};


// Wrappers to show properly in the inspector
[System.Serializable]
public class RewardRarityWrapper {
    public RewardRarity rewardRarity;
}

[System.Serializable]
public class  ValueWeightRewardRarity : ValueWeight<RewardRarityWrapper> {
    public List<ValueWeightReward> rewardWeights;
}

[System.Serializable]
public class  ValueWeightReward : ValueWeight<PlayerReward> {
}


public class RewardSceneController : MonoBehaviour
{
    public List<ValueWeightRewardRarity> rewardRarityWeights;
    public RewardSceneUI ui;

    public SceneChanger sceneChanger;


    private List<PlayerReward> currentRewards;

    const int numRewards = 3;

    private PlayerReward selectedReward;

    private bool canContinue = false;

    // rarity => playerRewardPool
    //private Dictionary<RewardRarity, RandomPool<PlayerReward>> rewardPools;
    private RandomPool<PlayerReward> finalRewardPool;

    void Start()
    {
        this.InitializeRewards();        
    }

    void Update()
    {
        
    }


    public void InitializeRewards() {
        this.currentRewards = new List<PlayerReward>();

        RewardAffinity affinity = GameManager.Instance.gameState.progressData.affinity;

        List<ValueWeight<PlayerReward>> allRewards = new List<ValueWeight<PlayerReward>>();

        float weightSumForErrorCheck = 0;
        foreach (ValueWeightRewardRarity rewardRarity in this.rewardRarityWeights) {
            weightSumForErrorCheck += rewardRarity.weight;
        }

        if (weightSumForErrorCheck != 1) {
            Debug.LogError("ERROR: Rarity pool weights should add to 1 or else they might not work properly!");
            return;
        }

        foreach (ValueWeightRewardRarity rewardRarity in this.rewardRarityWeights) {
            weightSumForErrorCheck += rewardRarity.weight;

            RewardRarity rarity = rewardRarity.value.rewardRarity;

            List<ValueWeight<PlayerReward>> rewardWeightsCasted = rewardRarity.rewardWeights.Cast<ValueWeight<PlayerReward>>().ToList();
            if (rewardWeightsCasted.Count() == 0) {
                Debug.LogError("Zero rewards in pool!");
                continue;
            }

            RandomPool<PlayerReward> rewardPool = new RandomPool<PlayerReward>(rewardWeightsCasted);

            // Note: ForAll normalizes weights afterwards
            rewardPool.ForAll( (ValueWeight<PlayerReward> reward) => {
                float normalWeight = 1/(float)rewardWeightsCasted.Count();
                reward.weight = normalWeight;
                if (affinity != RewardAffinity.DEFAULT && reward.value.affinityTypes.Contains(affinity)) {
                    reward.weight *= 2;
                }
            });

            rewardPool.ForAll( reward => reward.weight *= rewardRarity.weight );

            allRewards.AddRange(rewardPool.GetPoolWeights());
        }

        finalRewardPool = new RandomPool<PlayerReward>(allRewards);
        
        List<PlayerReward> rewards = finalRewardPool.PickN(numRewards);

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