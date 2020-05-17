using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(TurnController))]
public class StatusBarsUI : MonoBehaviour
{
    // TODO: Might be useful to use array instead of Lists
    private TurnController _controller{get; set;}

    [Header("References")]
    public RectTransform playerStatusBarParent;

    // TODO: Instead of RectTransform, do it through FighterSlot
    public List<RectTransform> enemyStatusBarParent;

    public ManaStatusBarUI manaStatusBarPrefab;
    public MookStatusBarUI mookStatusBarPrefab;
    public EnemyStatusBarUI enemyStatusBarPrefab;



    private List<StatusBarUI> statusBars;
    private List<EnemyStatusBarUI> enemyStatusBars;

    void Awake() {
        _controller = GetComponent<TurnController>();

    }

    void OnDestroy() {
        Messenger.RemoveListener<List<Player>>(Messages.OnPlayersJoinBattle, this.onPlayerJoin);
        Messenger.RemoveListener<FightResult>(Messages.OnFightEnd, this.onFightEnd);
    }

    public void Initialize() {
        Messenger.AddListener<List<Player>>(Messages.OnPlayersJoinBattle, this.onPlayerJoin);
        Messenger.AddListener<FightResult>(Messages.OnFightEnd, this.onFightEnd);

        this.UpdateStatusBars();
    }

    public void UpdateStatusBars() {
        List<Player> players = _controller.field.playerParty.GetActiveMembers();
        List<Enemy> enemies = _controller.field.enemyParty.GetActiveMembers();

        if (statusBars == null || enemyStatusBars == null || players.Count != statusBars.Count || enemies.Count != enemyStatusBars.Count) {
            // TODO: instead of clearing the list each time, create only the one that needs update.
            this.rebuildStatusBars();
        }

        this.setStatusBarUI();
    }

    private void rebuildStatusBars() {
        this.DestroyCurrentStatusBars();
        int playerCount = _controller.field.playerParty.GetActiveMembers().Count;

        statusBars = new List<StatusBarUI>();

        for (int i = 0; i < playerCount; i++) { 
            StatusBarUI statusBarForPlayer;
            
            if (i == 0) {
                statusBarForPlayer = Instantiate(manaStatusBarPrefab);
            } else {
                statusBarForPlayer = Instantiate(mookStatusBarPrefab);
            }

            statusBarForPlayer.transform.SetParent(playerStatusBarParent);
            statusBars.Add(statusBarForPlayer);
        }

        enemyStatusBars = new List<EnemyStatusBarUI>();

        int enemyCount = _controller.field.enemyParty.GetActiveMembers().Count;

        for (int i = 0; i < enemyCount; i++) {
            EnemyStatusBarUI statusBarForPlayer = Instantiate(enemyStatusBarPrefab);
            statusBarForPlayer.transform.SetParent(enemyStatusBarParent[i]);
            enemyStatusBars.Add(statusBarForPlayer);
        }
    }

    private void setStatusBarUI() {
        List<Player> players = _controller.field.playerParty.GetActiveMembers();
        for (int i = 0; i < players.Count; i++) {
            Player player = players[i];
            if (i == 0) {
                // Heros have mana
                ManaStatusBarUI heroStatusBar = (ManaStatusBarUI)statusBars[i];
                heroStatusBar.SetName(player.Name);
                heroStatusBar.SetHP(player.stats.GetHp(), players[i].stats.maxHp);
                heroStatusBar.SetMana(player.stats.GetMana(), player.stats.maxMana);
            } else {
                // Mooks have energy
                MookStatusBarUI mookStatusBar = (MookStatusBarUI)statusBars[i];
                mookStatusBar.SetName(player.Name);
                mookStatusBar.SetHP(player.stats.GetHp(), player.stats.maxHp);
                // Note: Energy might be its own thing later, but for now, lets just use mana
                mookStatusBar.SetEnergy(player.stats.GetMana(), player.stats.maxMana);
            }
        }

        List<Enemy> enemies = _controller.field.enemyParty.GetActiveMembers();

        for (int i = 0; i < enemies.Count; i++) {
            enemyStatusBars[i].SetName(enemies[i].Name);
            enemyStatusBars[i].SetHP(enemies[i].stats.GetHp(), enemies[i].stats.maxHp);
        }
    }

    private void onPlayerJoin(List<Player> players) {
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
