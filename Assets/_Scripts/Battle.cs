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
        List<FightingEntity> orderedPlayers = new List<FightingEntity>(_controller.field.GetAllFightingEntities());
        orderedPlayers.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        // TODO: Raise another message - specifically for battle order
        //Messenger.Broadcast<BattleResult>(Messages.OnBattleStart, new BattleResult(orderedPlayers));
        this._controller.ui.targetIconsUI.ClearTargetArrows();
        this._controller.ui.battleOrderUI.SetTurnOrder(orderedPlayers);
        result = new BattleResult(orderedPlayers);

        _controller.StartCoroutine(handleBattle(orderedPlayers));
    }

    private void onFightEnd(FightResult result) {
        this.result.results.Add(result);
    }

    private void initialize() {
        this.setUnsetMookCommands();
    }

    private IEnumerator handleBattle(List<FightingEntity> fighters) {
        for (int i = 0; i < fighters.Count; i++) {

            if (fighters[i] == null) {
                // This can happen if the fighter dies mid-battle
                continue;
            }

            // Apply Status Ailment
            fighters[i].GetAilmentController().TickAilmentEffects(_controller.battlePhase);

            if (fighters[i].GetQueuedAction() == null) {
                // This sets the enemy's action
                // TODO: Will be moved after AI is merged.
                fighters[i].SetQueuedAction(new QueuedAction(fighters[i], fighters[i].GetRandomAction(), new List<int>{_controller.field.playerParty.GetRandomActiveIndex()}  ));
            }

            QueuedAction attackerAction = fighters[i].GetQueuedAction();

            // This can happen if target dies mid-battle
            if (attackerAction._action.GetTargets(fighters[i], attackerAction.GetTargetIds()).Count == 0) {
                continue;
            }
            
            BattleFight fight = new BattleFight(_controller, fighters[i]);
            this.currentFight = fight;
            yield return _controller.StartCoroutine(fight.DoFight());

            this._controller.ui.battleOrderUI.PopFighter();
        }

        this.endBattle();
    }

    private void endBattle() {
        this.currentFight = null;
        this._controller.BroadcastOnBattleEnd(result);
    }

    private void setUnsetMookCommands() {
        foreach (var player in _controller.field.playerParty.GetActiveMembers()) {
            if (player.HasSetCommand() == false) {
                player.SetQueuedAction(new QueuedAction(player, player.GetRecommendedAction(), new List<int>{_controller.field.enemyParty.GetRandomActiveIndex()}  ));
            }
        }
    }

}
