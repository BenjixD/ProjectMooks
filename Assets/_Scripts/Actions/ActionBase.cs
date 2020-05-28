using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TargetTeam{
    MY_TEAM,
    ENEMY_TEAM,
    BOTH_TEAMS,
    NONE

}

public enum TargetType {
    SINGLE,
    ALL,
    NONE
};

[System.Serializable]
public class TargetInfo {
    public TargetTeam targetTeam;
    public TargetType targetType;

    public TargetInfo(TargetTeam team, TargetType type) {
        this.targetTeam = team;
        this.targetType = type;
    }
}

// BASIC = get its own command in the command line (i.e. attack/defend/run)
// MAGIC = magic
// ITEM = item
public enum ActionType {
    BASIC,
    MAGIC,
    ITEM

}

public class DeathResult {
    public FightingEntity attacker;
    public DamageReceiver deadEntity;
    public ActionBase movedUsedToKill;

    public DeathResult(FightingEntity attacker, DamageReceiver deadEntity, ActionBase move) {
        this.attacker = attacker;
        this.deadEntity = deadEntity;
        this.movedUsedToKill = move;
    }
}

public abstract class ActionBase : ScriptableObject {
    public string name;
    [Tooltip("Max number of uses for this action. Leave at 0 for unlimited uses.")]
    public int maxPP;
    private int _currPP;
    private bool _infiniteUses = false;
    public ActionCost actionCost;
    [TextArea]
    public string description;
    [Tooltip("The main command word, e.g., \"a\".")]
    public string commandKeyword;
    [Tooltip("The number of arguments following the keyword.")]
    public int commandArgs;
    [Tooltip("The name of the animation played for the user of the attack.")]
    public string userAnimName;

    public TargetInfo targetInfo;
    public ActionType actionType;

    private void Awake() {
        if (maxPP == 0) {
            _infiniteUses = true;
        } else {
            _currPP = maxPP;
        }
    }

    private bool CheckKeyword(string keyword) {
        return keyword == commandKeyword;
    }
    private bool CheckArgQuantity(int argQuantity) {
        return argQuantity == commandArgs;
    }

    protected bool BasicValidation(string[] splitCommand, FightingEntity user) {
        if (splitCommand.Length == 0 || !CheckKeyword(splitCommand[0]) || !CheckArgQuantity(splitCommand.Length - 1) || !GameManager.Instance.turnController.CanInputActions() || !CheckCost(user) ) {
            return false;
        }
        return true;
    }

    protected bool CheckCost(FightingEntity user) {
        if (user is Mook && ((Mook) user).stamina.GetStamina() < actionCost.stamina) {
            return false;
        }
        if (!_infiniteUses && _currPP < actionCost.PP) {
            return false;
        }
        PlayerStats stats = user.stats;
        return stats.GetHp() > actionCost.HP && stats.GetMana() >= actionCost.mana;
    }

    protected void PayCost(FightingEntity user) {
        PlayerStats stats = user.stats;
        stats.SetHp(stats.GetHp() - actionCost.HP);
        stats.SetMana(stats.GetMana() - actionCost.mana);
        _currPP -= actionCost.PP;
        if (user is Mook) {
            Mook mook = (Mook) user;
            mook.stamina.SetStamina(mook.stamina.GetStamina() - actionCost.stamina);
        }
    }

    public virtual bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        return BasicValidation(splitCommand, user);
    }
    
    public FightResult ExecuteAction(FightingEntity user, List<FightingEntity> targets) {
        if (!CheckCost(user)) {
            return new FightResult(user, this);
        }
        user.Animate(userAnimName, false);
        PayCost(user);
        FightResult result = ApplyEffect(user, targets);
        this.OnPostEffect(result);

        return result;
    }

    public List<FightingEntity> GetAllPossibleActiveTargets(FightingEntity user) {
        return this.GetAllPossibleTargets(user).Filter( (FightingEntity entity) => entity != null );
    }

    public List<FightingEntity> GetAllPossibleTargets(FightingEntity user) {
        if (targetInfo.targetTeam == TargetTeam.BOTH_TEAMS) {
            return GameManager.Instance.turnController.field.GetAllFightingEntities();
        }
        List<FightingEntity> potentialTargets;
        List<FightingEntity> enemies = new List<FightingEntity>(GameManager.Instance.turnController.field.enemyParty.members);
        List<FightingEntity> players = new List<FightingEntity>(GameManager.Instance.turnController.field.playerParty.members);

        if (user.isEnemy()) {
            potentialTargets = targetInfo.targetTeam == TargetTeam.MY_TEAM ? enemies : players;
        } else {
            potentialTargets = targetInfo.targetTeam == TargetTeam.MY_TEAM ? players : enemies;
        }

        return potentialTargets;
    }

    public List<FightingEntity> GetTargets(FightingEntity user, List<int> targetIds){ 
        List<FightingEntity> potentialTargets = GetAllPossibleTargets(user);
        List<FightingEntity> targets = new List<FightingEntity>();
        foreach (int target in targetIds) {
            if (target < potentialTargets.Count && potentialTargets[target] != null) {
                targets.Add(potentialTargets[target]);
            }
        }

        return targets;
    }

    public List<FightingEntity> GetTargets(FightingEntity user, int targetId) {
        return this.GetTargets(user, new List<int>{targetId});
    }

    protected virtual void OnPostEffect(FightResult result) {
    }


    protected int GetTargetIdFromString(string str, FightingEntity user) {
        str = str.ToUpper();
        List<FightingEntity> targets = GetAllPossibleActiveTargets(user);

        FightingEntity target = targets.Find((FightingEntity t) => t.targetName == str );
        if (target == null) {
            return -1;
        }
 
        return target.targetId;
    }

    public abstract FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets);
}
