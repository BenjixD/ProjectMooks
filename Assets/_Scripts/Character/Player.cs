using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public string playerName;
	public PlayerStats stats;
	public Job job;
	private Dictionary<ActionType, ActionBase> _actions = new Dictionary<ActionType, ActionBase>();
	private QueuedAction _queuedAction;

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
		_actions = GameManager.Instance.GetJobActionsList(job);
	}

	public void TryActionCommand(ActionType actionType, string[] splitCommand) {
		if (_actions.ContainsKey(actionType)) {
			if (!_actions[actionType].TryChooseAction(this, splitCommand)) {
				Debug.Log("Trying to use Action of type " + actionType + " failed");
			}
		} else {
			Debug.Log("Player does not have Action of type " + actionType);
		}
	}

	public void SetQueuedAction(QueuedAction queuedAction) {
		Debug.Log("Sucessfully set action " + queuedAction.GetAction() + " for player " + this.playerName);
		_queuedAction = queuedAction;
	}
}