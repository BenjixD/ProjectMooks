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

public class ActionChoiceResult {
    public enum State {
        QUEUED,
        INVALID_SYNTAX,
        INVALID_TARGET,
        INSUFFICIENT_COST,
        UNMATCHED,
        CANNOT_BE_QUEUED
    }
    
    public State state;
    public List<string> messages;

    public ActionChoiceResult() {
        this.state = State.QUEUED;
        this.messages = new List<string>();
    }

    public ActionChoiceResult(State state) {
        this.state = state;
        this.messages = new List<string>();
    }

    public ActionChoiceResult(State state, List<string> messages) {
        this.state = state;
        this.messages = messages;
    }

    public void SetState(State state) {
        this.state = state;
    }

    public void AddMessage(string message) {
        this.messages.Add(message);
    }

    public string GetAmalgamatedMessage() {
        string s = "";
        foreach(string msg in messages) {
            s += msg + "\n";
        }
        return s;
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
    protected BattleFight _battleFight; // The BattleFight in which this action is being used

    public class Messages {
        public const string KEYWORD_UNMATCHED = "Move keyword ({0}) does not match provided: ({1})";
        public const string INCORRECT_ARGUMENTS = "Invalid arguments for move ({0}), expected: ({1})";
        public const string INSUFFICIENT_COST = "Insufficient ({0}) to use move ({1}), expected: ({2})";
        public const string WRONG_NUMBER_OF_TARGETS = "Move ({0}) failed due to wrong number of targets. ({1})";
        public const string TARGET_DOES_NOT_EXIST = "Move ({0}) failed due to invalid target. ({1})";
        public const string QUEUED_MOVE = "Move ({0}) has been queued! ({1})";
    }
    
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

    private bool CheckKeyword(string keyword, ActionChoiceResult response) {
        bool res = keyword == commandKeyword;
        if(!res) {
            response.SetState(ActionChoiceResult.State.UNMATCHED);
            response.AddMessage(string.Format(Messages.KEYWORD_UNMATCHED, commandKeyword, keyword));
        }
        return res;
    }

    private bool CheckArgQuantity(int argQuantity, FightingEntity user, ActionChoiceResult response) {
        bool res = argQuantity == commandArgs;
        if(!res) {
            response.SetState(ActionChoiceResult.State.INVALID_SYNTAX);
            response.AddMessage(string.Format(Messages.INCORRECT_ARGUMENTS, commandKeyword, this.GetExampleCommandString()));
        }
        return res;
    }

    private bool CheckValidTarget(FightingEntity user, string[] splitCommand, ActionChoiceResult response) {
        bool res = false;   
        if (targetInfo.targetType == TargetType.SINGLE && splitCommand.Length == 2) {
            int targetId = this.GetTargetIdFromString(splitCommand[1], user);
            res = targetId != -1;
        }
        else if(targetInfo.targetType == TargetType.ALL && splitCommand.Length == 1) {
            res = true;
        }

        if(!res) {
            response.SetState(ActionChoiceResult.State.INVALID_TARGET);
            response.AddMessage(string.Format(Messages.WRONG_NUMBER_OF_TARGETS, commandKeyword, this.GetExampleCommandString()));
        }
        return res;
    }

    private bool CheckCost(FightingEntity user, ActionChoiceResult response) {
        PlayerStats stats = user.stats;

        if (stats.hp.GetValue() <= actionCost.HP) {
            response.SetState(ActionChoiceResult.State.INSUFFICIENT_COST);
            response.AddMessage(string.Format(Messages.INSUFFICIENT_COST, "HP", commandKeyword, actionCost.HP));
            return false;
        } else if(stats.mana.GetValue() < actionCost.mana) {
            response.SetState(ActionChoiceResult.State.INSUFFICIENT_COST);
            response.AddMessage(string.Format(Messages.INSUFFICIENT_COST, "MANA", commandKeyword, actionCost.mana));
            return false;
        } else if(currPP < actionCost.PP) {
            response.SetState(ActionChoiceResult.State.INSUFFICIENT_COST);
            response.AddMessage(string.Format(Messages.INSUFFICIENT_COST, "PP", commandKeyword, actionCost.PP));
            return false;
        } else if(user is Mook && ((Mook) user).stamina.GetStamina() < actionCost.stamina) {
            response.SetState(ActionChoiceResult.State.INSUFFICIENT_COST);
            response.AddMessage(string.Format(Messages.INSUFFICIENT_COST, "STAMINA", commandKeyword, actionCost.stamina));
            return false;
        }

        return true;
    }

    public bool CheckCost(FightingEntity user) {
        return CheckCost(user, new ActionChoiceResult());
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
    
    public virtual ActionChoiceResult TryChooseAction(FightingEntity user, string[] splitCommand) {
        ActionChoiceResult res = new ActionChoiceResult();
        if(CheckKeyword(splitCommand.Length == 0 ? "" : splitCommand[0], res) &&
            CheckArgQuantity(splitCommand.Length - 1, user, res) &&
            CheckCost(user, res) &&
            CheckValidTarget(user, splitCommand, res)) {
            res.SetState(ActionChoiceResult.State.QUEUED);
            res.AddMessage(string.Format(Messages.QUEUED_MOVE, commandKeyword, description));
            QueueAction(user, splitCommand);
        }

        return res;
    }

    public virtual bool QueueAction(FightingEntity user, string[] splitCommand) {
        if (targetInfo.targetType == TargetType.SINGLE && splitCommand.Length == 2) {
            int targetId = this.GetTargetIdFromString(splitCommand[1], user);
            user.SetQueuedAction(this, new List<int>{ targetId });
        } else if (targetInfo.targetType == TargetType.ALL && splitCommand.Length == 1) {
            List<int> targetIds = GetAllPossibleActiveTargets(user).Map(target => target.targetId);
            user.SetQueuedAction(this, targetIds);
        }

        return true;
    }

    public virtual IEnumerator ExecuteAction(FightingEntity user, List<FightingEntity> targets, BattleFight battleFight) {
        _battleFight = battleFight;
        FightResult result = new FightResult(user, this);
        if (CheckCost(user)) {
            PayCost(user);
            if (animation != null) {
                GameManager.Instance.time.GetController().StartCoroutine(animation.Animate(user, targets));
                yield return GameManager.Instance.time.GetController().WaitForSeconds(animation.GetAnimWindup());
            } else {
                Debug.LogWarning("No animation set for " + user.name + "'s " + name);
            }
            result = ApplyEffect(user, targets);
            OnPostEffect(result);
        }
        _battleFight.EndFight(result, this);
        if (animation != null) {
            yield return GameManager.Instance.time.GetController().WaitForSeconds(animation.GetAnimCooldown());
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

    public List<FightingEntity> GetTargets(FightingEntity user, List<int> targetIds){ 
        if (targetInfo.targetTeam == TargetTeam.BOTH_TEAMS) {
           return GetAllPossibleActiveTargets(user);
        }
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

    private string GetExampleCommandString() {
        if (targetInfo.targetType == TargetType.SINGLE) {
            return "!" + this.commandKeyword + " A/B/C/D";
        } else if (targetInfo.targetType == TargetType.NONE || targetInfo.targetType == TargetType.ALL) {
            return "!" + this.commandKeyword;
        }

        return "";
    }
}
