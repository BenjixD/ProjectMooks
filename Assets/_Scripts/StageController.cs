
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public void LoadNextStage() {
        this.Unload();
        GameManager.Instance.gameState.SetNextStageIndex(GameManager.Instance.gameState.progressData.currentStageIndex + 1); // May want to change logic in the future
        SceneManager.LoadScene("RewardScene");
    }

    public void LoadDeathScene() {
        this.Unload();
        SceneManager.LoadScene("GameOverScreen");
    }

    private void Unload() {
        GameManager.Instance.gameState.enemyParty.ClearFighters();
    }
}