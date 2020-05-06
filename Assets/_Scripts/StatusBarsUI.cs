using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(BattleController))]
public class StatusBarsUI : MonoBehaviour
{
    private BattleController _controller{get; set;}

    [Header("References")]
    public RectTransform playerStatusBarParent;

    public RectTransform enemyStatusBarParent;

    public StatusBarUI statusBarPrefab;



    private List<StatusBarUI> statusBars;
    private List<StatusBarUI> enemyStatusBars;

    void Awake() {
        _controller = GetComponent<BattleController>();
    }

    public void Initialize() {
        int playerCount = _controller.stage.GetPlayers().Count;

        statusBars = new List<StatusBarUI>();

        for (int i = 0; i < playerCount; i++) { 
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.parent = playerStatusBarParent;
            statusBars.Add(statusBarForPlayer);
        }

        enemyStatusBars = new List<StatusBarUI>();

        int enemyCount = _controller.stage.GetEnemies().Count;

        for (int i = 0; i < enemyCount; i++) {
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.parent = enemyStatusBarParent;
            enemyStatusBars.Add(statusBarForPlayer);
        }

        this.UpdateStatusBarUI();
    }

    public void UpdateStatusBarUI() {
        List<Player> players = _controller.stage.GetPlayers();
        for (int i = 0; i < players.Count; i++) {
            statusBars[i].SetName(players[i].Name);
            statusBars[i].SetHP(players[i].stats.GetHp(), players[i].stats.maxHp);
        }

        List<Enemy> enemies = _controller.stage.GetEnemies();

        for (int i = 0; i < enemies.Count; i++) {
            enemyStatusBars[i].SetName(enemies[i].Name);
            enemyStatusBars[i].SetHP(enemies[i].stats.GetHp(), enemies[i].stats.maxHp);
        }
    }

  

}
