
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BattleField))]
public class FighterSlotUI : MonoBehaviour {
    
    public List<RectTransform> targetParents;
    public ArrowUI targetPrefab;

    private List<ArrowUI> targetArrows = new List<ArrowUI>();
    private BattleField _field;


    // Mapping from each fighter slot to its current targets
    private Dictionary<FighterSlot, List<FighterSlot>> fighterToActionMap = new Dictionary<FighterSlot, List<FighterSlot>>();

    public void Initialize() {
        this._field = GetComponent<BattleField>();
        Messenger.AddListener<QueuedAction>(Messages.OnSetQueuedAction, this.onSetQueuedAction);
        this.ClearTargetArrows();
    }

    void OnDestroy() {
        Messenger.RemoveListener<QueuedAction>(Messages.OnSetQueuedAction, this.onSetQueuedAction);
    }


    public void ClearTargetArrows() {
        List<FighterSlot> slots = this._field.GetFighterSlots();
        foreach (var slot in slots) {
            slot.ClearAllArrowsUI();
        }

        foreach (var slot in slots) {
            fighterToActionMap[slot] = null;
        }
    }

    public void ClearTargetsForFighter(FightingEntity fighter) {
        this.ClearTargetsForSlot(fighter.fighterSlot);
    }

    private void ClearTargetsForSlot(FighterSlot slot) {
        List<FighterSlot> targets = this.fighterToActionMap[slot];
        foreach (FighterSlot targetSlot in targets) {
            if (targetSlot.fighter != null) {
                targetSlot.ClearArrowsUIOfColor(slot.fighter.GetOrderColor());
            }
        }
    }

    private void onSetQueuedAction(QueuedAction action) {
        FighterSlot slot = action.user.fighterSlot;
        if (!this.fighterToActionMap.ContainsKey(slot)) {
            Debug.LogError("ERROR: Slot not found!");
            return;
        }


        if (action.user.isEnemy()) {
            // Mockups don't show enemy targetting
            return;
        }


        if (this.fighterToActionMap[slot] != null) {
            this.ClearTargetsForSlot(slot);
        } else {
            Debug.Log("Action targets count: " + action.GetTargets().Count);
            this.fighterToActionMap[slot] = action.GetTargets().Map((FightingEntity fighter) => fighter.fighterSlot);
        }

        this.SetSlotUIForAction(action);
    }

    private void SetSlotUIForAction(QueuedAction action) {
        FightingEntity user = action.user;

        List<FightingEntity> targets = action.GetTargets();
        foreach (FightingEntity target in targets) {
            target.fighterSlot.AddTargetArrowUI(user.GetOrderColor());
        }

    }
}