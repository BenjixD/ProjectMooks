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

public class BattleFight : IDisposable
{
    public FightingEntity fighter;
    private BattleField _field {get; set;}

    public BattleFight(BattleField field, FightingEntity fighter) {
        this._field = field;
        this.fighter = fighter;
    }

    public void Dispose() {

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

        yield return GameManager.Instance.time.StartCoroutine(doAnimation(this.fighter, attackerAction, targets));

        FightResult fightResult = attackerAction.ExecuteAction();

        this.CheckIfAnyEntityDied(fightResult, attackerAction._action);

        this.onFightEnd(fightResult);
    }

    private void CheckIfAnyEntityDied(FightResult result, ActionBase action) {
        foreach (var damageReceiver in result.receivers) {
            if (damageReceiver.fighter.stats.hp.GetValue() <= 0) {
                DeathResult deathResult = new DeathResult(result.fighter, damageReceiver, action);
                Messenger.Broadcast<DeathResult>(Messages.OnEntityDeath, deathResult);
            }
        }
    }

    private void onFightEnd(FightResult result) {
        Messenger.Broadcast<FightResult>(Messages.OnFightEnd, result);
        Messenger.Broadcast(Messages.OnUpdateStatusBarsUI);
    }

    private IEnumerator doAnimation(FightingEntity a, QueuedAction attackerAction, List<FightingEntity> targets) {
        // TODO: Put animation here
        yield return GameManager.Instance.time.GetController().WaitForSeconds(1.0f);
    }

}
