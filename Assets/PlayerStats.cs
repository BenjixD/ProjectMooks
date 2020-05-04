using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats {
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
	public void ResetHp() {
		_hp = MaxHp;
	}

	public void ResetMana() {
		_mana = MaxMana;
	}

	public void ResetPhysical() {
		_physical = MaxPhysical;
	}

	public void ResetSpecial() {
		_special = MaxSpecial;
	}

	public void ResetDefense() {
		_defense = MaxDefense;
	}

	public void ResetResistance() {
		_resistance = MaxResistance;
	}

	public void ResetSpeed() {
		_speed = MaxSpeed;
	}
}