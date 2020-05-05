﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType {
    ATTACK,
    FIRE_ONE,
    FIRE_ALL,
    DEFEND
}

public abstract class ActionBase : ScriptableObject {
    public string name;
    [TextArea]
    public string description;
    public ActionType actionType;

    public abstract bool TryChooseAction(Player user, string[] splitCommand);
    
    // TODO: update params
    public abstract void ExecuteAction(Player user, Player[] targets);
}
