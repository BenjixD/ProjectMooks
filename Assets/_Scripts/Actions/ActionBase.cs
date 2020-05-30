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

[System.Serializable]
public class ActionCost {
    public int HP;
    public int mana;
    public int PP;
    public int stamina;
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

[CreateAssetMenu(fileName = "NewAction", menuName = "Actions/ActionBase", order = 1)]
public class ActionBase : ScriptableObject {
    public new string name;
    public ActionType actionType;
    [TextArea] public string description;

    [Header("Resources")]
    [Tooltip("Max number of uses for this action. Leave at 0 for unlimited uses.")]
    public int maxPP;
    private int _currPP;
    private bool _infiniteUses = false;
    public ActionCost actionCost;

    [Header("Effect Properties")]
    public TargetInfo targetInfo;
    [Tooltip("Basic action effects. Override ApplyEffect for more control over effects.")]
    public ActionEffects effects;

    [Header("Command")]
    [Tooltip("The main command word, e.g., \"a\".")]
    public string commandKeyword;
    [Tooltip("The number of arguments following the keyword.")]
    public int commandArgs;

    [Header("Animations")]
    [Tooltip("The name of the animation played for the user of the attack.")]
    public string userAnimName;

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
        if (splitCommand.Length == 0 || 
            !CheckKeyword(splitCommand[0]) || 
            !CheckArgQuantity(splitCommand.Length - 1) || 
            !GameManager.Instance.battleComponents.turnManager.GetPhase().CanInputActions() || 
            !CheckCost(user) ) {
            Debug.Log(GameManager.Instance.battleComponents.turnManager.GetPhase().GetType().Name);
            return false;
        }
        return true;
    }

    private bool CheckCost(FightingEntity user) {
        PlayerStats stats = user.stats;
        if (stats.GetHp() <= actionCost.HP || stats.GetMana() < actionCost.mana) {
            return false;
        }
        if (!_infiniteUses && _currPP < actionCost.PP) {
            return false;
        }
        if (user is Mook && ((Mook) user).stamina.GetStamina() < actionCost.stamina) {
            return false;
        }
        return true;
    }

    private void PayCost(FightingEntity user) {
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
        if (!BasicValidation(splitCommand, user)) {
            return false;
        }
        if (targetInfo.targetType == TargetType.SINGLE) {
            int targetId = this.GetTargetIdFromString(splitCommand[1], user);
            if (targetId == -1) {
                return false;
            }
            user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ targetId }));
        } else if (targetInfo.targetType == TargetType.ALL) {
            List<int> targetIds = this.GetAllPossibleTargets(user).Map((FightingEntity target) => target.targetId );
            user.SetQueuedAction(new QueuedAction(user, this, targetIds));
        }
        return true;
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
            return GameManager.Instance.battleComponents.field.GetAllFightingEntities();
        }
        List<FightingEntity> potentialTargets;
        List<FightingEntity> enemies = new List<FightingEntity>(GameManager.Instance.battleComponents.field.GetEnemyObjects());
        List<FightingEntity> players = new List<FightingEntity>(GameManager.Instance.battleComponents.field.GetPlayerObjects());

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
            if (target < potentialTargets.Count && potentialTargets[target] != null && !potentialTargets[target].HasModifier(ModifierAilment.MODIFIER_UNTARGETTABLE)) {
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

    public virtual FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        PlayerStats before, after;
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        int attackDamage = (int) (user.stats.GetPhysical() * effects.physicalScaling + user.stats.GetSpecial() * effects.specialScaling);
        
        foreach (FightingEntity target in targets) {
            // Default damage mitigation: defense and resistance with half the scaling ratios
            int defence = (int) ((target.stats.GetDefense() * effects.physicalScaling + target.stats.GetResistance() * effects.specialScaling) / 2);
            int damage = Mathf.Max(attackDamage - defence, 0);
            // If there is a healing effect, negate the damage and ignore damage mitigation
            if (effects.heals) {
                damage = -attackDamage;
            }

            before = (PlayerStats)target.stats.Clone();
            target.stats.SetHp(target.stats.GetHp() - damage);
            after = (PlayerStats)target.stats.Clone();

            foreach(StatusAilment sa in effects.statusAilments) {
                target.GetAilmentController().AddStatusAilment(Instantiate(sa));
            }

            receivers.Add(new DamageReceiver(target, before, after, effects.statusAilments));
        }
        return new FightResult(user, this, receivers);
    }
}
