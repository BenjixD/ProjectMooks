
using UnityEngine;
using System.Collections.Generic;

public class SkeletonMaterialOutliner : MonoBehaviour {

    public MeshRenderer meshRenderer;

    private Dictionary<Color, Material[]> outlinedMaterials = new Dictionary<Color, Material[]>();

    public float targetSelectionOutlineWidth = 3.5f;

    public SkeletonMaterialReplacer materialReplacer;

    public void SetOutlineColor(Color color) {
        if (color == null) {
            this.UnsetOutline();
            return;
        }

        if (!outlinedMaterials.ContainsKey(color)) {
            this.InitializeOutlineColor(color);
        }

        materialReplacer.UseMaterialOverride();
        materialReplacer.SetMaterialOverride(outlinedMaterials[color]);
    }

    public void UnsetOutline() {
        materialReplacer.UseOriginal();
    }
    
    private void InitializeOutlineColor(Color color) {
        Material[] materials = meshRenderer.materials;
        foreach (var material in materials) {
            material.SetFloat("_OutlineWidth", targetSelectionOutlineWidth);
            material.SetColor("_OutlineColor", color);
        }
        outlinedMaterials.Add(color, materials);
    }

}
