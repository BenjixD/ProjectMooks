using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BattleField))]
[RequireComponent(typeof(TurnManager))]
[RequireComponent(typeof(CommandSelector))]
[RequireComponent(typeof(StageController))]
public class BattleComponentLoader : MonoBehaviour {
	public TurnManager turnManager {get;set;}
	public BattleField field {get;set;}
	public BattleUI ui;
	public CommandSelector commandSelector {get;set;}
	public StageController stageController {get;set;}

	void Start() {
		GameManager.Instance.battleComponents = this;
		GameManager.Instance.time.GetController().StartCoroutine(InitializeComponentsCR());
	}

	// 
	// Overwrittable for custom loading procedure
	//
	protected virtual IEnumerator InitializeComponentsCR() {
		turnManager = GetComponent<TurnManager>();
		field = GetComponent<BattleField>();
		stageController = GetComponent<StageController>();
		commandSelector = GetComponent<CommandSelector>();

		field.Initialize();
		ui.Initialize();

		turnManager.Initialize(field, ui, commandSelector, stageController);
		yield return null;
	}
}