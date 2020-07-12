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

    public override void Dispose() {
        if (this.currentFight != null) {
            this.currentFight.Dispose();
        }

        Messenger.RemoveListener<FightResult>(Messages.OnFightEnd, this.OnFightEnd);

        base.Dispose();
    }

    protected override void OnPhaseStart() {
        // Set Mook Commands
        this.SetUnsetMookCommands();


        Debug.Log("ON BATTLE PHASE Start");

        // Order Players
        fighters = new List<FightingEntity>(this._field.GetAllFightingEntities());
        fighters.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.speed.GetValue().CompareTo(a.stats.speed.GetValue()); });
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
        yield return GameManager.Instance.time.GetController().WaitForSeconds(0.5f);
        this._ui.targetIconsUI.ClearTargetArrows();

        for (int i = 0; i < fighters.Count; i++) {

            if (fighters[i] == null) {
                // This can happen if the fighter dies mid-battle
                this._ui.battleOrderUI.PopFighter();
                yield return GameManager.Instance.time.GetController().WaitForSeconds(1.0f);
                continue;
            }

            // Apply Status Ailment
            fighters[i].GetAilmentController().TickAilmentEffects(this._phase);

            if (fighters[i].GetQueuedAction() == null) {
                // This sets the enemy's action
                // TODO: Will be moved after AI is merged.
                ActionBase enemyAction = fighters[i].GetRandomAction();
                List<int> targets = new List<int>();
                if (enemyAction.targetInfo.targetType == TargetType.SINGLE) {
                    targets = new List<int>{this._field.GetRandomPlayerObjectIndex()};
                } else if (enemyAction.targetInfo.targetType == TargetType.ALL) {
                    targets = enemyAction.GetAllPossibleTargetIds(fighters[i]);
                }
                fighters[i].SetQueuedAction(new QueuedAction(fighters[i], enemyAction, targets));
            }

            QueuedAction attackerAction = fighters[i].GetQueuedAction();

            // This can happen if target dies mid-battle
            if (attackerAction._action.GetTargets(fighters[i], attackerAction.GetTargetIds()).Count == 0) {
                this._ui.battleOrderUI.PopFighter();
                yield return GameManager.Instance.time.GetController().WaitForSeconds(1.0f);
                continue;
            }
            
            BattleFight fight = new BattleFight(_field, fighters[i]);
            this.currentFight = fight;
            yield return GameManager.Instance.time.GetController().StartCoroutine(fight.DoFight());

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
                ActionBase action = player.GetRecommendedAction();

                switch (action.targetInfo.targetType) {
                    case TargetType.SINGLE:
                        List<FightingEntity> possibleTargets = action.GetAllPossibleActiveTargets(player);
                        int randomIndex = Random.Range(0, possibleTargets.Count);
                        player.SetQueuedAction(new QueuedAction(player, player.GetRecommendedAction(), new List<int>{ possibleTargets[randomIndex].targetId }  ));
                        break;
                    case TargetType.ALL:
                        List<int> allEnemies = new List<int>();
                        for (int i = 0; i < this._field.GetEnemyObjects().Length; i++) {
                            allEnemies.Add(i);
                        }

                        player.SetQueuedAction(new QueuedAction(player, action, allEnemies));
                        break;
                    default:
                        player.SetQueuedAction(new QueuedAction(player, action, new List<int>()));
                        break;
                }
            }
        }
    }
}