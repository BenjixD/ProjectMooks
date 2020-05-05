using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TargetType{
    MY_TEAM,
    ENEMY_TEAM
}
public abstract class ActionBase : ScriptableObject {
    public string name;
    [TextArea]
    public string description;
    [Tooltip("The main command word, e.g., \"a\".")]
    public string commandKeyword;
    [Tooltip("The number of arguments following the keyword.")]
    public int commandArgs;


    public TargetType targetIdType;

    private bool CheckKeyword(string keyword) {
        return keyword == commandKeyword;
    }
    private bool CheckArgQuantity(int argQuantity) {
        return argQuantity == commandArgs;
    }

    protected bool BasicValidation(string[] splitCommand) {
        if (splitCommand.Length == 0 || !CheckKeyword(splitCommand[0]) || !CheckArgQuantity(splitCommand.Length - 1) || !GameManager.Instance.battleController.inputActionsPhase ) {
            return false;
        }
        return true;
    }

    public abstract bool TryChooseAction(FightingEntity user, string[] splitCommand);
    
    // TODO: update params
    public abstract void ExecuteAction(FightingEntity user, List<FightingEntity> targets);

    public List<FightingEntity> GetTargets(FightingEntity user, List<int> targetIds){ 
        List<FightingEntity> potentialTargets;
        List<FightingEntity> enemies = new List<FightingEntity>(GameManager.Instance.battleController.enemies);
        List<FightingEntity> players = new List<FightingEntity>(GameManager.Instance.party.GetPlayersInPosition());

        if (user.isEnemy()) {
            potentialTargets = targetIdType == TargetType.MY_TEAM ? enemies : players;
        } else {
            potentialTargets = targetIdType == TargetType.MY_TEAM ? players : enemies;
        }

        List<FightingEntity> targets = new List<FightingEntity>();
        foreach (int target in targetIds) {
            targets.Add(potentialTargets[target]);
        }

        return targets;
    }
}
