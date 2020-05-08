﻿using System.Collections;
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

    }

    void OnDestroy() {
        Messenger.RemoveListener<Player>(Messages.OnPlayerJoinBattle, this.onPlayerJoin);
        Messenger.RemoveListener<FightResult>(Messages.OnFightEnd, this.onFightEnd);
    }

    public void Initialize() {
        Messenger.AddListener<Player>(Messages.OnPlayerJoinBattle, this.onPlayerJoin);
        Messenger.AddListener<FightResult>(Messages.OnFightEnd, this.onFightEnd);

        this.UpdateStatusBars();
    }

    public void UpdateStatusBars() {
        List<Player> players = _controller.stage.GetActivePlayers();
        List<Enemy> enemies = _controller.stage.GetActiveEnemies();

        if (statusBars == null || enemyStatusBars == null || players.Count != statusBars.Count || enemies.Count != enemyStatusBars.Count) {
            this.rebuildStatusBars();
        }

        this.setStatusBarUI();
    }

    private void rebuildStatusBars() {
        this.DestroyCurrentStatusBars();
        int playerCount = _controller.stage.GetActivePlayers().Count;

        statusBars = new List<StatusBarUI>();

        for (int i = 0; i < playerCount; i++) { 
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.SetParent(playerStatusBarParent);
            statusBars.Add(statusBarForPlayer);
        }

        enemyStatusBars = new List<StatusBarUI>();

        int enemyCount = _controller.stage.GetActiveEnemies().Count;

        for (int i = 0; i < enemyCount; i++) {
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.SetParent(enemyStatusBarParent);
            enemyStatusBars.Add(statusBarForPlayer);
        }
    }

    private void setStatusBarUI() {
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

    private void onPlayerJoin(Player player) {
        this.UpdateStatusBars();
    }

    private void onFightEnd(FightResult result) {
        this.UpdateStatusBars();
    }

    private void DestroyCurrentStatusBars() {
        if (statusBars != null) {
            foreach (StatusBarUI statusBar in statusBars) {
                Destroy(statusBar.gameObject);
            }
            this.statusBars.Clear();
        }

        if (enemyStatusBars != null) {
            foreach (StatusBarUI statusBar in enemyStatusBars) {
                Destroy(statusBar.gameObject);
            }
            this.enemyStatusBars.Clear();
        }

    }
}