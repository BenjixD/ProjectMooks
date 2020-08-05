using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainEvent : EventStage {

    protected override void InitializeContinueButton() {
        _continueButton.GetComponent<Button>().onClick.AddListener(GoToRewardScene);
    }

    private void GoToRewardScene() {
        SceneManager.LoadScene("RewardScene");
        Destroy(gameObject);
    }
}
