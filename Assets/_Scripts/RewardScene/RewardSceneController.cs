using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSceneController : MonoBehaviour
{

    public List<PlayerReward> rewardPool;
    public RewardSceneUI ui;


    private List<PlayerReward> currentRewards;

    const int numRewards = 3;


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

        this.ui.InitializeRewards(this.currentRewards);
    }

    public void ApplyBuff(int index) {
        PlayerReward chosenReward = this.currentRewards[index];
        Player hero = GameManager.Instance.gameState.playerParty.GetFighters<Player>()[0];
        hero.setPermanentBuff(chosenReward);
    }
}
