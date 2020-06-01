using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

[System.Serializable]
public class AilmentController {
	// Ongoing status ailments
	private Dictionary<string, StatusAilment> _ailments = new Dictionary<string, StatusAilment>();
	private FightingEntity _entity;

	public AilmentController(FightingEntity fe) {
		_entity = fe;
	}

	public StatusAilment GetAilment(string name) {
		if(_ailments.ContainsKey(name)) {
			return _ailments[name];	
		} else {
			return null;
		}
	}

	public ReadOnlyDictionary<string, StatusAilment> GetAllAilments() {
		return new ReadOnlyDictionary<string, StatusAilment>(_ailments);
	}

	public ReadOnlyDictionary<string, StatusAilment> GetAilmentsOfPhase(TurnPhase bp) {
		return new ReadOnlyDictionary<string, StatusAilment>(
			_ailments.Where(kvp => kvp.Value.phase == bp).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
	}

	public void AddStatusAilment(StatusAilment ailment) {
		if(_ailments.ContainsKey(ailment.name)) {
			_ailments[ailment.name].StackWith(_entity, ailment);
		} else {
			_ailments.Add(ailment.name, ailment);
			ailment.ApplyTo(_entity);
		}
	}

	public void RemoveStatusAilment(string ailment) {
		StatusAilment status = _ailments[ailment];
		_ailments.Remove(ailment);
		status.Recover(_entity);
	}

	public void RemoveAllStatusAilments() {
		_ailments.Clear();
	}

	public void DecrementAllAilmentsDuration() {
		List<string> timeout = new List<string>();
		foreach(KeyValuePair<string, StatusAilment> entry in _ailments.ToList()) {
			entry.Value.DecrementDuration();
			if(entry.Value.duration <= 0) {
				timeout.Add(entry.Key);
			}
		}

		foreach(string s in timeout) {
			RemoveStatusAilment(s);
		}
	}

	public void DecrementAilmentDuration(StatusAilment status) {
		if(_ailments.ContainsKey(status.name)) {
			StatusAilment s = _ailments[status.name];
			s.DecrementDuration();
			if(s.duration <= 0) {
				RemoveStatusAilment(s.name);
			}	
		}
	}

	public void TickAilmentEffect(StatusAilment status) {
		if(_ailments.ContainsKey(status.name)) {
			StatusAilment s = _ailments[status.name];
			s.TickEffect(_entity);
		}
	}

	public void TickAilmentEffects(TurnPhase bp) {
		foreach(KeyValuePair<string, StatusAilment> entry in _ailments) {
			if(entry.Value.phase == bp) {
				entry.Value.TickEffect(_entity);	
			}
		}
	}

	public void TickAllAilmentEffects() {
		foreach(KeyValuePair<string, StatusAilment> entry in _ailments.ToList()) {
			entry.Value.TickEffect(_entity);
		}
	}
}