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

    // Keep a copy of the actions here because Mooks can change their actions mid-fight
    private List<QueuedAction> queuedActions;

    public BattlePhase(BattleUI ui, BattleField field, string callback) : base(TurnPhase.BATTLE, callback) {
        this._ui = ui;
        this._field = field;
        Messenger.AddListener<FightResult>(Messages.OnFightEnd, this.OnFightEnd);
    }

    public override void Dispose() {
        if (this.currentFight != null) {
            this.currentFight.Dispose();
        }

        Messenger.RemoveListener<FightResult>(Messages.OnFightEnd, this.OnFightEnd);

        base.Dispose();
    }

    protected override void OnPhaseStart() {
        // Order Players

        
        fighters = new List<FightingEntity>(this._field.GetAllFightingEntities());

        fighters = fighters.Filter(fighter => !fighter.HasModifier(ModifierAilment.MODIFIER_CANNOT_USE_ACTION));
        fighters.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.speed.GetValue().CompareTo(a.stats.speed.GetValue()); });

        this.SetEnemyCommands();
        this.SetUnsetMookCommands();

        queuedActions = new List<QueuedAction>();
        foreach (FightingEntity fighter in fighters) {
            queuedActions.Add((QueuedAction)fighter.GetQueuedAction().Clone());
        }

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
        yield return GameManager.Instance.time.GetController().StartCoroutine(this._ui.battleStartUI.PlayAnimation());

        this._ui.targetIconsUI.ClearTargetArrows();

        for (int i = 0; i < fighters.Count; i++) {

            if (fighters[i] == null) {
                // This can happen if the fighter dies mid-battle
                this._ui.battleOrderUI.PopFighter();
                yield return GameManager.Instance.time.GetController().WaitForSeconds(1.0f);
                continue;
            }

            // Apply Status Ailment
            fighters[i].ailmentController.TickAilmentEffects(this._phase);

            if (queuedActions[i] == null) {
                Debug.LogError("ERROR: Queued action was null!");
            }

            QueuedAction attackerAction = queuedActions[i];

            // This can happen if target dies mid-battle
            if (attackerAction._action.TargetsDead(fighters[i], attackerAction.GetTargetIds())) {
                Debug.Log("Skip attacker's turn: " + fighters[i].Name);
                this._ui.battleOrderUI.PopFighter();
                yield return GameManager.Instance.time.GetController().WaitForSeconds(1.0f);
                continue;
            }
            
            BattleFight fight = new BattleFight(_field, fighters[i], attackerAction, this._ui);
            this.currentFight = fight;

            yield return GameManager.Instance.time.GetController().StartCoroutine( this._ui.focusOverlay.EnableOverlay() );

            yield return GameManager.Instance.time.GetController().StartCoroutine(fight.DoFight());

            yield return GameManager.Instance.time.GetController().StartCoroutine( this._ui.focusOverlay.DisableOverlay() );

            this._ui.battleOrderUI.PopFighter();
        }
    }

    private void SetEnemyCommands() {
        foreach (FightingEntity fighter in this.fighters) {
            if (fighter.isEnemy()) {
                ActionBase enemyAction = fighter.GetRandomAction();
                List<int> targets = new List<int>();
                if (enemyAction.targetInfo.targetType == TargetType.SINGLE) {
                    targets = new List<int>{this._field.GetRandomPlayerObjectIndex()};
                } else if (enemyAction.targetInfo.targetType == TargetType.ALL) {
                    targets = enemyAction.GetAllPossibleTargetIds();
                }
                fighter.SetQueuedAction(enemyAction, targets);
            }
        }
    }

    private void OnFightEnd(FightResult result) {
        this.result.results.Add(result);
        this.currentFight = null;
    }

    private void SetUnsetMookCommands() {
        foreach (var player in this._field.GetActivePlayerObjects()) {
            if (!player.HasSetCommand()) {

                if (player.HasModifier(ModifierAilment.MODIFIER_CANNOT_USE_ACTION)) {
                    // TODO later
                    return;
                }

                ActionBase action = player.GetRecommendedAction();

                switch (action.targetInfo.targetType) {
                    case TargetType.SINGLE:
                        List<FightingEntity> possibleTargets = action.GetAllPossibleActiveTargets(player);
                        int randomIndex = Random.Range(0, possibleTargets.Count);
                        player.SetQueuedAction(player.GetRecommendedAction(), new List<int>{ possibleTargets[randomIndex].targetId });
                        break;
                    case TargetType.ALL:
                        List<int> allEnemies = new List<int>();
                        for (int i = 0; i < this._field.GetEnemyObjects().Length; i++) {
                            allEnemies.Add(i);
                        }

                        player.SetQueuedAction(action, allEnemies);
                        break;
                    default:
                        player.SetQueuedAction(action, new List<int>());
                        break;
                }
            }
        }
    }
}