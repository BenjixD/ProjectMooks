
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A party holds an array of FightingEntities
// T should be one of Player, Enemy
public class Party<T> where T : Fighter
{
    public const int maxPlayers = 4;
    private T[] members = new T[maxPlayers];

    public void SetMember(int index, T member) {
        this.members[index] = member;
    }

    public T[] GetMembers() {
        return members;
    }

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
        return members[Random.Range(0, castedMembers.Count)].fighter.targetId;
    }

    public U[] GetFighters<U>() where U : FightingEntity {
        U[] array = new U[maxPlayers];
        for (int i = 0; i < maxPlayers; i++) {
            if (members[i] != null) {
                array[i] = (U)members[i].fighter;
            } else {
                array[i] = null;
            }
        }

        return array;
    }

    public List<U> GetActiveFighters<U>() where U : FightingEntity {
        U[] fighters = this.GetFighters<U>();
        List<U> activeFighters = new List<U>();
        for (int i = 0; i < fighters.Length; i++) {
            if (fighters[i] != null) {
                activeFighters.Add(fighters[i]);
            }
        }

        return activeFighters;
    }



    public static Color IndexToColor(int index) {
        if (typeof(T) == typeof(EnemyObject)) {
            return GameManager.Instance.party.enemyColor;
        }  else {
            return GameManager.Instance.party.IndexToColor(index);
        }
    }
}