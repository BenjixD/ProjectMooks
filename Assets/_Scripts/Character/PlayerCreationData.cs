using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCreationData {
	public string name;
	public PlayerStats stats;
	public Job job;

	public PlayerCreationData(string n, PlayerStats template) {
		this.name = n;
		this.stats = new PlayerStats(template);
		this.stats.RandomizeStats();
		this.stats.ResetStats();
		this.job = ChooseJobFromStats(template);
	}

	public PlayerCreationData(string n, PlayerStats stats, Job job) {
		this.name = n;
		this.stats = stats;
		this.stats.ResetStats();
		this.job = job;
	}

	// TODO: Probably should use some kind of k-means clustering instead of defined logic
	private Job ChooseJobFromStats(PlayerStats template) {
		if(stats.maxHp > template.maxHp && stats.maxPhysical > stats.maxSpecial) {
			return Job.WARRIOR;
		} else if(stats.maxMana > template.maxMana && stats.maxSpecial > stats.maxPhysical) {
			return Job.MAGE;
		} else if(stats.maxDefense > template.maxDefense && stats.maxResistance > template.maxResistance) {
			return Job.CLERIC;
		} else {
            List<Job> jobsList = GameManager.Instance.getMookJobs();

			return jobsList[UnityEngine.Random.Range(0, jobsList.Count)];
		}
	}
}