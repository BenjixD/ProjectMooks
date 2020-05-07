using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BattleResult {

    public List<FightingEntity> fighters = new List<FightingEntity>();
    public List<FightResult> results = new List<FightResult>();

    public BattleResult(){ }
    public BattleResult(List<FightingEntity> fighters) {
        this.fighters = new List<FightingEntity>(fighters);
    }
}


public class Battle
{
    private TurnController _controller {get; set;}

    public BattleFight currentFight{get; set;}

    private BattleResult result;

    public Battle(TurnController controller) {
        _controller = controller;
        Messenger.AddListener<FightResult>(Messages.OnFightEnd, this.onFightEnd);
    }

    ~Battle() {
        Messenger.RemoveListener<FightResult>(Messages.OnFightEnd, this.onFightEnd);
    }

    public void StartBattle() {

        this.initialize();
        // Sort by player speed
        List<FightingEntity> orderedPlayers = new List<FightingEntity>(_controller.stage.GetAllFightingEntities());
        orderedPlayers.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        result = new BattleResult(orderedPlayers);
        Messenger.Broadcast<BattleResult>(Messages.OnBattleStart, result);
        result = new BattleResult(orderedPlayers);

        Messenger.AddListener<FightResult>(Messages.OnFightEnd, this.onFightEnd);

        _controller.StartCoroutine(handleBattle(orderedPlayers));
    }

    private void onFightEnd(FightResult result) {
        this.result.results.Add(result);
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

        this.endBattle();
    }

    private void endBattle() {
        this.currentFight = null;

        Messenger.Broadcast<BattleResult>(Messages.OnBattleEnd, result);
    }

    private void setUnsetMookCommands() {
        foreach (var player in _controller.stage.GetPlayers()) {
            if (player.HasSetCommand() == false) {
                player.SetQueuedAction(new QueuedAction(player, player.actions[0], new List<int>{_controller.stage.GetRandomEnemyIndex()}  ));
            }
        }
    }

}
