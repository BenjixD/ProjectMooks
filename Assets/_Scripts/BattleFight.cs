using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleFight
{
    private TurnController _controller {get; set;}

    public FightingEntity fighter;

    public BattleFight(TurnController controller, FightingEntity fighter) {
        _controller = controller;
        this.fighter = fighter;
    }

    public IEnumerator DoFight() {
        if (this.fighter.isEnemy()) {
            this.getEnemyAction();
        }

        QueuedAction attackerAction = this.fighter.GetQueuedAction();
        string attackerName = this.fighter.Name;
        if (attackerAction == null) {
            Debug.LogError("Cannot find queued action: "  + attackerName);
            yield break;
        }
        string attackName = attackerAction._action.name;
        List<FightingEntity> targets = attackerAction._action.GetTargets(this.fighter, attackerAction.GetTargetIds());

        // Set commentary text
        string commentaryText;
        if (targets.Count == 1) {
            commentaryText = "" + attackerName + " used " + attackName + " on " + targets[0].Name;
        } else {
            commentaryText = "" + attackerName + " used " + attackName;
        }

        this._controller.ui.commandCardUI.SetCommentaryUIText(commentaryText);

        yield return this._controller.StartCoroutine(doAnimation(this.fighter, attackerAction, targets));

        this.applyEffect(attackerAction);

        this._controller.ui.statusBarsUI.UpdateStatusBarUI();

        this.onFightEnd();
    }

    private void onFightEnd() {

    }

    private void getEnemyAction() {
        // TODO: Implment AI here
        this.fighter.SetQueuedAction(new QueuedAction(this.fighter, this.fighter.GetRandomAction(), new List<int>{_controller.stage.GetRandomPlayerIndex()}  ));
    }

    private void applyEffect(QueuedAction attackerAction) {
        attackerAction.ExecuteAction();
    }

    private IEnumerator doAnimation(FightingEntity a, QueuedAction attackerAction, List<FightingEntity> targets) {
        // TODO: Put animation here
        yield return new WaitForSeconds(1.5f);
    }

}
