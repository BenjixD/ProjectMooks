using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public string playerName;
	public PlayerStats stats;
	public Job job;

	public void Initialize(PlayerCreationData data) {
		SetName(data.name);
		SetStats(data.stats);
		SetJob(data.job);
	}

	// Getters / Setters 
	public string GetJobName() {
		return job.ToString();
	}

	public void SetName(string name) {
		this.playerName = name;
	}

	public void SetStats(PlayerStats stats) {
		this.stats = stats;
	}

	public void SetJob(Job job) {
		this.job = job;
	}
}