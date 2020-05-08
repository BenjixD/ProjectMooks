using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageReceiver {
    public FightingEntity fighter;
    public PlayerStats before;
    public PlayerStats after;

    public DamageReceiver(FightingEntity fighter, PlayerStats before, PlayerStats after) {
        this.fighter = fighter;
        this.before = before;
        this.after = after;
    }
}

public class FightResult {
    public FightingEntity fighter;
    public List<DamageReceiver> receivers;
    public ActionBase action;

    public FightResult(FightingEntity fighter, ActionBase action) {
        this.fighter = fighter;
        this.action = action;
        this.receivers = new List<DamageReceiver>(); 
    }

    public FightResult(FightingEntity fighter, ActionBase action, List<DamageReceiver> receivers) {
        this.fighter = fighter;
        this.action = action;
        this.receivers = receivers;
    }

    public FightResult(FightingEntity fighter, ActionBase action, DamageReceiver receiver) {
        this.fighter = fighter;
        this.action = action;
        this.receivers = new List<DamageReceiver>();
        this.receivers.Add(receiver);
    }
}

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

        Messenger.Broadcast<FightResult>(Messages.OnFightStart, new FightResult(this.fighter, this.fighter.GetQueuedAction().GetAction()));

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

        FightResult fightResult = attackerAction.ExecuteAction();

        this._controller.ui.statusBarsUI.UpdateStatusBarUI();

        this.onFightEnd(fightResult);
    }

    private void onFightEnd(FightResult result) {
        Messenger.Broadcast<FightResult>(Messages.OnFightEnd, result);
    }

    private void getEnemyAction() {
        // TODO: Aggro targetting
        this.fighter.SetQueuedAction(new QueuedAction(this.fighter, this.fighter.GetRecommendedAction(), new List<int>{_controller.stage.GetRandomPlayerIndex()}  ));
    }

    private IEnumerator doAnimation(FightingEntity a, QueuedAction attackerAction, List<FightingEntity> targets) {
        // TODO: Put animation here
        yield return new WaitForSeconds(1.5f);
    }

}
