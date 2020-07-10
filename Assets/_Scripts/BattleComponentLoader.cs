using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BattleUI))]
[RequireComponent(typeof(BattleField))]
[RequireComponent(typeof(TurnManager))]
[RequireComponent(typeof(CommandSelector))]
[RequireComponent(typeof(StageController))]
public class BattleComponentLoader : MonoBehaviour {
	public TurnManager turnManager {get;set;}
	public BattleField field {get;set;}
	public BattleUI ui {get;set;}
	public CommandSelector commandSelector {get;set;}
	public StageController stageController {get;set;}

	void Start() {
		GameManager.Instance.battleComponents = this;
		GameManager.Instance.time.GetController().StartCoroutine(InitializeComponentsCR());
	}

    // TODO: move this somewhere else?
    public void EvictMook(FightingEntity mookObject) {
        GameManager.Instance.gameState.playerParty.EvictPlayer(mookObject.targetId);
        Destroy(mookObject.gameObject);
        this.ui.statusBarsUI.UpdateStatusBars();
    }

	// 
	// Overwrittable for custom loading procedure
	//
	protected virtual IEnumerator InitializeComponentsCR() {
		turnManager = GetComponent<TurnManager>();
		field = GetComponent<BattleField>();
		ui = GetComponent<BattleUI>();
		stageController = GetComponent<StageController>();
		commandSelector = GetComponent<CommandSelector>();

		field.Initialize();
		ui.Initialize();
		turnManager.Initialize(field, ui, commandSelector, stageController);
		yield return null;
	}
}