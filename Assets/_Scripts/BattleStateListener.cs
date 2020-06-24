using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: May want to store these listeners with the Game Manager
public class BattleStateListener : MonoBehaviour {
    void Start() {
        Messenger.AddListener<DeathResult>(Messages.OnEntityDeath, this.OnEntityDeath);
        Messenger.AddListener(Messages.OnWaveComplete, this.OnWaveComplete);
    }

    void OnDestroy() {
        Messenger.RemoveListener<DeathResult>(Messages.OnEntityDeath, this.OnEntityDeath);
        Messenger.AddListener(Messages.OnWaveComplete, this.OnWaveComplete);
    }

    private void OnEntityDeath(DeathResult result) {
        FightingEntity deadFighter = result.deadEntity.fighter;
        // TODO: Play death animation

        bool isEnemy = deadFighter.isEnemy();
        if (isEnemy) {
            GameManager.Instance.gameState.enemyParty.SetFighter(deadFighter.targetId, null);
            Destroy(deadFighter.gameObject);

            bool stillHasEnemies = false;
            EnemyObject[] enemies = GameManager.Instance.battleComponents.field.GetEnemyObjects();
            for (int i = 0; i < enemies.Length; i++) {
                if (enemies[i] != null) {
                    stillHasEnemies = true;
                    break;
                }
            }

            if (!stillHasEnemies) {
                OnWaveComplete();
            }
        } else {

            if (deadFighter.targetId == 0) {
                OnHeroDeath(result);
            } else {
                GameManager.Instance.gameState.playerParty.EvictPlayer(deadFighter.targetId);
                Destroy(deadFighter.gameObject);
            }
        }

        GameManager.Instance.battleComponents.ui.statusBarsUI.UpdateStatusBars();

         // Note: How we determine this part may change depending on how we do Mook deaths:
        List<PlayerObject> allPlayers = GameManager.Instance.battleComponents.field.GetActivePlayerObjects();
        if (allPlayers.Count == 1 && GameManager.Instance.battleComponents.field.GetHeroPlayer().HasModifier(ModifierAilment.MODIFIER_DEATH)) {
            GameManager.Instance.battleComponents.stageController.LoadDeathScene();
        }
    }

    private void OnWaveComplete() {
        StageInfoContainer stageInfo = GameManager.Instance.gameState.GetCurrentStage();
        WaveInfoContainer waveInfo = stageInfo.GetWaveInfo(GameManager.Instance.battleComponents.field.currentWaveIndex);

        GameManager.Instance.battleComponents.field.currentWaveIndex++;
        if (GameManager.Instance.battleComponents.field.currentWaveIndex >= stageInfo.numWaves) {
            GameManager.Instance.battleComponents.stageController.CompleteStage();
        } else {
            GameManager.Instance.battleComponents.field.GenerateEnemyList(GameManager.Instance.battleComponents.field.currentWaveIndex);
        }
    }

    private void OnHeroDeath(DeathResult result) {
        List<PlayerObject> allPlayers = GameManager.Instance.battleComponents.field.GetActivePlayerObjects();

        PlayerObject heroPlayer = GameManager.Instance.battleComponents.field.GetHeroPlayer();

        heroPlayer.DoDeathAnimation();

        StatusAilment reviveStatusAilmentPrefab = GameManager.Instance.models.GetCommonStatusAilment("Hero's Miracle");
        heroPlayer.GetAilmentController().AddStatusAilment(Instantiate(reviveStatusAilmentPrefab));
    }
}