
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface TestInterface {
    FightingEntity GetPlayer();
}


// T should be one of Player, enemy
public class Party<T> where T : FightingEntity
{
    public const int maxPlayers = 4;
    public T[] members = new T[maxPlayers]; 

    public List<T> GetActiveMembers() {
        List<T> activeMembers = new List<T>();
        for (int i = 0; i < members.Length; i++) {
            if (members[i] != null) {
                activeMembers.Add(members[i]);
            }
        }

        return activeMembers;
    }

    public int GetRandomActiveIndex() {
        List<T> castedMembers = GetActiveMembers();
        return members[Random.Range(0, castedMembers.Count)].targetId;
    }
    
}