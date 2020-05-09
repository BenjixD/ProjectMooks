using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAI : Player {
	void Start() {
		_ai = new FightingEntityAI(this, 100f, 0.5f);
		StartCoroutine(Attack());
	}

	IEnumerator Attack() {
		while(true) {
			ActionBase action = _ai.GetSuggestion();
			Debug.Log("Suggested Move: " + action.name);
			float result = Random.Range(-1.0f, 1.0f);
			if(result < -0.5f) {
				Debug.Log("That move wasn't very effective...");
			} else if(result > -0.5f && result < 0.5f){
				Debug.Log("That move worked out.");
			} else {
				Debug.Log("That move was super effective!");
			}
			//_ai.FeedbackOnBestAction(result);
			_ai.PerturbScore();
			yield return new WaitForSeconds(3f);
		}
	}
}