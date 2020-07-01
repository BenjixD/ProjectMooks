using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSceneController : MonoBehaviour
{

    public AffinityController affinityController;
    public RewardController rewardController;
    public StatRewardController statDistributionController;

    public Transform affinityParent;
    public Transform rewardParent;
    public Transform statDistributionParent;

    public SceneChanger sceneChanger;


    private List<Transform> rewardSlides = new List<Transform>();
    int slideIndex = 0;



    // Start is called before the first frame update
    void Start()
    {
        rewardSlides.Add(affinityParent);
        rewardSlides.Add(rewardParent);
        rewardSlides.Add(statDistributionParent);
        slideIndex = 0;

        foreach (var slide in rewardSlides) {
            slide.gameObject.SetActive(false);
        }

        this.InitializeSlide();
        this.rewardSlides[0].gameObject.SetActive(true);
    }

    private void AdvanceSlide() {
        rewardSlides[slideIndex].gameObject.SetActive(false);
        slideIndex++;

        this.InitializeSlide();
    }

    private void InitializeSlide() {
        Transform curSlide = rewardSlides[slideIndex];
        if (curSlide == affinityParent) {
            this.InitializeAffinityController();
        } else if (curSlide == rewardParent) {
            this.InitializeRewardController();
        } else if (curSlide == statDistributionParent) {
            this.InitializeStatDistributionController();
        }

        curSlide.gameObject.SetActive(true);
    }

    public void InitializeAffinityController() {
        this.affinityController.Initialize(this.AdvanceSlide);
    }

    public void InitializeRewardController() {
        this.rewardController.Initialize(this.AdvanceSlide);
    }


    public void InitializeStatDistributionController() {
        this.statDistributionController.Initialize(this.ChangeScene);
    }


    public void ChangeScene() {
        this.sceneChanger.ChangeScene();
    }
}
