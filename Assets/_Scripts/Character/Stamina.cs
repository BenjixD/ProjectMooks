using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stamina {
	[SerializeField]
	private int _value;

	public int GetStamina() {
		return _value;
	}

	public void SetStamina(int stamina) {
		_value = stamina;
		if(_value < 0) {
			_value = 0;
		}
	}
}