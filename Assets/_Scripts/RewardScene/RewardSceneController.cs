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
        this.currentRewards = currentRewards.CreateNRandom(numRewards);
        this.ui.InitializeRewards(this.currentRewards);
    }



    public void ApplyBuff(int index) {
        PlayerReward chosenReward = this.currentRewards[index];
        Player hero = GameManager.Instance.gameState.playerParty.GetFighters<Player>()[0];
        hero.setPermanentBuff(chosenReward);
    }
}
