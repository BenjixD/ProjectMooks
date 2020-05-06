using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarsUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform playerStatusBarParent;

    public RectTransform enemyStatusBarParent;

    public StatusBarUI statusBarPrefab;



    private List<StatusBarUI> statusBars;
    private List<StatusBarUI> enemyStatusBars;

    public void Initialize() {
        int playerCount = GameManager.Instance.battleController.stage.GetPlayers().Count;

        statusBars = new List<StatusBarUI>();

        for (int i = 0; i < playerCount; i++) { 
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.parent = playerStatusBarParent;
            statusBars.Add(statusBarForPlayer);
        }

        enemyStatusBars = new List<StatusBarUI>();

        int enemyCount = GameManager.Instance.battleController.stage.GetEnemies().Count;

        for (int i = 0; i < enemyCount; i++) {
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.parent = enemyStatusBarParent;
            enemyStatusBars.Add(statusBarForPlayer);
        }

        this.UpdateStatusBarUI();
    }

    public void UpdateStatusBarUI() {
        List<Player> players = GameManager.Instance.battleController.stage.GetPlayers();
        for (int i = 0; i < players.Count; i++) {
            statusBars[i].SetName(players[i].Name);
            statusBars[i].SetHP(players[i].stats.GetHp(), players[i].stats.maxHp);
        }

        List<Enemy> enemies = GameManager.Instance.battleController.stage.GetEnemies();

        for (int i = 0; i < enemies.Count; i++) {
            enemyStatusBars[i].SetName(enemies[i].Name);
            enemyStatusBars[i].SetHP(enemies[i].stats.GetHp(), enemies[i].stats.maxHp);
        }
    }

  

}
