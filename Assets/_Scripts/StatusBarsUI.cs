using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(TurnController))]
public class StatusBarsUI : MonoBehaviour
{
    private TurnController _controller{get; set;}

    [Header("References")]
    public RectTransform playerStatusBarParent;

    public RectTransform enemyStatusBarParent;

    public StatusBarUI statusBarPrefab;



    private List<StatusBarUI> statusBars;
    private List<StatusBarUI> enemyStatusBars;

    void Awake() {
        _controller = GetComponent<TurnController>();
        Messenger.AddListener<Player>(Messages.OnPlayerJoinBattle, this.onPlayerJoin);
    }

    void OnDestroy() {
        Messenger.RemoveListener<Player>(Messages.OnPlayerJoinBattle, this.onPlayerJoin);
    }

    public void Initialize() {
        this.DestroyCurrentStatusBars();
        int playerCount = _controller.stage.GetActivePlayers().Count;

        statusBars = new List<StatusBarUI>();

        for (int i = 0; i < playerCount; i++) { 
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.parent = playerStatusBarParent;
            statusBars.Add(statusBarForPlayer);
        }

        enemyStatusBars = new List<StatusBarUI>();

        int enemyCount = _controller.stage.GetActiveEnemies().Count;

        for (int i = 0; i < enemyCount; i++) {
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.parent = enemyStatusBarParent;
            enemyStatusBars.Add(statusBarForPlayer);
        }

        this.UpdateStatusBarUI();
    }


    public void UpdateStatusBarUI() {
        List<Player> players = _controller.stage.GetActivePlayers();
        for (int i = 0; i < players.Count; i++) {
            statusBars[i].SetName(players[i].Name);
            statusBars[i].SetHP(players[i].stats.GetHp(), players[i].stats.maxHp);
        }

        List<Enemy> enemies = _controller.stage.GetActiveEnemies();

        for (int i = 0; i < enemies.Count; i++) {
            enemyStatusBars[i].SetName(enemies[i].Name);
            enemyStatusBars[i].SetHP(enemies[i].stats.GetHp(), enemies[i].stats.maxHp);
        }
    }

    public void onPlayerJoin(Player player) {
        this.Initialize();
    }

    private void DestroyCurrentStatusBars() {
        if (statusBars != null) {
            foreach (var statusBar in statusBars) {
                Destroy(statusBar);
            }
            this.statusBars.Clear();
        }

        if (enemyStatusBars != null) {
            foreach (var statusBar in enemyStatusBars) {
                Destroy(statusBar);
            }
            this.enemyStatusBars.Clear();
        }

    }
  

}
