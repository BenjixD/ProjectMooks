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

public abstract class ActionBase : ScriptableObject {
    public string name;
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

    private bool CheckKeyword(string keyword) {
        return keyword == commandKeyword;
    }
    private bool CheckArgQuantity(int argQuantity) {
        return argQuantity == commandArgs;
    }

    protected bool BasicValidation(string[] splitCommand) {
        if (splitCommand.Length == 0 || !CheckKeyword(splitCommand[0]) || !CheckArgQuantity(splitCommand.Length - 1) || !GameManager.Instance.turnController.CanInputActions() ) {
            return false;
        }
        return true;
    }


    public abstract bool TryChooseAction(FightingEntity user, string[] splitCommand);
    
    public FightResult ExecuteAction(FightingEntity user, List<FightingEntity> targets) {
        user.Animate(userAnimName, false);
        return ApplyEffect(user, targets);
    }

    public List<FightingEntity> GetPotentialActiveTargets(FightingEntity user) {
        if (targetInfo.targetTeam == TargetTeam.BOTH_TEAMS) {
            return GameManager.Instance.turnController.stage.GetAllFightingEntities();
        }
        List<FightingEntity> potentialTargets;
        List<FightingEntity> enemies = new List<FightingEntity>(GameManager.Instance.turnController.stage.GetActiveEnemies());
        List<FightingEntity> players = new List<FightingEntity>(GameManager.Instance.turnController.stage.GetActivePlayers());

        if (user.isEnemy()) {
            potentialTargets = targetInfo.targetTeam == TargetTeam.MY_TEAM ? enemies : players;
        } else {
            potentialTargets = targetInfo.targetTeam == TargetTeam.MY_TEAM ? players : enemies;
        }

        return potentialTargets;
    }

    public List<FightingEntity> GetTargets(FightingEntity user, List<int> targetIds){ 
        List<FightingEntity> potentialTargets = GetPotentialActiveTargets(user);
        List<FightingEntity> targets = new List<FightingEntity>();
        foreach (int target in targetIds) {
            if (target < potentialTargets.Count) {
                targets.Add(potentialTargets[target]);
            }
        }

        return targets;
    }

    public List<FightingEntity> GetTargets(FightingEntity user, int targetId) {
        return this.GetTargets(user, new List<int>{targetId});
    }

    protected int GetTargetIdFromString(string str, FightingEntity user) {
        int targetId;
        if (!int.TryParse(str, out targetId)) {
            return -1;
        }

        List<FightingEntity> targets = GetPotentialActiveTargets(user);

        bool foundTargetId = false;

        foreach (FightingEntity target in targets) {
            if (target.targetId == targetId) {
                foundTargetId = true;
                break;
            }
        }

        if (!foundTargetId) {
            return -1;
        }

        return targetId;
    }

    public abstract FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets);
}
