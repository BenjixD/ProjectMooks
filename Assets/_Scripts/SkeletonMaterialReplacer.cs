
using UnityEngine;
using System.Collections.Generic;


public class SkeletonMaterialReplacer : MonoBehaviour {

   private Material[] materialsIWantToUse;
   private Material[] originalMaterials;
   private MeshRenderer meshRenderer;

    // Spine sets the material every frame, so we need to hack it to change material properties at runtime:
    // http://esotericsoftware.com/forum/Different-materials-for-same-Spine-character-5445

    public bool setOriginal = false;

   void Awake() {
       meshRenderer = GetComponent<MeshRenderer>();
       originalMaterials = meshRenderer.sharedMaterials;
   }

   void OnWillRenderObject ()
   {
        if (materialsIWantToUse == null || meshRenderer == null || setOriginal == true) {
            return;
        }

       // Use this for multi-material setups.
        var sharedMaterials = meshRenderer.sharedMaterials;

        if (materialsIWantToUse.Length != sharedMaterials.Length) {
            Debug.LogError("Incorrect amount of materials");
            return;
        }

        for (int i = 0; i < sharedMaterials.Length; i++) {
            sharedMaterials[i] = materialsIWantToUse[i];
        }
        meshRenderer.sharedMaterials = sharedMaterials;
   }

   public void SetMaterialOverride(Material[] materialsIWantToUse) {
       this.materialsIWantToUse = materialsIWantToUse;
   }

    public void UseMaterialOverride() {
        this.setOriginal = false;
    }

   public void UseOriginal() {
       this.setOriginal = true;
        var sharedMaterials = meshRenderer.sharedMaterials;
        for (int i = 0; i < sharedMaterials.Length; i++) {
            sharedMaterials[i] = originalMaterials[i];
        }
        
        meshRenderer.sharedMaterials = sharedMaterials;
   }
}
