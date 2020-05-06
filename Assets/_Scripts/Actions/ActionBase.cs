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
    [Tooltip("The name of the animation played for the user of the attack.")]
    public string userAnimName;

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
    
    public void ExecuteAction(FightingEntity user, List<FightingEntity> targets) {
        user.Animate(userAnimName, false);
        ApplyEffect(user, targets);
    }

    public abstract void ApplyEffect(FightingEntity user, List<FightingEntity> targets);
}
