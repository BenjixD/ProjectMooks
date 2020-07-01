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

    [SerializeField] // Note: the only reason it's serializable is for debugging purposes.
    protected int currentValue; // Updated on applying StatModifer
    
    protected int minValue = Int32.MinValue; // Usually only necessary for health/mana
    protected int maxValue = Int32.MaxValue;

    public PlayerStat(){
    }

    public PlayerStat(Stat stat, int value, int minValue = Int32.MinValue, int maxValue = Int32.MaxValue) {
        this.stat = stat;
        this.currentValue = value;
    }

    public void SetMinMax(int min, int max) {
        this.minValue = min;
        this.maxValue = max;
        this.currentValue = Mathf.Clamp(this.currentValue, minValue, maxValue);
    }

    public virtual void SetValue(int value) {
        this.currentValue = Mathf.Clamp(value, minValue, maxValue);
    }

    public virtual void ApplyDelta(int delta) {
        this.SetValue(this.GetValue() + delta );
    }

    public virtual int GetValue() {
        return this.currentValue;
    }

    public virtual object Clone()
    {
        PlayerStat stat = new PlayerStat(this.stat, this.currentValue);
        return stat;
    }

};

// "baseValue" is max
// current value cannot go higher than base value.
[System.Serializable]
public class PlayerStatWithModifiers : PlayerStat {

    public int originalBaseValue {get; set;}
    public int divisor = 1;

    [SerializeField]
    private int baseValue; // Can set in inspector if you need to

    private PriorityList<StatModifier> modifiers = new PriorityList<StatModifier>();

    private Action<int> callback = null;


    public PlayerStatWithModifiers(Stat stat, int value, int minValue = Int32.MinValue, int maxValue = Int32.MaxValue, Action<int> callback = null)
    : base(stat, value, minValue, maxValue) {
        this.baseValue = value;
        this.callback = callback;
        this.originalBaseValue = this.baseValue;
    }

    public void SetCallback(Action<int> callback) {
        this.callback = callback;
    }

    public override int GetValue() {

        int runningValue = baseValue;

        foreach (StatModifier modifier in this.modifiers) {
            runningValue = modifier.Apply(runningValue, this.baseValue, this.minValue, this.maxValue);
        }

        this.currentValue = runningValue;

        if (this.callback != null) {
            this.callback(this.currentValue);
        }

        return this.currentValue;
    }

    // Note: This FORCE sets the BASE value. Use sparingly.
    public override void SetValue(int value) {
        this.ClearModifiers();
        this.baseValue = Mathf.Clamp(value, minValue, maxValue);
    }

    public override void ApplyDelta(int delta) {
        this.ApplyDeltaModifier(delta, StatModifier.Type.FLAT);
    }

    public override object Clone()
    {
        PlayerStatWithModifiers stat = new PlayerStatWithModifiers(this.stat, this.baseValue, this.minValue, this.maxValue, this.callback);
        PriorityList<StatModifier> clonedModifiers = new PriorityList<StatModifier>();

        foreach (StatModifier modifier in this.modifiers) {
            clonedModifiers.Add((StatModifier)modifier.Clone());
        }

        stat.modifiers = clonedModifiers;

        return stat;
    }

    // Helper for simply applying change
    public StatModifier ApplyDeltaModifier(float delta, StatModifier.Type type = StatModifier.Type.FLAT) {
        StatModifier modifier = new StatModifier(type, delta);
        this.ApplyModifier(modifier);
        this.GetValue();
        return modifier;
    }

    public int GetBaseValue() {
        return this.baseValue;
    }

    public void SetBaseValue(int value, bool setOriginalValue = false) {
        this.baseValue = value;
        if (setOriginalValue) {
            this.originalBaseValue = this.baseValue;
        }
    }

    // Modifier helpers
    public PriorityList<StatModifier> GetModifiers() {
        return this.modifiers;
    }

    public void ApplyModifier(StatModifier modifier) {
        this.modifiers.Add(modifier);
    }

    public void RemoveModifier(StatModifier modifier) {
        this.modifiers.Remove(modifier);
    }

    public void ClearModifiers() {
        this.modifiers.Clear();
    }

    public void RandomizeBase(int minValue, int maxValue) {
        this.SetBaseValue((int)(maxValue * (BoxMuller.GetRandom() + 1)) + minValue, true);
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
    public virtual int Apply(int runningValue, int baseValue, int minValue, int maxValue) {
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

        resValue = (long)Mathf.Clamp(resValue, minValue, maxValue);

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
	public int level = 1;
    public int statPoints = 0;
    public PlayerStat hp = new PlayerStat(Stat.HP, 1);
    public PlayerStatWithModifiers maxHp = new PlayerStatWithModifiers(Stat.MAX_HP, 1);
    public PlayerStat mana = new PlayerStat(Stat.MANA, 1);
    public PlayerStatWithModifiers maxMana = new PlayerStatWithModifiers(Stat.MAX_MANA, 1);

    public PlayerStatWithModifiers physical = new PlayerStatWithModifiers(Stat.PHYSICAL, 1);
    public PlayerStatWithModifiers special = new PlayerStatWithModifiers(Stat.SPECIAL, 1);
    public PlayerStatWithModifiers defence = new PlayerStatWithModifiers(Stat.DEFENSE, 1);
    public PlayerStatWithModifiers resistance = new PlayerStatWithModifiers(Stat.RESISTANCE, 1);
    public PlayerStatWithModifiers speed = new PlayerStatWithModifiers(Stat.SPEED, 1);

    private Dictionary<Stat, PlayerStatWithModifiers> modifiableStats;

	public PlayerStats() {
        this.Initialize();
	}

    private void Initialize() {
		level = 1;
        this.modifiableStats = new Dictionary<Stat, PlayerStatWithModifiers>();
        this.modifiableStats.Add(Stat.MAX_HP, maxHp);
        this.modifiableStats.Add(Stat.MAX_MANA, maxMana);
        this.modifiableStats.Add(Stat.PHYSICAL, physical);
        this.modifiableStats.Add(Stat.SPECIAL, special);
        this.modifiableStats.Add(Stat.DEFENSE, defence);
        this.modifiableStats.Add(Stat.RESISTANCE, resistance);
        this.modifiableStats.Add(Stat.SPEED, speed);

        this.SetupStatPair(hp, maxHp);
        this.SetupStatPair(mana, maxMana);
    }

	public void RandomizeStats() {
        foreach (KeyValuePair<Stat, PlayerStatWithModifiers> statPair in this.modifiableStats) {
            PlayerStatWithModifiers stat = statPair.Value;
            stat.RandomizeBase(1, stat.GetBaseValue());

            this.hp.SetValue(this.maxHp.GetValue());
            this.mana.SetValue(this.maxMana.GetValue());            
        }
	}

	public object Clone() {
        PlayerStats stats = new PlayerStats();
        stats.level = this.level;

        stats.hp = (PlayerStat)this.hp.Clone();
        stats.mana = (PlayerStat)this.mana.Clone();

        stats.maxHp = (PlayerStatWithModifiers)this.maxHp.Clone();
        stats.maxMana = (PlayerStatWithModifiers)this.maxMana.Clone();
        stats.physical = (PlayerStatWithModifiers)this.physical.Clone();
        stats.special = (PlayerStatWithModifiers)this.special.Clone();
        stats.defence = (PlayerStatWithModifiers)this.defence.Clone();
        stats.resistance = (PlayerStatWithModifiers)this.resistance.Clone();
        stats.speed = (PlayerStatWithModifiers)this.speed.Clone();

        stats.Initialize();

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
            case (Stat.MAX_HP):
                return this.maxHp;
			case (Stat.MANA):
				return this.mana;
            case (Stat.MAX_MANA):
                return this.maxMana;
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
        foreach (KeyValuePair<Stat, PlayerStatWithModifiers> statPair in this.modifiableStats) {
            PlayerStatWithModifiers stat = statPair.Value;
            stat.ClearModifiers();
            stat.GetValue();
        }
	}

    // Level up stat. Used for hero.
    public bool LevelUpStat(Stat stat) {
        if (!this.modifiableStats.ContainsKey(stat)) {
            Debug.LogError("ERROR: Not assignable stat");
            return false;
        }

        PlayerStatWithModifiers theStat = this.modifiableStats[stat];

        int costToLevelUp = StatLevelHelpers.GetCostToLevelUpStat(theStat.GetBaseValue(), theStat.divisor);
        if (costToLevelUp > this.statPoints) {
            Debug.LogWarning("Not enough stat points to level up!");
            return false;
        }

        this.statPoints -= costToLevelUp;
        theStat.SetBaseValue(theStat.GetBaseValue() + theStat.divisor);
        return true;
    }

    // Applys level assuming that 
    public void ApplyStatsBasedOnLevel(int level, bool manaless = false) {

        if (level <= this.level) {
            return;
        }

        statPoints += StatLevelHelpers.GetNumStatPointsForLevelUp(this.level, level);
        this.level = level;

        if (statPoints == 0) {
          return;
        }

        List<ValueWeight<PlayerStatWithModifiers>> statsWithModifiersWeights = new List<ValueWeight<PlayerStatWithModifiers>>();
        do {
            statsWithModifiersWeights = new List<ValueWeight<PlayerStatWithModifiers>>();
            foreach (KeyValuePair<Stat, PlayerStatWithModifiers> statPair in this.modifiableStats) {
                PlayerStatWithModifiers statWithModifiers = statPair.Value;
                if (statWithModifiers.stat == Stat.MAX_MANA && manaless == true) {
                    continue;
                }

                int cost = StatLevelHelpers.GetCostToLevelUpStat(statWithModifiers.GetBaseValue(), statWithModifiers.divisor);

                if (statPoints >=  cost) {
                    statsWithModifiersWeights.Add(new ValueWeight<PlayerStatWithModifiers>(statWithModifiers, (float)statWithModifiers.originalBaseValue / statWithModifiers.divisor));
                }
            }

            if (statsWithModifiersWeights.Count <= 0) {
                break;
            }

            RandomPool<PlayerStatWithModifiers> statsPool = new RandomPool<PlayerStatWithModifiers>(statsWithModifiersWeights);
            PlayerStatWithModifiers theStat = statsPool.PickOne();
            int costToLevelUp = StatLevelHelpers.GetCostToLevelUpStat(theStat.GetBaseValue(), theStat.divisor);
            theStat.SetBaseValue(theStat.GetBaseValue() + theStat.divisor);

            statPoints -= costToLevelUp;
        } while (statsWithModifiersWeights.Count > 0);

        this.hp.SetValue(this.maxHp.GetValue());
        this.mana.SetValue(this.maxMana.GetValue());

    }

    public void LevelUp() {
        int additionalStatPoints = StatLevelHelpers.GetNumStatPointsForLevelUp(this.level, this.level+1);
        this.statPoints += additionalStatPoints;
        this.level++;
    }

    public Dictionary<Stat, PlayerStatWithModifiers> GetModifiableStats() {
        return this.modifiableStats;
    }

    // Setup stat pairs for like hp/max hp and mana/max mana
    private void SetupStatPair(PlayerStat curStat, PlayerStatWithModifiers maxStat) {
        curStat.SetMinMax(0, maxStat.GetBaseValue());
        curStat.SetValue(maxStat.GetBaseValue());
        maxStat.SetCallback( (int maxValue) => curStat.SetMinMax(0, maxValue) );

        maxStat.divisor = 3;
    }
}