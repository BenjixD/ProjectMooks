using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
	public string Name;

	// Max Stats
	public int MaxHp;
	public int MaxMana;
	public int MaxPhysical;
	public int MaxSpecial;
	public int MaxDefense;
	public int MaxResistance;
	public int MaxSpeed;
	public bool IsRare;

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

	public Player(string name) {
		Name = name;
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
	public void resetHp() {
		_hp = MaxHp;
	}

	public void resetMana() {
		_mana = MaxMana;
	}

	public void resetPhysical() {
		_physical = MaxPhysical;
	}

	public void resetSpecial() {
		_special = MaxSpecial;
	}

	public void resetDefense() {
		_defense = MaxDefense;
	}

	public void resetResistance() {
		_resistance = MaxResistance;
	}

	public void resetSpeed() {
		_speed = MaxSpeed;
	}
}