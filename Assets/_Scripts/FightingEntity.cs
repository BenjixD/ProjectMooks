
using UnityEngine;
using System.Collections.Generic;

public class FightingEntity : MonoBehaviour
{
    public string Name;
    public PlayerStats stats;

    public HashSet<string> modifiers = new HashSet<string>();
}