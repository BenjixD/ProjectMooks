using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterSlot : MonoBehaviour
{
    public int slotIndex; // Should be equal to fighter.targetIndex

    public Transform instantiationTransform;

    public RectTransform targetParent;
    public ArrowUI targetPrefab;

    public FightingEntity fighter{get; set;}


    private List<ArrowUI> targetArrows = new List<ArrowUI>();


    public bool IsXFlipped() {
        return instantiationTransform.transform.localScale.x < 0; 
    }

    public void InitializePosition(FightingEntity player) {
        this.fighter = player;
        this.fighter.targetId = this.slotIndex;
        player.transform.SetParent(this.instantiationTransform.transform, false);
        player.transform.localPosition = Vector3.zero;
        if (player.fighterMessageBox != null) {
            player.fighterMessageBox.Initialize(this.IsXFlipped());
        }

        player.fighterSlot = this;

        // Set arrows position based on fighter's specification
        WorldspaceToScreenUI parentPositioner = targetParent.GetComponent<WorldspaceToScreenUI>();
        parentPositioner.SetWorldPoint(fighter.GetComponent<FighterPositions>().arrows);
    }


    public void AddTargetArrowUI(Color arrowColor) {
        ArrowUI newArrow = Instantiate<ArrowUI>(targetPrefab, targetParent);
        newArrow.SetColor(arrowColor);
        targetArrows.Add(newArrow);
    }

    public void ClearArrowsUIOfColor(Color arrowColor) {
        for (int i = this.targetArrows.Count-1; i >= 0; i--) {
            ArrowUI arrow = this.targetArrows[i];
            if (arrow.curArrowColor == arrowColor) {
                Destroy(arrow.gameObject);
                this.targetArrows.RemoveAt(i);
            }
        }
    }


    public void ClearAllArrowsUI() {
        foreach (var arrow in this.targetArrows) {
            Destroy(arrow.gameObject);
        }

        this.targetArrows.Clear();
    }

}
