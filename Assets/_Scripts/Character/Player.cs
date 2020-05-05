using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : FightingEntity {
    public bool isHero = false;

    void Start() {
    }

	public void Initialize(PlayerCreationData data) {
		Name = data.name;
		SetStats(data.stats);
		SetJob(data.job);
	}



}