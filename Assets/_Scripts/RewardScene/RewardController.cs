﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
    public List<ValueWeightReward> rewardWeights;
}

[System.Serializable]
public class  ValueWeightReward : ValueWeight<PlayerReward> {
}


public class RewardController : MonoBehaviour
{
    public List<ValueWeightRewardRarity> rewardRarityWeights;
    public RewardControllerUI ui;



    private List<PlayerReward> currentRewards;

    const int numRewards = 3;

    private PlayerReward selectedReward;

    private bool canContinue = false;

    // rarity => playerRewardPool
    //private Dictionary<RewardRarity, RandomPool<PlayerReward>> rewardPools;
    private RandomPool<PlayerReward> finalRewardPool;
    private Action callback;

    public void Initialize(Action callback)
    {
        this.callback = callback;
        this.InitializeRewards();
    }

    public void InitializeRewards() {
        this.currentRewards = new List<PlayerReward>();
        Player hero = GameManager.Instance.gameState.playerParty.GetHeroFighter();

        RewardAffinity affinity = GameManager.Instance.gameState.progressData.affinity;

        List<ValueWeight<PlayerReward>> allRewards = new List<ValueWeight<PlayerReward>>();

        float weightSumForErrorCheck = 0;
        foreach (ValueWeightRewardRarity rewardRarity in this.rewardRarityWeights) {
            weightSumForErrorCheck += rewardRarity.weight;
        }

        if (weightSumForErrorCheck != 1) {
            // Note: Technically we can check this in the code by making it into a pool and normalizing it, but if it doesn't add to 1,
            // it probably isn't intended 
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

            Debug.Log("Number of weights in pool " + rarity.ToString() + " " + rewardWeightsCasted.Count());

            RandomPool<PlayerReward> rewardPool = new RandomPool<PlayerReward>(rewardWeightsCasted);

            rewardPool.ForAll( (ValueWeight<PlayerReward> reward) => {


                float normalWeight = 1/(float)rewardWeightsCasted.Count();

                if (reward.value.isUnique && hero.playerRewards.Find((PlayerReward re) => re.name == reward.value.name ) != null) {
                    reward.weight = 0;
                    Debug.Log("Weight changed from: " + normalWeight + " to: " + reward.weight);
                    return;
                }

                reward.weight = normalWeight;
                if (affinity != RewardAffinity.DEFAULT && reward.value.affinityTypes.Contains(affinity)) {
                    reward.weight *= 2;
                    Debug.Log("Weight changed from: " + normalWeight + " to: " + reward.weight);
                }
            });

            rewardPool.ForAll( reward =>  reward.weight *= rewardRarity.weight );

            allRewards.AddRange(rewardPool.GetPoolWeights());
        }


        Debug.Log("Rewards:");
        for (int i = 0; i < allRewards.Count; i++) {
            Debug.Log(allRewards[i].value.name + " " + allRewards[i].weight);
        }

    

        finalRewardPool = new RandomPool<PlayerReward>(allRewards);
        
        

        List<PlayerReward> rewards = finalRewardPool.PickN(numRewards);

        foreach (PlayerReward reward in rewards) {
            PlayerReward rewardInstance = Instantiate(reward);
            rewardInstance.Initialize(GameManager.Instance.gameState.playerParty.GetHeroFighter());
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

        Player hero = GameManager.Instance.gameState.playerParty.GetHeroFighter();
        hero.playerRewards.Add(reward);

        switch (reward.rewardType) {
            case PlayerRewardType.AILMENT:
                hero.ailmentController.AddStatusAilment(((PlayerRewardAilment)reward).ailment);
            break;
            case PlayerRewardType.ACTION:
                hero.actions.Add(((PlayerRewardAction)reward).action);
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
                this.callback();
                yield break;
            }

            yield return null;
        }
    }

    
}
