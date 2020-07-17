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
    public ActionAnimation animation;

    [Header("Resources")]
    [Tooltip("Max PP for this action.")]
    public int maxPP;
    [HideInInspector] public int currPP;
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

    [Header("References")]
    [SerializeField] protected GameObject damagePopupCanvasPrefab = null;

    const string WRONG_NUMBER_OF_TARGETS = "Move failed due to wrong number of targets. ({0})";


    private void Awake() {
        currPP = maxPP;
    }

    public bool CostsPP() {
        return maxPP != 0;
    }

    public string GetCommandFormat() {
        string command = "!" + commandKeyword;
        if (commandArgs > 0) {
            command += " [target]";
        }
        return command;
    }

    private bool CheckKeyword(string keyword) {
        return keyword == commandKeyword;
    }
    private bool CheckArgQuantity(int argQuantity, FightingEntity user) {
        bool res = argQuantity == commandArgs;
        if (!res) {
            this.tmp_MessagePlayer(user, string.Format(WRONG_NUMBER_OF_TARGETS, this.GetExampleCommandString()));
        }
        return argQuantity == commandArgs;
    }

    protected bool BasicValidation(string[] splitCommand, FightingEntity user) {
        if (splitCommand.Length == 0 || 
            !CheckKeyword(splitCommand[0]) || 
            !CheckArgQuantity(splitCommand.Length - 1, user) || 
            //!GameManager.Instance.battleComponents.turnManager.GetPhase().CanInputActions() ||  // Mooks can now input actions inbetween turns
            !CheckCost(user) ) {
            return false;
        }

        return true;
    }

    public bool CheckCost(FightingEntity user) {
        PlayerStats stats = user.stats;

        if (stats.hp.GetValue() <= actionCost.HP) {
            string message = user.Name + " has insufficient HP use " + name;
            Debug.Log(message);
            this.tmp_MessagePlayer(user, message);
            return false;
        }
        if (stats.mana.GetValue() < actionCost.mana) {
            string message = user.Name + " has insufficient mana use " + name;
            Debug.Log(message);
            this.tmp_MessagePlayer(user, message);
            return false;
        }
        if (currPP < actionCost.PP) {
            string message = user.Name + " has insufficient PP use " + name;
            Debug.Log(message);
            this.tmp_MessagePlayer(user, message);
            return false;
        }
        if (user is Mook && ((Mook) user).stamina.GetStamina() < actionCost.stamina) {
            string message = user.Name + " has insufficient stamina use " + name;
            Debug.Log(message);
            this.tmp_MessagePlayer(user, message);
            return false;
        }
        return true;
    }

    private void PayCost(FightingEntity user) {
        PlayerStats stats = user.stats;

        stats.hp.ApplyDelta(-actionCost.HP);
        stats.mana.ApplyDelta(-actionCost.mana);

        currPP -= actionCost.PP;
        if (user is Mook) {
            Mook mook = (Mook) user;
            mook.stamina.SetStamina(mook.stamina.GetStamina() - actionCost.stamina);
        }
    }

    public void RestorePP() {
        currPP = maxPP;
    }
    
    public virtual bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        if (!BasicValidation(splitCommand, user)) {
            return false;
        }
        return QueueAction(user, splitCommand);
    }

    public virtual bool QueueAction(FightingEntity user, string[] splitCommand) {
        if (!CheckArgQuantity(splitCommand.Length - 1, user)) {
            this.tmp_MessagePlayer(user, string.Format(WRONG_NUMBER_OF_TARGETS, this.GetExampleCommandString()));
            return false;
        }

        if (!CheckCost(user)) {
            return false;
        }

        if (targetInfo.targetType == TargetType.SINGLE && splitCommand.Length == 2) {
            int targetId = this.GetTargetIdFromString(splitCommand[1], user);
            if (targetId == -1) {
                return false;
            }
            user.SetQueuedAction(this, new List<int>{ targetId });
            return true;
        } else if (targetInfo.targetType == TargetType.ALL && splitCommand.Length == 1) {
            List<int> targetIds = GetAllPossibleTargetIds(user);
            user.SetQueuedAction(this, targetIds);
            return true;
        }

        this.tmp_MessagePlayer(user, string.Format(WRONG_NUMBER_OF_TARGETS, this.GetExampleCommandString()));

        return false;
    }

    public IEnumerator ExecuteAction(FightingEntity user, List<FightingEntity> targets, System.Action<FightResult, ActionBase> onFightEnd) {
        FightResult result = new FightResult(user, this);
        if (CheckCost(user)) {
            PayCost(user);
            if (animation != null) {
                animation.Animate(user.GetAnimController());
                yield return GameManager.Instance.time.GetController().WaitForSeconds(animation.timeBeforeEffect);
            } else {
                Debug.LogWarning("No animation set for " + user.name + "'s " + name);
            }
            result = ApplyEffect(user, targets);
            OnPostEffect(result);
        }
        onFightEnd(result, this);
        if (animation != null) {
            yield return GameManager.Instance.time.GetController().WaitForSeconds(animation.timeAfterEffect);
        }
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

    public List<int> GetAllPossibleTargetIds(FightingEntity user) {
        List<FightingEntity> possibleTargets = this.GetAllPossibleTargets(user);
        List<int> targetIds = new List<int>();
        for (int i = 0; i < possibleTargets.Count; i++) {
            targetIds.Add(i);
        }
        return targetIds;
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

    protected List<StatusAilment> InflictStatuses(FightingEntity target) {
        List<StatusAilment> inflicted = new List<StatusAilment>();
        foreach(AilmentInfliction infliction in effects.statusAilments) {
            if (target.ailmentController.TryInflictAilment(infliction)) {
                inflicted.Add(infliction.statusAilment);
            }
        }
        return inflicted;
    }

    public virtual FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        PlayerStats before, after;
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        int attackDamage = (int) (user.stats.physical.GetValue() * effects.physicalScaling + user.stats.special.GetValue() * effects.specialScaling);
        
        foreach (FightingEntity target in targets) {
            // Default damage mitigation: defense and resistance with half the scaling ratios
            int damage = attackDamage;
            // If there is a healing effect, negate the damage and ignore damage mitigation
            if (effects.heals) {
                damage = -attackDamage;
            } else {
                damage = (int)(damage + target.stats.GetDamageMultiplierArmor());
            }
            InstantiateDamagePopup(target, damage);

            before = (PlayerStats)target.stats.Clone();
            target.stats.hp.ApplyDelta(-damage);
            List<StatusAilment> inflicted = InflictStatuses(target);
            after = (PlayerStats)target.stats.Clone();

            // TODO: Unit test this
            //if (before.hp.GetValue() - after.hp.GetValue() != damage) {
            //    Debug.LogError("ERROR: Logging stats doesn't match real thing!: " + after.hp.GetValue() + " " + before.hp.GetValue() + " " + damage);
            //}


            receivers.Add(new DamageReceiver(target, before, after, inflicted));
        }
        return new FightResult(user, this, receivers);
    }

    protected void InstantiateDamagePopup(FightingEntity target, int damage) {
        DamageType damageType = effects.heals ? DamageType.HEALING : DamageType.NORMAL;
        InstantiateDamagePopup(target, damage, damageType);
    }

    protected void InstantiateDamagePopup(FightingEntity target, int damage, DamageType damageType) {
        DamagePopup popup = Instantiate(damagePopupCanvasPrefab).GetComponent<DamagePopup>();
        popup.Initialize(target, damage, damageType);
    }

    // May not be needed after enum change
    private void tmp_MessagePlayer(FightingEntity fighter, string message) {
        ActionListener actionListener = fighter.GetComponent<ActionListener>();
        if (actionListener != null && actionListener.enableReply) {
            actionListener.EchoDirectMessage(fighter.Name, message);
        }
    }

    private string GetExampleCommandString() {
        if (targetInfo.targetType == TargetType.SINGLE) {
            return "!" + this.name + " A/B/C/D";
        } else if (targetInfo.targetType == TargetType.NONE || targetInfo.targetType == TargetType.ALL) {
            return "!" + this.name;
        }

        return "";
    }

}
