
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public void LoadNextStage() {
        GameManager.Instance.gameState.SetNextStageIndex(GameManager.Instance.gameState.progressData.currentStageIndex + 1); // May want to change logic in the future
        SceneManager.LoadScene("WorldMap");
    }

    public void LoadDeathScene() {
        SceneManager.LoadScene("GameOverScreen");
    }
}