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
		this.stats = (PlayerStats)template.Clone();
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
		if(stats.maxHp.GetBaseValue() > template.maxHp.GetBaseValue() && stats.physical.GetBaseValue() > stats.special.GetBaseValue()) {
			return Job.WARRIOR;
		} else if(stats.maxHp.GetBaseValue() > template.maxHp.GetBaseValue() && stats.maxSpeed.GetBaseValue() > template.maxSpeed.GetBaseValue()) {
			return Job.LANCER;
		} else if(stats.maxMana.GetBaseValue() > template.maxMana.GetBaseValue() && stats.special.GetBaseValue() > stats.physical.GetBaseValue()) {
			return Job.MAGE;
		} else if(stats.defence.GetBaseValue() > template.defence.GetBaseValue() && stats.resistance.GetBaseValue() > template.resistance.GetBaseValue()) {
			return Job.CLERIC;
		} else {
            List<Job> jobsList = GameManager.Instance.models.getMookJobs();

			return jobsList[UnityEngine.Random.Range(0, jobsList.Count)];
		}
	}
}