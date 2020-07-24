using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarsUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform playerStatusBarParent;

    // TODO: Instead of RectTransform, do it through FighterSlot
    public RectTransform[] enemyStatusBarParent;

    public ManaStatusBarUI manaStatusBarPrefab;
    public MookStatusBarUI mookStatusBarPrefab;
    public EnemyStatusBarUI enemyStatusBarPrefab;


    private StatusBarUI[] statusBars = new StatusBarUI[PlayerParty.maxPlayers];
    private EnemyStatusBarUI[] enemyStatusBars = new EnemyStatusBarUI[EnemyParty.maxPlayers];

    private int maxMookEnergy = 8;

    public void Initialize() {
        Messenger.AddListener<BattleResult>(Messages.OnBattleEnd, this.OnBattleEnd);
        Messenger.AddListener<QueuedAction>(Messages.OnSetQueuedAction, this.onSetQueuedAction);
        Messenger.AddListener(Messages.OnUpdateStatusBarsUI, this.UpdateStatusBars);


        OutOfJuiceAilment outOfJuicePrefab = (OutOfJuiceAilment)GameManager.Instance.models.GetCommonStatusAilment("Out of Juice");
        this.maxMookEnergy = outOfJuicePrefab.duration;

        this.buildStatusBars();
        this.UpdateStatusBars();
    }

    void OnDestroy() {
        Messenger.RemoveListener<BattleResult>(Messages.OnBattleEnd, this.OnBattleEnd);
        Messenger.RemoveListener<QueuedAction>(Messages.OnSetQueuedAction, this.onSetQueuedAction);
        Messenger.RemoveListener(Messages.OnUpdateStatusBarsUI, this.UpdateStatusBars);
    }

    public void UpdateStatusBars() {
        PlayerObject[] players = GameManager.Instance.battleComponents.field.GetPlayerObjects();

        for (int i = 0; i < players.Length; i++) {
            if (this.statusBars[i] == null) {
                continue;
            }

            PlayerObject player = players[i];

            if (player == null) {
                this.statusBars[i].gameObject.SetActive(false);
                continue;
            }

            if (i == 0) {
                // Heros have mana
                ManaStatusBarUI heroStatusBar = (ManaStatusBarUI)statusBars[i];
                heroStatusBar.SetFighter(player);
                heroStatusBar.SetName(player.Name);
                heroStatusBar.SetHP(player.stats.hp.GetValue(), player.stats.maxHp.GetValue());
                heroStatusBar.SetMana(player.stats.mana.GetValue(), player.stats.maxMana.GetValue());
                heroStatusBar.SetNameColor(player.GetOrderColor());
                heroStatusBar.SetStatusAilmentIcons(player.ailmentController.GetAllAilments().Map(entry => entry.Value));
            } else {
                // Mooks have energy
                MookStatusBarUI mookStatusBar = (MookStatusBarUI)statusBars[i];
                mookStatusBar.SetFighter(player);
                mookStatusBar.SetName(player.Name);
                mookStatusBar.SetHP(player.stats.hp.GetValue(), player.stats.maxHp.GetValue());
                mookStatusBar.SetStatusAilmentIcons(player.ailmentController.GetAllAilments().Map(entry => entry.Value));
                // Note: Energy might be its own thing later, but for now, lets just use mana
                
                mookStatusBar.SetEnergy(player.ailmentController.GetAilment("Out of Juice").duration, maxMookEnergy);
                mookStatusBar.SetNameColor(player.GetOrderColor());
                if (!player.HasSetCommand()) {
                    mookStatusBar.actionMenuUI.UnsetActions();
                }
            }

            this.statusBars[i].gameObject.SetActive(true);
        }

        EnemyObject[] enemies = GameManager.Instance.battleComponents.field.GetEnemyObjects();

        for (int i = 0; i < enemies.Length; i++) {
            if (this.enemyStatusBars[i] == null) {
                continue;
            }

            EnemyObject enemy = enemies[i];
            if (enemy == null) {
                this.enemyStatusBars[i].gameObject.SetActive(false);
                continue;
            }

            enemyStatusBars[i].SetFighter(enemy);
            enemyStatusBars[i].SetName(enemies[i].targetName);
            enemyStatusBars[i].SetHP(enemies[i].stats.hp.GetValue(), enemies[i].stats.maxHp.GetValue());
            enemyStatusBars[i].SetStatusAilmentIcons(enemy.ailmentController.GetAllAilments().Map(entry => entry.Value));
            Transform barMarker = enemy.GetComponent<FighterPositions>().statusBar;
            WorldspaceToScreenUI barPositioner = enemyStatusBars[i].GetComponent<WorldspaceToScreenUI>();
            barPositioner.SetWorldPoint(barMarker);

            this.enemyStatusBars[i].gameObject.SetActive(true);
        }
    }

    private void buildStatusBars() {
        for (int i = 0; i < PlayerParty.maxPlayers; i++) { 
            StatusBarUI statusBarForPlayer;
            
            if (i == 0) {
                statusBarForPlayer = Instantiate(manaStatusBarPrefab);
            } else {
                statusBarForPlayer = Instantiate(mookStatusBarPrefab);
            }

            statusBarForPlayer.transform.SetParent(playerStatusBarParent);
            statusBarForPlayer.gameObject.SetActive(false);
            statusBarForPlayer.gameObject.name = statusBarForPlayer.gameObject.name + i;
            statusBars[i] = statusBarForPlayer;
        }


        for (int i = 0; i < EnemyParty.maxPlayers; i++) {
            EnemyStatusBarUI statusBarForEnemy = Instantiate(enemyStatusBarPrefab, enemyStatusBarParent[i]);
            statusBarForEnemy.gameObject.SetActive(false);
            enemyStatusBars[i] = statusBarForEnemy;
        }
    }

    private void OnBattleEnd(BattleResult result) {
        PlayerObject[] players = GameManager.Instance.battleComponents.field.GetPlayerObjects();
        for (int i = 1; i < players.Length; i++) {
            // Experimental: Mooks keep their actions at the end of each turn
            // Mooks have energy
            //MookStatusBarUI mookStatusBar = (MookStatusBarUI)statusBars[i];
            //mookStatusBar.actionMenuUI.UnsetActions();
        }

        this.UpdateStatusBars();
    }

    private void onSetQueuedAction(QueuedAction action) {
        if (action == null) {
            Debug.LogError("Should not be broadcasted null");
        }

        foreach (StatusBarUI statusBar in this.statusBars) {
            if (statusBar.GetName() == action.user.Name) {
                if (statusBar.GetType() == typeof(MookStatusBarUI)) {
                    MookStatusBarUI mookStatus = (MookStatusBarUI) statusBar;
                    mookStatus.actionMenuUI.SetAction(action);
                }
            }
        }
    }
}
