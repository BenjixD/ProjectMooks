using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Battle
{
    private TurnController _controller {get; set;}

    public BattleFight currentFight{get; set;}

    public Battle(TurnController controller) {
        _controller = controller;
    }

    public void StartTurn() {

        this.initialize();
        // Sort by player speed
        List<FightingEntity> orderedPlayers = new List<FightingEntity>(_controller.stage.GetAllFightingEntities());
        orderedPlayers.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        _controller.StartCoroutine(handleBattle(orderedPlayers));

    }

    private void initialize() {
        this.setUnsetMookCommands();
        this._controller.ui.commandCardUI.InitializeCommantaryUI();
    }

    private IEnumerator handleBattle(List<FightingEntity> fighters) {
        for (int i = 0; i < fighters.Count; i++) {
            BattleFight fight = new BattleFight(_controller, fighters[i]);
            this.currentFight = fight;
            yield return _controller.StartCoroutine(fight.DoFight());
        }

        this.endTurn();
    }

    private void endTurn() {
        this._controller.OnPlayerTurnStart();
        this.currentFight = null;
    }

    private void setUnsetMookCommands() {
        foreach (var player in _controller.stage.GetPlayers()) {
            if (player.HasSetCommand() == false) {
                player.SetQueuedAction(new QueuedAction(player, player.actions[0], new List<int>{_controller.stage.GetRandomEnemyIndex()}  ));
            }
        }
    }

}
