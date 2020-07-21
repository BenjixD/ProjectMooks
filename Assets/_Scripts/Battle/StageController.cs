
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public void LoadNextScene() {
        this.Unload();
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