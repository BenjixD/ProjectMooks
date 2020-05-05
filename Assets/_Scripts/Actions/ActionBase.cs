using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionBase : ScriptableObject {
    public string name;
    [TextArea]
    public string description;
    [Tooltip("The main command word, e.g., \"a\".")]
    public string commandKeyword;
    [Tooltip("The number of arguments following the keyword.")]
    public int commandArgs;

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
}
