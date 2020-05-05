
using UnityEngine;
using System.Collections.Generic;



public class BattleCommand {
    public BattleOption option = BattleOption.ATTACK;
    public int target = 0; // Remember that chat will enter a number of target + 1
    public bool targetIsEnemy = true; // TODO: Allow for self targetting

}

public enum BattleOption {
    ATTACK = 0,
    MAGIC = 1,
    DEFEND = 2,
    LENGTH = 3
};


public class FightingEntity : MonoBehaviour
{
    public string Name;
    public PlayerStats stats;

    public BattleCommand command;

    public HashSet<string> modifiers = new HashSet<string>();
}