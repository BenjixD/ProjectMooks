using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class FightingEntityAI {
	public class ActionWeight : IComparer<Tuple<float, ActionBase>> {
		public int Compare(Tuple<float, ActionBase> a, Tuple<float, ActionBase> b) {
			if(a.Item1 < b.Item1) {
				return -1;
			} else if(a.Item1 == b.Item1) {
				return 0;
			} else {
				return 1;
			}
		}

	}

	private FightingEntity _char;
	private SortedSet<Tuple<float, ActionBase>> _movesetScore;

	[SerializeField]
	private float _precision = 100f; // Higher precision, less variance
	[SerializeField]
	private float _weight = 0.5f; // Higher Weight, larger punishment for poor feedback [0, 1]
	private float _avgEffectiveness = -1f; // [-1 , 1], try some moves before effectiveness goes up

	public FightingEntityAI(FightingEntity fe, float precision, float weight) {
		this._char = fe;
		_precision = precision;
		_weight = weight;
		InitializeMovesetScore(this._char.actions, _precision);
	}

	public FightingEntityAI(FightingEntity fe) {
		this._char = fe;
		InitializeMovesetScore(this._char.actions, _precision);
	}

	public ActionBase GetSuggestion() {
		return GetBestAction().Item2;
	}

	public ActionBase GetRandomAction() {
		return _char.actions[UnityEngine.Random.Range(0, _char.actions.Count)];
	}

	public void PerturbScore() {
		SortedSet<Tuple<float, ActionBase>> newScore = new SortedSet<Tuple<float, ActionBase>>(new ActionWeight());
		foreach(Tuple<float, ActionBase> action in _movesetScore) {
			float score = action.Item1 * (BoxMuller.GetRandom()/2 + 1);
			// Rubberband
			score -= (score - _precision) * _weight;
			newScore.Add(new Tuple<float, ActionBase>(score, action.Item2));
		}
		_movesetScore = newScore;
	}

	public void ReviewFightResult(List<FightResult> fights) {
		foreach(FightResult fight in fights) {
			ActionBase action = fight.action;
			float effectiveness = 0;
			foreach(DamageReceiver dr in fight.receivers) {
				effectiveness += StatDeltaEffectiveness(dr.before, dr.after) / fight.receivers.Count;
			}
			effectiveness = Mathf.Clamp((effectiveness - _avgEffectiveness) / 2, -1f, 1f);
			_avgEffectiveness = (_avgEffectiveness + effectiveness) / 2;

			FeedbackOnAction(effectiveness, action);
		}
	}

	private void InitializeMovesetScore(List<ActionBase> actions, float precision) {
		_movesetScore = new SortedSet<Tuple<float, ActionBase>>(new ActionWeight());
		foreach(ActionBase action in actions) {
			_movesetScore.Add(new Tuple<float, ActionBase>(precision * (BoxMuller.GetRandom()/2 + 1), action));
		}
	}

	private Tuple<float, ActionBase> GetBestAction() {
		return _movesetScore.Max;
	}

	private float StatDeltaEffectiveness(PlayerStats before, PlayerStats after) {
		// TODO: Generalize stat diff scores
		float delta = 0f;
		delta += (after.GetHp() - before.GetHp()) / (float)(before.GetHp() <= 0 ? 1 : before.GetHp());
		delta += (after.GetMana() - before.GetMana()) / (float)(before.GetMana() <= 0 ? 1 : before.GetMana());
		delta += (after.GetPhysical() - before.GetPhysical()) / (float)(before.GetPhysical() <= 0 ? 1 : before.GetPhysical());
		delta += (after.GetSpecial() - before.GetSpecial()) / (float)(before.GetSpecial() <= 0 ? 1 : before.GetSpecial());
		delta += (after.GetDefense() - before.GetDefense()) / (float)(before.GetDefense() <= 0 ? 1 : before.GetDefense());
		delta += (after.GetResistance() - before.GetResistance()) / (float)(before.GetResistance() <= 0 ? 1 : before.GetResistance());
		delta += (after.GetSpeed() - before.GetSpeed()) / (float)(before.GetSpeed() <= 0 ? 1 : before.GetSpeed());
		delta *= -1f / 7f;
		return delta;
	}

	private void FeedbackOnBestAction(float feedback) {
		// Update Score based on feedback
		Tuple<float, ActionBase> bestAction = _movesetScore.Max;
		_movesetScore.Remove(bestAction);
		float score = bestAction.Item1 + feedback * _weight * _precision;
		if(score < 0) {
			score = 0;
		}
		else if(score > 2 * _precision) {
			score = _precision * 2;
		}

		_movesetScore.Add(new Tuple<float, ActionBase>(score, bestAction.Item2));
	}

	private void FeedbackOnAction(float feedback, ActionBase action) {
		Tuple<float, ActionBase> chosenAction = null;
		foreach(Tuple<float, ActionBase> tuple in _movesetScore) {
			if(tuple.Item2 == action) {
				chosenAction = tuple;
				break;
			}
		}

		if(chosenAction != null) {
			_movesetScore.Remove(chosenAction);
			float score = chosenAction.Item1 + feedback * _weight * _precision;
			if(score < 0) {
				score = 0;
			}
			else if(score > 2 * _precision) {
				score = _precision * 2;
			}
			_movesetScore.Add(new Tuple<float, ActionBase>(score, action));
		}
	}

	private void LogMoveScores() {
		foreach(Tuple<float, ActionBase> action in _movesetScore) {
			Debug.Log(action.Item2.name + " : " + action.Item1);
		}
	}
}
