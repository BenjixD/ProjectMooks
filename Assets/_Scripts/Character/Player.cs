using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : FightingEntity {
	public string playerName;
	public Job job;

    public Canvas speechCanvas; 
    public Text speechCanvasText;

	public string GetJobName() {
		return job.ToString();
	}
}