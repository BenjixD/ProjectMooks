using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public abstract class Party : MonoBehaviour {
    public List<Color> targetColors;
    protected Fighter[] fighters;

    // Initialize fighters array
    public abstract void Initialize();

    public void SetFighter(int index, Fighter fighter) {
        this.fighters[index] = fighter;
    }

    public T[] GetFighters<T>() where T : Fighter {
        return fighters.Cast<T>().ToArray();
    } 

    public List<U> GetActiveFighters<U>() where U : Fighter {
        U[] fighters = this.GetFighters<U>();
        List<U> activeFighters = new List<U>();
        for (int i = 0; i < fighters.Length; i++) {
            if (fighters[i] != null) {
                activeFighters.Add(fighters[i]);
            }
        }

        return activeFighters;
    }

    public U[] GetFighterObjects<U>() where U : FightingEntity {
        return fighters.Select( fighter => fighter != null ? fighter.fighter : null ).ToArray().Cast<U>().ToArray();
    }

    public List<U> GetActiveFighterObjects<U>() where U : FightingEntity {
        U[] fighters = this.GetFighterObjects<U>();
        List<U> activeFighters = new List<U>();
        for (int i = 0; i < fighters.Length; i++) {
            if (fighters[i] != null) {
                activeFighters.Add(fighters[i]);
            }
        }

        return activeFighters;
    }

    public int GetRandomActiveIndex() {
        List<Fighter> fighters = GetActiveFighters<Fighter>();
        return this.fighters[Random.Range(0, fighters.Count)].fighter.targetId;
    }

    public Color IndexToColor(int index) {
        return targetColors[index];
    }
}
