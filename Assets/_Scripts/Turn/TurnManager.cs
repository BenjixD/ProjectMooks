using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// The main controller/manager for a battle
public class TurnManager : MonoBehaviour {
	[SerializeField]
	public List<Phase> phases;
	public string callback;

	private int _curPhase;

	void Start() {
		AddListeners();
	}

    void OnDestroy() {
        RemoveListeners();
        foreach (var phase in this.phases) {
            phase.Dispose();
        }
    }

	public void Initialize(BattleField field, BattleUI ui, CommandSelector commandSelector, StageController stageController) {
		// Initialize Stages
		phases = new List<Phase>();
		_curPhase = 0;

		// Add the Phases in Order
		phases.Add(new TurnStartPhase(field, ui, callback));
		phases.Add(new PartySetupPhase(ui, field, callback));
		phases.Add(new MoveSelectionPhase(ui, field, commandSelector, callback));
		phases.Add(new BattlePhase(ui, field, callback));
		phases.Add(new TurnEndPhase(field, callback));

		GameManager.Instance.time.GetController().StartCoroutine(phases[_curPhase].RunPhase());
	}

	public Phase GetPhase() {
		return phases[_curPhase];
	}

	//
	// Private Methods
	//	
	private void AddListeners() {
		Messenger.AddListener(callback, this.StartNextPhase);
	}

	private void RemoveListeners() {
		Messenger.RemoveListener(callback, this.StartNextPhase);	
	}

	private void StartNextPhase() {
		_curPhase = (_curPhase + 1) % phases.Count;
		GameManager.Instance.time.GetController().StartCoroutine(phases[_curPhase].RunPhase());
	}
}