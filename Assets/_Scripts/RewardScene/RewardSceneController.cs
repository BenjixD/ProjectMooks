using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSceneController : MonoBehaviour
{

    public AffinityController affinityController;
    public RewardController rewardController;

    public Transform affinityParent;
    public Transform rewardParent;



    // Start is called before the first frame update
    void Start()
    {
        this.affinityParent.gameObject.SetActive(false);
        this.rewardParent.gameObject.SetActive(false);

        this.InitializeAffinityController();
    }

    public void InitializeAffinityController() {
        this.affinityParent.gameObject.SetActive(true);
        affinityController.Initialize(this.InitializeRewardController);
    }

    public void InitializeRewardController() {
        this.affinityParent.gameObject.SetActive(false);
        this.rewardParent.gameObject.SetActive(true);
        rewardController.Initialize();
    }

}
