using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterSlot : MonoBehaviour
{
    public Transform instantiationTransform;

    public bool IsXFlipped() {
        return instantiationTransform.transform.localScale.x < 0; 
    }

    public void InitializePosition(FightingEntity player) {
        player.transform.SetParent(this.instantiationTransform.transform, false);
        player.transform.localPosition = Vector3.zero;
        if (player.fighterMessageBox != null) {
            player.fighterMessageBox.Initialize(this.IsXFlipped());
        }
    }
}
