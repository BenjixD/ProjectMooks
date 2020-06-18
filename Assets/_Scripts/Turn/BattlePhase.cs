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


[System.Serializable]
public class BattlePhase : Phase {

    public List<FightingEntity> fighters;
    public BattleFight currentFight{get; set;}

    protected BattleUI _ui {get; set; }
    protected BattleField _field {get; set;}
    private BattleResult result;

    public BattlePhase(BattleUI ui, BattleField field, string callback) : base(TurnPhase.BATTLE, callback) {
        this._ui = ui;
        this._field = field;
        Messenger.AddListener<FightResult>(Messages.OnFightEnd, this.OnFightEnd);
    }

    ~BattlePhase() {
        Messenger.RemoveListener<FightResult>(Messages.OnFightEnd, this.OnFightEnd);
    }

    protected override void OnPhaseStart() {
        // Set Mook Commands
        this.SetUnsetMookCommands();

        // Order Players
        fighters = new List<FightingEntity>(this._field.GetAllFightingEntities());
        fighters.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });
        result = new BattleResult(fighters);

        // Call Base Implementation
        base.OnPhaseStart();
    }

    protected override void OnPhaseEnd() {
        Messenger.Broadcast<BattleResult>(Messages.OnBattleEnd, this.result);
        this.currentFight = null;

        // Call Base Implementation
        base.OnPhaseEnd();
    }

    protected override IEnumerator Run() {
        this._ui.battleOrderUI.SetTurnOrder(fighters);
        yield return GameManager.Instance.time.WaitForSeconds(0.5f);
        this._ui.targetIconsUI.ClearTargetArrows();

        for (int i = 0; i < fighters.Count; i++) {

            if (fighters[i] == null) {
                // This can happen if the fighter dies mid-battle
                this._ui.battleOrderUI.PopFighter();
                yield return GameManager.Instance.time.WaitForSeconds(1.0f);
                continue;
            }

            // Apply Status Ailment
            fighters[i].GetAilmentController().TickAilmentEffects(this._phase);

            if (fighters[i].GetQueuedAction() == null) {
                // This sets the enemy's action
                // TODO: Will be moved after AI is merged.
                fighters[i].SetQueuedAction(new QueuedAction(fighters[i], fighters[i].GetRandomAction(), new List<int>{this._field.GetRandomPlayerObjectIndex()}  ));
            }

            QueuedAction attackerAction = fighters[i].GetQueuedAction();

            // This can happen if target dies mid-battle
            if (attackerAction._action.GetTargets(fighters[i], attackerAction.GetTargetIds()).Count == 0) {
                this._ui.battleOrderUI.PopFighter();
                yield return GameManager.Instance.time.WaitForSeconds(1.0f);
                continue;
            }
            
            BattleFight fight = new BattleFight(_field, fighters[i]);
            this.currentFight = fight;
            yield return GameManager.Instance.time.StartCoroutine(fight.DoFight());

            this._ui.battleOrderUI.PopFighter();
        }
    }

    private void OnFightEnd(FightResult result) {
        this.result.results.Add(result);
        this.currentFight = null;
    }

    private void SetUnsetMookCommands() {
        foreach (var player in this._field.GetActivePlayerObjects()) {
            if (!player.HasSetCommand()) {
                player.SetQueuedAction(new QueuedAction(player, player.GetRecommendedAction(), new List<int>{this._field.GetRandomEnemyObjectIndex()}  ));
            }
        }
    }
}