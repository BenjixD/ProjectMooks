using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Stat {
	HP,
	MANA,
	PHYSICAL,
	SPECIAL,
	DEFENSE,
	RESISTANCE,
	SPEED
};

[System.Serializable]
public class PlayerStat : ICloneable {
    public Stat stat; // Set in inspector pls
    public int baseValue; // Can set in inspector if you need to
    public int minValue = -2147483648; // either 0 or -2147483648 for most cases
    public int maxValue = 2147483647; // 2147483647 for most cases

    private int currentValue; // Updated on applying StatModifer

    private List<StatModifier> modifiers;

    private bool dirty = false;

    public PlayerStat(){}

    public PlayerStat(Stat stat, int baseValue, int minValue = -2147483648, int maxValue = 2147483647) {
        this.stat = stat;
        this.baseValue = baseValue;
        this.minValue = -2147483648;
        this.maxValue = 2147483647;
    }

    public List<StatModifier> GetModifiers() {
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

    public int GetCurrentValue() {
        if (this.dirty == false) {
            return this.currentValue;
        } else {

            int runningValue = baseValue;

            foreach (StatModifier modifier in this.modifiers) {
                runningValue = modifier.Apply(runningValue, this.baseValue, this.minValue, this.maxValue);
            }

            this.currentValue = runningValue;
            this.SetDirty(false);
            return this.currentValue;
        }
    }

    public void RandomizeBase(int minValue, int maxValue) {
        this.baseValue = (int)(maxValue * (BoxMuller.GetRandom() + 1)) + minValue;
    }

    // Helper for simply applying flat change
    public int ApplyFlatChange(int change) {
        this.ApplyModifier(new StatModifier(StatModifier.Type.FLAT, change));
        return this.GetCurrentValue();
    }

    // Helper for applying percentage change
    public int ApplyPercentageChange(float change) {
        this.ApplyModifier(new StatModifier(StatModifier.Type.PERCENTAGE, change));
        return this.GetCurrentValue();
    }

    public object Clone()
    {
        PlayerStat stat = new PlayerStat(this.stat, this.baseValue, this.minValue, this.maxValue);
        List<StatModifier> clonedModifiers = new List<StatModifier>();

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
public class StatModifier : ICloneable {
    public enum Type {
        FLAT,
        PERCENTAGE
    };

    public float value;
    public StatModifier.Type type;

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
			case Type.PERCENTAGE:
                resValue += (int)(baseValue * value / 100);
				break;
			case Type.FLAT:
                resValue += (int)value;
				break;
		}

        if (resValue <= minValue) {
            resValue = minValue;
        }

        if (resValue >= maxValue) {
            resValue = maxValue;
        }

        return (int)resValue;
    }

    public object Clone()
    {
        return new StatModifier(this.type, this.value);
    }
};

[System.Serializable]
public class PlayerStats: ICloneable {
	public int level;

	// Max Stats
	public int maxHp;
	public int maxMana;
	public int maxPhysical;
	public int maxSpecial;
	public int maxDefense;
	public int maxResistance;
	public int maxSpeed;
	public bool isRare;

	// Base Stats
	[SerializeField]
	private int _hp;
	[SerializeField]
	private int _mana;
	[SerializeField]
	private int _physical;
	[SerializeField]
	private int _special;
	[SerializeField]
	private int _defense;
	[SerializeField]
	private int _resistance;
	[SerializeField]
	private int _speed;

	// Regen Fields
	[SerializeField]
	private int _hpRegen;
	[SerializeField]
	private int _manaRegen;

	public PlayerStats() {
		level = 1;
	}

	public PlayerStats(PlayerStats stats) {
		maxHp = stats.maxHp;
		maxMana = stats.maxMana;
		maxPhysical = stats.maxPhysical;
		maxSpecial = stats.maxSpecial;
		maxDefense = stats.maxDefense;
		maxResistance = stats.maxResistance;
		maxSpeed = stats.maxSpeed;
	}

	public void RandomizeStats() {
		maxHp = (int)(maxHp * (BoxMuller.GetRandom() + 1)) + 1;
		maxMana = (int)(maxMana * (BoxMuller.GetRandom() + 1)) + 1;
		maxSpeed = (int)(maxSpeed * (BoxMuller.GetRandom() + 1));

		maxPhysical = (int)(maxPhysical * (BoxMuller.GetRandom() + 1));
		maxSpecial = (int)(maxSpecial * (BoxMuller.GetRandom() + 1));

		maxDefense = (int)(maxDefense * (BoxMuller.GetRandom() + 1));
		maxResistance = (int)(maxResistance * (BoxMuller.GetRandom() + 1));
	}

	public object Clone() {
		return this.MemberwiseClone();
	}

	public void LogStats() {
		Debug.Log("HP: " + _hp);
		Debug.Log("MANA: " + _mana);
		Debug.Log("PHYSICAL: " + _physical);
		Debug.Log("SPECIAL: " + _special);
		Debug.Log("DEFENSE: " + _defense);
		Debug.Log("RESISTANCE: " + _resistance);
		Debug.Log("SPEED: " + _speed);
	}

	public int GetStat(Stat stat) {
		switch(stat) {
			case (Stat.HP):
				return GetHp();
			case (Stat.MANA):
				return GetMana();
			case (Stat.PHYSICAL):
				return GetPhysical();
			case (Stat.SPECIAL):
				return GetSpecial();
			case (Stat.DEFENSE):
				return GetDefense();
			case (Stat.RESISTANCE):
				return GetResistance();
			case (Stat.SPEED):
				return GetSpeed();
		}
		Debug.LogWarning("Tried to retrieve invalid stat.");
		return GetHp();
	}

	public void ModifyStat(Stat stat, int value) {
		switch(stat) {
			case (Stat.HP):
				SetHp(GetHp() + value);
				break;
			case (Stat.MANA):
				SetMana(GetMana() + value);
				break;
			case (Stat.PHYSICAL):
				SetPhysical(GetPhysical() + value);
				break;
			case (Stat.SPECIAL):
				SetSpecial(GetSpecial() + value);
				break;
			case (Stat.DEFENSE):
				SetDefense(GetDefense() + value);
				break;
			case (Stat.RESISTANCE):
				SetResistance(GetResistance() + value);
				break;
			case (Stat.SPEED):
				SetSpeed(GetSpeed() + value);
				break;
		}
	}

	// Getter / Setter ------------ //
	public int GetHp() {
		return _hp;
	}

	public void SetHp(int hp) {
		_hp = hp;
		if(_hp < 0) {
			_hp = 0;
		}
	}

	public int GetMana() {
		return _mana;
	}

	public void SetMana(int mana) {
		_mana = mana;
		if(_mana < 0) {
			_mana = 0;
		}
	}

	public int GetPhysical() {
		return _physical;
	}

	public void SetPhysical(int physical) {
		_physical = physical;
		if(_physical < 0) {
			_physical = 0;
		}
	}

	public int GetSpecial() {
		return _special;
	}

	public void SetSpecial(int special) {
		_special = special;
		if(_special < 0) {
			_special = 0;
		}
	}

	public int GetDefense() {
		return _defense;
	}

	public void SetDefense(int defense) {
		_defense = defense;
		if(_defense < 0) {
			_defense = 0;
		}
	}

	public int GetResistance() {
		return _resistance;
	}

	public void SetResistance(int resistance) {
		_resistance = resistance;
		if(_resistance < 0) {
			_resistance = 0;
		} 
	}

	public int GetSpeed() {
		return _speed;
	}

	public void SetSpeed(int speed) {
		_speed = speed;
		if(_speed < 0) {
			_speed = 0;
		}
	}

	// Resetter ------------------//
	public void ResetStats() {
		ResetHp();
		ResetMana();
		ResetPhysical();
		ResetSpecial();
		ResetDefense();
		ResetResistance();
		ResetSpeed();	
	}

	public void ResetHp() {
		_hp = maxHp;
	}

	public void ResetMana() {
		_mana = maxMana;
	}

	public void ResetPhysical() {
		_physical = maxPhysical;
	}

	public void ResetSpecial() {
		_special = maxSpecial;
	}

	public void ResetDefense() {
		_defense = maxDefense;
	}

	public void ResetResistance() {
		_resistance = maxResistance;
	}

	public void ResetSpeed() {
		_speed = maxSpeed;
	}
}