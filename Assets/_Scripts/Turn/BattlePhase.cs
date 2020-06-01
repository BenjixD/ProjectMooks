using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BattlePhase : Phase {

	public List<FightingEntity> fighters;
    public BattleFight currentFight{get; set;}
    private BattleResult result;

    public BattlePhase(TurnController controller, string callback) : base(controller, callback) {
        Messenger.AddListener<FightResult>(Messages.OnFightEnd, this.OnFightEnd);
    }

    ~BattlePhase() {
        Messenger.RemoveListener<FightResult>(Messages.OnFightEnd, this.OnFightEnd);
    }

    protected override void OnPhaseStart() {
    	// Set Mook Commands
    	this.SetUnsetMookCommands();

    	// Order Players
    	fighters = new List<FightingEntity>(_controller.field.GetAllFightingEntities());
        fighters.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });
        result = new BattleResult(fighters);

        // Call Base Implementation
    	base.OnPhaseStart();
    }

    protected override void OnPhaseEnd() {
    	this.currentFight = null;

    	// Call Base Implementation
    	base.OnPhaseEnd();
    }

    protected override IEnumerator Run() {
        this._controller.ui.battleOrderUI.SetTurnOrder(fighters);
        yield return GameManager.Instance.time.WaitForSeconds(0.5f);
        this._controller.ui.targetIconsUI.ClearTargetArrows();

        for (int i = 0; i < fighters.Count; i++) {

            if (fighters[i] == null) {
                // This can happen if the fighter dies mid-battle
                this._controller.ui.battleOrderUI.PopFighter();
                yield return GameManager.Instance.time.WaitForSeconds(1.0f);
                continue;
            }

            // Apply Status Ailment
            fighters[i].GetAilmentController().TickAilmentEffects(this._phase);

            if (fighters[i].GetQueuedAction() == null) {
                // This sets the enemy's action
                // TODO: Will be moved after AI is merged.
                fighters[i].SetQueuedAction(new QueuedAction(fighters[i], fighters[i].GetRandomAction(), new List<int>{_controller.field.playerParty.GetRandomActiveIndex()}  ));
            }

            QueuedAction attackerAction = fighters[i].GetQueuedAction();

            // This can happen if target dies mid-battle
            if (attackerAction._action.GetTargets(fighters[i], attackerAction.GetTargetIds()).Count == 0) {
                this._controller.ui.battleOrderUI.PopFighter();
                yield return GameManager.Instance.time.WaitForSeconds(1.0f);
                continue;
            }
            
            BattleFight fight = new BattleFight(_controller, fighters[i]);
            this.currentFight = fight;
            yield return _controller.StartCoroutine(fight.DoFight());

            this._controller.ui.battleOrderUI.PopFighter();
        }
    }

    private void OnFightEnd(FightResult result) {
    	this.result.results.Add(result);
    }

    private void SetUnsetMookCommands() {
        foreach (var player in _controller.field.playerParty.GetActiveMembers()) {
            if (player.HasSetCommand() == false) {
                player.SetQueuedAction(new QueuedAction(player, player.GetRecommendedAction(), new List<int>{_controller.field.enemyParty.GetRandomActiveIndex()}  ));
            }
        }
    }
}