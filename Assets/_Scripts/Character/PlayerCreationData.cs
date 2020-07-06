using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCreationData {
	public string name;
	public PlayerStats stats;
	public Job job;

	public PlayerCreationData(string n, Job job) {
		this.name = n;
		this.job = job;
        this.stats = (PlayerStats)GameManager.Instance.models.GetAllJobActionLists().Find( actionList => actionList.job == job ).prefab.stats.Clone();


        // TODO: This is a good automated test
        for (int i = 0; i < (int)Job.LENGTH; i++) {
            PlayerStats testStats = (PlayerStats)GameManager.Instance.models.GetAllJobActionLists().Find( actionList => actionList.job == (Job)i ).prefab.stats.Clone();
            if (testStats == null || testStats.maxHp.GetValue() <= 10) {
                Debug.LogError("ERROR: Job is not set correctly!  " + (Job)i);
            }
        }
	}

/*
	// TODO: Probably should use some kind of k-means clustering instead of defined logic
	private Job ChooseJobFromStats(PlayerStats template) {
		if(stats.maxHp.GetBaseValue() > template.maxHp.GetBaseValue() && stats.physical.GetBaseValue() > stats.special.GetBaseValue()) {
			return Job.WARRIOR;
		} else if(stats.maxHp.GetBaseValue() > template.maxHp.GetBaseValue() && stats.speed.GetBaseValue() > template.speed.GetBaseValue()) {
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
*/

}