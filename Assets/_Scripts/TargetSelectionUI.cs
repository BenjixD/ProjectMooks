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
        this.CommonInitialization(possibleTargets);
        this._commandSelector = selector;
        this._commandSelector.Initialize(0, possibleTargets.Count-1, this.UpdateTargetSelectionUI, true);

        this.UpdateTargetSelectionUI();
    }

    public void InitializeTargetSelectionAll(List<FightingEntity> possibleTargets) {
        this.CommonInitialization(possibleTargets);
        this._commandSelector = null;
        this.UpdateTargetSelectionUI();
    }

    public void ClearSelection() {
        if (potentialTargets != null) {
            for (int i = 0; i < potentialTargets.Count; i++) {
                this.SetMaterialOutline(i, false);
            }

            potentialTargets.Clear();
        }

        _commandSelector = null;
    }

    private void CommonInitialization(List<FightingEntity> possibleTargets) {
        this.ClearSelection();
        this.potentialTargets = possibleTargets;
    }


    private void UpdateTargetSelectionUI() {
        if (potentialTargets == null) {
            return;
        }

        for (int i = 0; i < potentialTargets.Count; i++) {
            this.SetMaterialOutline(i, false);
        }

        if (this._commandSelector != null) {
            // Single target
            int index = _commandSelector.GetChoice();
            FightingEntity target = potentialTargets[index];
            this.SetMaterialOutline(index, true);

        } else {
            // All targets
            for (int i = 0; i < potentialTargets.Count; i++) {
                this.SetMaterialOutline(i, true);
            }
        }
    }

    private void SetMaterialOutline(int targetIndex, bool enabled) {
        FightingEntity target = potentialTargets[targetIndex];
        SkeletonMaterialOutliner materialOutliner = target.GetComponent<SkeletonMaterialOutliner>();
        if (enabled) {
            materialOutliner.SetOutlineColor(this.targetSelectionColor);
        } else {
            materialOutliner.UnsetOutline();
        }

    }

}
