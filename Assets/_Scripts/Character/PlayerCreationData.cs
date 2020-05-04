using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreationData {
	public string name;
	public PlayerStats stats;

	public PlayerCreationData(string n, PlayerStats template) {
		name = n;
		stats = new PlayerStats(template);
		stats.RandomizeStats();
		stats.ResetStats();
	}
}