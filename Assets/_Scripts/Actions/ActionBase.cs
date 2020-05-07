using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TargetType{
    MY_TEAM,
    ENEMY_TEAM,
    ALL_TEAMS
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


    public TargetType targetIdType;

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

    // Returns true iff input is an integer, non-negative, and no less than the number of enemies
    protected bool TargetIdValidation(string targetStr) {
        int targetId;
        if (!int.TryParse(targetStr, out targetId)) {
            return false;
        }
        if (targetId < 0 || targetId >= GameManager.Instance.turnController.stage.GetEnemies().Count) {
            return false;
        }
        return true;
    }

    public abstract bool TryChooseAction(FightingEntity user, string[] splitCommand);
    
    public FightResult ExecuteAction(FightingEntity user, List<FightingEntity> targets) {
        user.Animate(userAnimName, false);
        return ApplyEffect(user, targets);
    }

    public List<FightingEntity> GetPotentialTargets(FightingEntity user) {
        if (targetIdType == TargetType.ALL_TEAMS) {
            return GameManager.Instance.turnController.stage.GetAllFightingEntities();
        }
        List<FightingEntity> potentialTargets;
        List<FightingEntity> enemies = new List<FightingEntity>(GameManager.Instance.turnController.stage.GetEnemies());
        List<FightingEntity> players = new List<FightingEntity>(GameManager.Instance.party.GetPlayersInPosition());

        if (user.isEnemy()) {
            potentialTargets = targetIdType == TargetType.MY_TEAM ? enemies : players;
        } else {
            potentialTargets = targetIdType == TargetType.MY_TEAM ? players : enemies;
        }

        return potentialTargets;
    }

    public List<FightingEntity> GetTargets(FightingEntity user, List<int> targetIds){ 
        List<FightingEntity> potentialTargets = GetPotentialTargets(user);
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

        List<FightingEntity> targets = GetPotentialTargets(user);

        if (targetId < 0 || targetId >= targets.Count) {
            return -1;
        }

        return targetId;
    }

    public abstract FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets);
}
