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

    const string MookFightMessageDamage = "{0} used {1} to deal {2} to {3}!";
    const string MookFightMessageGeneric = "{0} used {1}!";

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

    public string GetAnnouncerMessage() {
        string message = "";
        bool hasWrittenMessage = false;
        if (action.targetInfo.targetTeam == TargetTeam.ENEMY_TEAM && action.targetInfo.targetType == TargetType.SINGLE) {
            int damage = receivers[0].before.hp.GetValue() - receivers[0].after.hp.GetValue();
            if (damage > 0) {
                message = string.Format(MookFightMessageDamage, fighter.Name, action.name, damage, receivers[0].fighter.Name );
                hasWrittenMessage = true;
            }
        }

        if (!hasWrittenMessage) {
            message = string.Format(MookFightMessageGeneric, fighter.Name, action.name);
            hasWrittenMessage = true;
        }


        return message;
    }
}

public class BattleFight : IDisposable
{
    public FightingEntity fighter;
    private BattleField _field {get; set;}
    private QueuedAction queuedAction;
    private BattleUI _ui;

    public BattleFight(BattleField field, FightingEntity fighter, QueuedAction action, BattleUI ui) {
        this._field = field;
        this.fighter = fighter;
        this.queuedAction = action;
        this._ui = ui;
    }

    public void Dispose() {

    }

    public IEnumerator DoFight() {
        // if (this.fighter.isEnemy()) {
        //     this.getEnemyAction();
        // }

        Messenger.Broadcast<FightResult>(Messages.OnFightStart, new FightResult(this.fighter, queuedAction.GetAction()));

        QueuedAction attackerAction = queuedAction;
        string attackerName = this.fighter.Name;
        if (attackerAction == null) {
            Debug.LogError("Cannot find queued action: "  + attackerName);
            yield break;
        }
        ActionBase action = attackerAction.GetAction();
        string attackName = action.name;
        List<FightingEntity> targets = action.GetTargets(this.fighter, attackerAction.GetTargetIds());
        if (targets.Count == 1) {
            Debug.Log(this.fighter.Name + " Execute Action: " + attackName + targets[0].targetId);
        }

    

        List<FightingEntity> focusedFighters = new List<FightingEntity>();
        focusedFighters.Add(fighter);

        List<FightingEntity> focusedTargets = action.GetFocusedTargets(this.fighter, attackerAction.GetTargetIds());
        foreach (FightingEntity target in focusedTargets) {
            focusedFighters.Add(target);
        }


        List<FightingEntity> unfocusedFighters = this._field.GetAllFightingEntities().Filter(fighter => !focusedFighters.Contains(fighter));

        foreach (FightingEntity focusedFighter in focusedFighters) {
            this._ui.focusOverlay.AddToFocusLayer(focusedFighter.gameObject, true);
        }

        foreach (FightingEntity unfocusedFighter in unfocusedFighters) {
            this._ui.focusOverlay.AddToFocusLayer(unfocusedFighter.gameObject, false);
        }


        yield return GameManager.Instance.time.GetController().StartCoroutine(action.ExecuteAction(fighter, targets, this));
    }



    public void EndFight(FightResult fightResult, ActionBase action) {
        CheckIfAnyEntityDied(fightResult, action);

        // Mooks should broadcast their fights for clarity
        if (!fighter.isEnemy() && !fighter.IsHero()) {
            // TODO: do in more clean way
            ActionListener actionListener = fighter.GetComponent<ActionListener>();
            actionListener.EchoMessage(fightResult.GetAnnouncerMessage());
        } 

        onFightEnd(fightResult);
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
}
