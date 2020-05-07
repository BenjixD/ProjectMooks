using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class TargetSelectionUI : MonoBehaviour
{

    public Color targetSelectionColor;
    public float targetSelectionOutlineWidth = 3.5f;


    private List<FightingEntity> potentialTargets;
    private CommandSelector _commandSelector {get; set;}

    List<Material[]> outlinedMaterials;

    
    public void InitializeTargetSelectionSingle(List<FightingEntity> possibleTargets, int initialChoice, CommandSelector selector) {
        this.potentialTargets = possibleTargets;
        this._commandSelector = selector;
        this._commandSelector.Initialize(0, possibleTargets.Count-1, this.UpdateTargetSelectionUI, true);

        this.CreateOutlinedMaterials(possibleTargets);

        this.UpdateTargetSelectionUI();
    }

    public void InitializeTargetSelectionAll(List<FightingEntity> possibleTargets) {
        this.potentialTargets = possibleTargets;
        this._commandSelector = null;
    }

    public void ClearSelection() {
        for (int i = 0; i < potentialTargets.Count; i++) {
            this.SetMaterialOutline(i, false);
        }

        potentialTargets.Clear();
        _commandSelector = null;
    }

    private void CreateOutlinedMaterials(List<FightingEntity> possibleTargets) {
        outlinedMaterials = new List<Material[]>();
        foreach (var target in possibleTargets) {
            Material[] materials = target.GetComponent<MeshRenderer>().materials;
            foreach (var material in materials) {
                material.SetFloat("_OutlineWidth", targetSelectionOutlineWidth);
                material.SetColor("_OutlineColor", targetSelectionColor);
            }
            outlinedMaterials.Add(materials);
        }
    }

    private void UpdateTargetSelectionUI() {
        // TOOD: FIX sprite highlighting


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
        SkeletonMaterialReplacer materialReplacer = target.GetComponent<SkeletonMaterialReplacer>();
        if (enabled) {
            materialReplacer.UseMaterialOverride();
            materialReplacer.SetMaterialOverride(outlinedMaterials[targetIndex]);
        } else {
            materialReplacer.UseOriginal();
        }

    }

}
