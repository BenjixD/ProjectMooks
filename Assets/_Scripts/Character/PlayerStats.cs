using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// TODO: Break apart basic stats (HP) and modifiable stats (MAX_HP). They don't have even common logic to justify subclassing
public enum ModifiableStat {
    MAX_HP,
    MAX_MANA,
	PHYSICAL,
	SPECIAL,
	DEFENSE,
	RESISTANCE,
	SPEED
};

public enum RawStat {
    HP,
    MANA
}

public interface IPlayerStat {
    void SetValue(int value);
    void ApplyDelta(int delta);

    int GetValue();
}

[System.Serializable]
public class PlayerRawStat : ICloneable, IPlayerStat {
    public RawStat stat; // Set in inspector pls

    [SerializeField] // Note: the only reason it's serializable is for debugging purposes.
    protected int currentValue; // Updated on applying StatModifer
    
    protected int minValue = Int32.MinValue; // Usually only necessary for health/mana
    protected int maxValue = Int32.MaxValue;

    public PlayerRawStat(){
    }

    public PlayerRawStat(RawStat stat, int value, int minValue = Int32.MinValue, int maxValue = Int32.MaxValue) {
        this.stat = stat;
        this.currentValue = value;
    }

    public void SetMinMax(int min, int max) {
        this.minValue = min;
        this.maxValue = max;
        this.currentValue = Mathf.Clamp(this.currentValue, minValue, maxValue);
    }

    public void SetValue(int value) {
        this.currentValue = Mathf.Clamp(value, minValue, maxValue);
    }

    public void ApplyDelta(int delta) {
        this.SetValue(this.GetValue() + delta );
    }

    public int GetValue() {
        return this.currentValue;
    }

    public object Clone()
    {
        PlayerRawStat stat = new PlayerRawStat(this.stat, this.currentValue);
        return stat;
    }

};

// "baseValue" is max
// current value cannot go higher than base value.
[System.Serializable]
public class PlayerStatWithModifiers : IPlayerStat {
    public ModifiableStat stat;

    [SerializeField] // Note: the only reason it's serializable is for debugging purposes.
    protected int currentValue; // Updated on applying StatModifer

    public int originalBaseValue {get; set;}
    public int divisor = 1;
    public int growth = 1;

    [SerializeField]
    private int baseValue; // Can set in inspector if you need to

    private PriorityList<StatModifier> modifiers = new PriorityList<StatModifier>();

    private Action<int> callback = null;


    public PlayerStatWithModifiers(ModifiableStat stat, int value, int growth, int divisor = 1, Action<int> callback = null) {
        this.baseValue = value;
        this.currentValue = this.baseValue;
        this.callback = callback;
        this.originalBaseValue = this.baseValue;
        this.divisor = divisor;
        this.growth = growth;
    }

    public void SetCallback(Action<int> callback) {
        this.callback = callback;
    }

    public void ApplyBaseValueBasedOnLevel(int level) {
        this.baseValue = originalBaseValue + (level - 1) * growth;
    }

    public int GetValue() {
        int runningValue = baseValue;

        foreach (StatModifier modifier in this.modifiers) {
            runningValue = modifier.Apply(runningValue, this.baseValue);
        }

        this.currentValue = runningValue;

        if (this.callback != null) {
            this.callback(this.currentValue);
        }

        return this.currentValue;
    }

    // Note: This FORCE sets the BASE value. Use sparingly.
    public void SetValue(int value) {
        this.ClearModifiers();
        this.baseValue = value;
    }

    public void ApplyDelta(int delta) {
        this.ApplyDeltaModifier(delta, StatModifier.Type.FLAT);
    }

    public object Clone()
    {
        PlayerStatWithModifiers stat = new PlayerStatWithModifiers(this.stat, this.baseValue, this.growth, this.divisor, this.callback);
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
    public virtual int Apply(int runningValue, int baseValue) {
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

        // resValue = (long)Mathf.Clamp(resValue, minValue, maxValue);

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
    public int statPoints{get; set;}
    public PlayerRawStat hp = new PlayerRawStat(RawStat.HP, 1);
    public PlayerRawStat mana = new PlayerRawStat(RawStat.MANA, 1);

    public PlayerStatWithModifiers maxHp = new PlayerStatWithModifiers(ModifiableStat.MAX_HP, 1, 1);
    public PlayerStatWithModifiers maxMana = new PlayerStatWithModifiers(ModifiableStat.MAX_MANA, 1, 1);

    public PlayerStatWithModifiers physical = new PlayerStatWithModifiers(ModifiableStat.PHYSICAL, 1, 1);
    public PlayerStatWithModifiers special = new PlayerStatWithModifiers(ModifiableStat.SPECIAL, 1, 1);
    public PlayerStatWithModifiers defence = new PlayerStatWithModifiers(ModifiableStat.DEFENSE, 1, 1);
    public PlayerStatWithModifiers resistance = new PlayerStatWithModifiers(ModifiableStat.RESISTANCE, 1, 1);
    public PlayerStatWithModifiers speed = new PlayerStatWithModifiers(ModifiableStat.SPEED, 1, 1);

    private Dictionary<ModifiableStat, PlayerStatWithModifiers> modifiableStats;

	public PlayerStats() {
        this.Initialize();
	}


    // TODO: Break apart basic stats (HP) and modifiable stats (MAX_HP). They don't have even common logic to justify subclassing
    private void Initialize() {
		level = 1;
        this.modifiableStats = new Dictionary<ModifiableStat, PlayerStatWithModifiers>();
        this.modifiableStats.Add(ModifiableStat.MAX_HP, maxHp);
        this.modifiableStats.Add(ModifiableStat.MAX_MANA, maxMana);
        this.modifiableStats.Add(ModifiableStat.PHYSICAL, physical);
        this.modifiableStats.Add(ModifiableStat.SPECIAL, special);
        this.modifiableStats.Add(ModifiableStat.DEFENSE, defence);
        this.modifiableStats.Add(ModifiableStat.RESISTANCE, resistance);
        this.modifiableStats.Add(ModifiableStat.SPEED, speed);

        this.SetupStatPair(hp, maxHp);
        this.SetupStatPair(mana, maxMana);
    }

	public object Clone() {
        PlayerStats stats = new PlayerStats();
        stats.level = this.level;

        stats.hp = (PlayerRawStat)this.hp.Clone();
        stats.mana = (PlayerRawStat)this.mana.Clone();

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

	public PlayerStatWithModifiers GetStat(ModifiableStat stat) {
		switch(stat) {
            case (ModifiableStat.MAX_HP):
                return this.maxHp;
            case (ModifiableStat.MAX_MANA):
                return this.maxMana;
			case (ModifiableStat.PHYSICAL):
				return this.physical;
			case (ModifiableStat.SPECIAL):
				return this.special;
			case (ModifiableStat.DEFENSE):
				return this.defence;
			case (ModifiableStat.RESISTANCE):
				return this.resistance;
			case (ModifiableStat.SPEED):
				return this.speed;
		}
		Debug.LogWarning("Tried to retrieve invalid stat.");
		return this.maxHp;
	}

    public PlayerRawStat GetRawStat(RawStat stat) {
		switch(stat) {
			case (RawStat.HP):
				return this.hp;
			case (RawStat.MANA):
				return this.mana;
		}

        return this.hp;
    }



	public int GetStatValue(ModifiableStat stat) {
		return this.GetStat(stat).GetValue();
	}


	// Resetter ------------------//
	public void ResetStats() {
        foreach (KeyValuePair<ModifiableStat, PlayerStatWithModifiers> statPair in this.modifiableStats) {
            PlayerStatWithModifiers stat = statPair.Value;
            stat.ClearModifiers();
            stat.GetValue();
        }

        this.SetHPMPToMax();
	}

    public void SetHPMPToMax() {
        this.hp.SetValue(this.maxHp.GetValue());
        this.mana.SetValue(this.maxMana.GetValue());
    }

    // Level up stat. Used for hero.
    public bool LevelUpStat(ModifiableStat stat) {

        // TODO: FIX THIS
        if (!this.modifiableStats.ContainsKey(stat)) {
            Debug.LogError("ERROR: Not assignable stat");
            return false;
        }


        PlayerStatWithModifiers theStat = this.modifiableStats[stat];
        int costToLevelUp = StatLevelHelpers.GetCostToLevelUpStat(theStat.growth, theStat.divisor);
        theStat.growth += StatLevelHelpers.LEVEL_UP_GROWTH_INCREASE;
        theStat.ApplyBaseValueBasedOnLevel(this.level);
        this.statPoints -= costToLevelUp;

        return true;
    }

    // Applys level based on level. Note that this resets all stats.
    public void ApplyStatsBasedOnLevel(int level) {

        if (level <= this.level) {
            return;
        }

        statPoints += StatLevelHelpers.GetNumStatPointsForLevelUp(this.level, level);
        this.level = level;

        this.RefreshStatsBasedOnLevel();

        this.SetHPMPToMax();
    }

    public void LevelUp() {
        int additionalStatPoints = StatLevelHelpers.GetNumStatPointsForLevelUp(this.level, this.level+1);
        this.statPoints += additionalStatPoints;
        this.level++;
        this.RefreshStatsBasedOnLevel();
    }

    public Dictionary<ModifiableStat, PlayerStatWithModifiers> GetModifiableStats() {
        return this.modifiableStats;
    }

    public void RefreshStatsBasedOnLevel() {
        foreach (KeyValuePair<ModifiableStat, PlayerStatWithModifiers> statPair in this.modifiableStats) {
            PlayerStatWithModifiers statWithModifiers = statPair.Value;
            statWithModifiers.ApplyBaseValueBasedOnLevel(this.level);
        }
    }

    // Setup stat pairs for like hp/max hp and mana/max mana
    private void SetupStatPair(PlayerRawStat curStat, PlayerStatWithModifiers maxStat) {
        curStat.SetMinMax(0, maxStat.GetBaseValue());
        curStat.SetValue(maxStat.GetBaseValue());
        maxStat.SetCallback( (int maxValue) => curStat.SetMinMax(0, maxValue) );
        //maxStat.divisor = 3;
    }

    // https://leagueoflegends.fandom.com/wiki/Armor
    public float GetDamageMultiplierArmor() {
        return ((float)100 / (100 + this.defence.GetValue()));
    }

    public float GetDamageMultiplierResistence() {
        return ((float)100 / (100 + this.resistance.GetValue()));
    }
}