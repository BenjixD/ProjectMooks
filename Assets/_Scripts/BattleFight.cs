using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageReceiver {
    public FightingEntity fighter;
    public PlayerStats before;
    public PlayerStats after;
    public List<StatusAilment> ailments;

    public DamageReceiver(FightingEntity fighter, PlayerStats before, PlayerStats after) {
        this.fighter = fighter;
        this.before = before;
        this.after = after;
        ailments = new List<StatusAilment>();
    }

    public DamageReceiver(FightingEntity fighter, PlayerStats before, PlayerStats after, List<StatusAilment> ailments) {
        this.fighter = fighter;
        this.before = before;
        this.after = after;
        this.ailments = ailments;
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
        // if (this.fighter.isEnemy()) {
        //     this.getEnemyAction();
        // }

        Messenger.Broadcast<FightResult>(Messages.OnFightStart, new FightResult(this.fighter, this.fighter.GetQueuedAction().GetAction()));

        QueuedAction attackerAction = this.fighter.GetQueuedAction();
        string attackerName = this.fighter.Name;
        if (attackerAction == null) {
            Debug.LogError("Cannot find queued action: "  + attackerName);
            yield break;
        }
        string attackName = attackerAction._action.name;
        List<FightingEntity> targets = attackerAction._action.GetTargets(this.fighter, attackerAction.GetTargetIds());

        yield return this._controller.StartCoroutine(doAnimation(this.fighter, attackerAction, targets));

        FightResult fightResult = attackerAction.ExecuteAction();

        this.CheckIfAnyEntityDied(fightResult, attackerAction._action);

        this.onFightEnd(fightResult);
    }

    private void CheckIfAnyEntityDied(FightResult result, ActionBase action) {
        foreach (var damageReceiver in result.receivers) {
            if (damageReceiver.fighter.stats.GetHp() <= 0) {
                DeathResult deathResult = new DeathResult(result.fighter, damageReceiver, action);
                Messenger.Broadcast<DeathResult>(Messages.OnEntityDeath, deathResult);
            }
        }
    }

    private void onFightEnd(FightResult result) {
        Messenger.Broadcast<FightResult>(Messages.OnFightEnd, result);
    }

    private void getEnemyAction() {
        // TODO: Aggro targetting
        this.fighter.SetQueuedAction(new QueuedAction(this.fighter, this.fighter.GetRecommendedAction(), new List<int>{_controller.field.GetRandomPlayerObjectIndex()}  ));
    }

    private IEnumerator doAnimation(FightingEntity a, QueuedAction attackerAction, List<FightingEntity> targets) {
        // TODO: Put animation here
        yield return GameManager.Instance.time.WaitForSeconds(1.0f);
    }

}
