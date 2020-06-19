using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Stat {
	HP,
    MAX_HP,
	MANA,
    MAX_MANA,
	PHYSICAL,
	SPECIAL,
	DEFENSE,
	RESISTANCE,
	SPEED
};

[System.Serializable]
public class PlayerStat : ICloneable {
    public Stat stat; // Set in inspector pls
    [SerializeField]
    private int baseValue; // Can set in inspector if you need to
    [SerializeField]
    bool zeroClamped = false;
    private int currentValue; // Updated on applying StatModifer

    private PriorityList<StatModifier> modifiers;

    private bool dirty = false;

    public PlayerStat(){}

    public PlayerStat(Stat stat, int baseValue, bool zeroClamped = false) {
        this.stat = stat;
        this.baseValue = baseValue;
        this.zeroClamped = zeroClamped;
    }



    public PriorityList<StatModifier> GetModifiers() {
        return this.modifiers;
    }

    public void ApplyModifier(StatModifier modifier) {
        this.modifiers.Add(modifier);
        this.SetDirty(true);
    }

    public void RemoveModifier(StatModifier modifier) {
        this.modifiers.Remove(modifier);
        this.SetDirty(true);
    }

    public void ClearModifiers() {
        this.modifiers.Clear();
        this.SetDirty(true);
    }

    public int GetBaseValue() {
        return this.baseValue;
    }

    public void SetBaseValue(int baseValue) {
        this.baseValue = baseValue;
    }


    public int GetValue() {
        if (this.dirty == false) {
            return this.currentValue;
        } else {

            int runningValue = baseValue;

            foreach (StatModifier modifier in this.modifiers) {
                runningValue = modifier.Apply(runningValue, this.baseValue, this.zeroClamped);
            }

            this.currentValue = runningValue;
            this.SetDirty(false);
            return this.currentValue;
        }
    }

    public void RandomizeBase(int minValue, int maxValue) {
        this.baseValue = (int)(maxValue * (BoxMuller.GetRandom() + 1)) + minValue;

    }

    // Helper for simply applying change
    public StatModifier ApplyDelta(float delta, StatModifier.Type type = StatModifier.Type.FLAT) {
        StatModifier modifier = new StatModifier(type, delta);
        this.ApplyModifier(modifier);
        this.GetValue();
        return modifier;
    }

    public object Clone()
    {
        PlayerStat stat = new PlayerStat(this.stat, this.baseValue, this.zeroClamped);
        PriorityList<StatModifier> clonedModifiers = new PriorityList<StatModifier>();

        foreach (StatModifier modifier in this.modifiers) {
            clonedModifiers.Add((StatModifier)modifier.Clone());
        }

        stat.modifiers = clonedModifiers;
        stat.SetDirty(true);

        return stat;
    }

    private void SetDirty(bool dirty) {
        this.dirty = dirty;
    }
};

[System.Serializable]
public class StatModifier : ICloneable, IComparable<StatModifier> {
    public enum Type {
        FLAT,
        ADD_PERCENTAGE,
        MULTIPLY_PERCENTAGE
    };

    public float value;
    public StatModifier.Type type;
    public int priority = 1;

    public StatModifier(){}

    // Note: Should be able to be both Serializable, and instantiable.
    public StatModifier(StatModifier.Type type, float value) {
        this.type = type;
        this.value = value;
    }

    // Modifer is in its own class and virtual to enable more control in case you want to do something super special
    public virtual int Apply(int runningValue, int baseValue, bool zeroClamped) {
        long resValue = runningValue;
		switch(this.type) {
			case Type.ADD_PERCENTAGE:
                resValue += (int)(baseValue * value / 100);
				break;
            case Type.MULTIPLY_PERCENTAGE:
                resValue *= (int)(baseValue * value / 100);
                break;
			case Type.FLAT:
                resValue += (int)value;
				break;
		}

        resValue = (long)Mathf.Clamp(resValue, Int32.MinValue, Int32.MinValue);

        if (resValue <= 0 && zeroClamped) {
            resValue = 0;
        }

        return (int)resValue;
    }

    public object Clone()
    {
        return new StatModifier(this.type, this.value);
    }

    public int CompareTo(StatModifier other)
    {
        if (this.priority < other.priority) return -1;
        else if (this.priority > other.priority) return 1;
        else return 0;
    }

};

[System.Serializable]
public class PlayerStats: ICloneable {
	public int level;
    public PlayerStat hp;
    public PlayerStat maxHp;
    public PlayerStat mana;
    public PlayerStat maxMana;

    public PlayerStat physical;
    public PlayerStat special;
    public PlayerStat defence;
    public PlayerStat resistance;
    public PlayerStat speed;

    private List<PlayerStat> stats;

	public PlayerStats() {
		level = 1;
        this.stats = new List<PlayerStat>();
        this.stats.Add(hp);
        this.stats.Add(mana);
        this.stats.Add(physical);
        this.stats.Add(special);
        this.stats.Add(defence);
        this.stats.Add(resistance);
        this.stats.Add(speed);

	}

	public void RandomizeStats() {
        foreach (PlayerStat stat in this.stats) {

            switch (stat.stat) {
                case Stat.HP:
                case Stat.MANA:
                    stat.RandomizeBase(1, stat.GetBaseValue());
                break;

                default:
                    stat.RandomizeBase(1, stat.GetBaseValue());
                    break;
            }
        }
	}

	public object Clone() {
        PlayerStats stats = new PlayerStats();
        stats.level = this.level;

        for (int i = 0; i < stats.stats.Count; i++) {
            stats.stats[i] = (PlayerStat)this.stats[i].Clone();
        }

		return stats;
	}

	public void LogStats() {
		Debug.Log("HP: " + hp.GetValue());
		Debug.Log("MANA: " + mana.GetValue());
		Debug.Log("PHYSICAL: " + physical.GetValue());
		Debug.Log("SPECIAL: " + special.GetValue());
		Debug.Log("DEFENSE: " + defence.GetValue());
		Debug.Log("RESISTANCE: " + resistance.GetValue());
		Debug.Log("SPEED: " + speed.GetValue());
	}

	public PlayerStat GetStat(Stat stat) {
		switch(stat) {
			case (Stat.HP):
				return this.hp;
			case (Stat.MANA):
				return this.mana;
			case (Stat.PHYSICAL):
				return this.physical;
			case (Stat.SPECIAL):
				return this.special;
			case (Stat.DEFENSE):
				return this.defence;
			case (Stat.RESISTANCE):
				return this.resistance;
			case (Stat.SPEED):
				return this.speed;
		}
		Debug.LogWarning("Tried to retrieve invalid stat.");
		return this.hp;
	}

	public int GetStatValue(Stat stat) {
		return this.GetStat(stat).GetValue();
	}


	// Resetter ------------------//
	public void ResetStats() {
        foreach (PlayerStat stat in this.stats) {
            stat.ClearModifiers();
            stat.GetValue();
        }
	}
}