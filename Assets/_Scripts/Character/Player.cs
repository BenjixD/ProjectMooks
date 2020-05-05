using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public string playerName;
	public PlayerStats stats;
	public Job job;

	// Ongoing status ailments
	private Dictionary<string, StatusAilment> _ailments = new Dictionary<string, StatusAilment>();

	public void Initialize(PlayerCreationData data) {
		SetName(data.name);
		SetStats(data.stats);
		SetJob(data.job);
	}

	public void AddStatusAilment(StatusAilment ailment) {
		if(_ailments.ContainsKey(ailment.name)) {
			_ailments[ailment.name].StackWith(this, ailment);
		} else {
			_ailments.Add(ailment.name, ailment);
			ailment.ApplyTo(this);
		}
	}

	public void RemoveStatusAilment(string ailment) {
		StatusAilment status = _ailments[ailment];
		_ailments.Remove(ailment);
		status.Recover(this);
	}

	public void DecrementAilmentsDuration() {
		List<string> timeout = new List<string>();
		foreach(KeyValuePair<string, StatusAilment> entry in _ailments) {
			if(entry.Value.duration <= 0) {
				timeout.Add(entry.Key);
			} else {
				entry.Value.DecrementDuration();
			}
		}

		foreach(string s in timeout) {
			RemoveStatusAilment(s);
		}
	}

	public void TickAilmentEffects() {
		foreach(KeyValuePair<string, StatusAilment> entry in _ailments) {
			entry.Value.TickEffect(this);
		}
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