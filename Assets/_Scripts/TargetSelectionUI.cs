using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class TargetSelectionUI : MonoBehaviour
{

    public Color targetSelectionColor;

    private List<FightingEntity> potentialTargets;
    private CommandSelector _commandSelector {get; set;}



    
    public void InitializeTargetSelectionSingle(List<FightingEntity> possibleTargets, int initialChoice, CommandSelector selector) {
        this.potentialTargets = possibleTargets;
        this._commandSelector = selector;
        this._commandSelector.Initialize(0, possibleTargets.Count-1, this.UpdateTargetSelectionUI, true);
        this.UpdateTargetSelectionUI();
    }

    public void InitializeTargetSelectionAll(List<FightingEntity> possibleTargets) {
        this.potentialTargets = possibleTargets;
        this._commandSelector = null;
    }

    public void ClearSelection() {
        //foreach (var target in potentialTargets) {
        //    target.GetComponent<SpriteRenderer>().color = Color.white;
        //}

        potentialTargets.Clear();
        _commandSelector = null;
    }

    private void UpdateTargetSelectionUI() {
        // TODO: Fix broken target highlighting
        return;
        
        if (potentialTargets == null) {
            return;
        }

        foreach (var target in potentialTargets) {
            target.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (this._commandSelector != null) {
            // Single target
            FightingEntity target = potentialTargets[_commandSelector.GetChoice()];
            target.GetComponent<SpriteRenderer>().color = targetSelectionColor;

        } else {
            // All targets
            foreach (var target in potentialTargets) {
                target.GetComponent<SpriteRenderer>().color = targetSelectionColor;
            }
        }
    }




}
