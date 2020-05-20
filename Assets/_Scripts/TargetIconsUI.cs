
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TargetIconsUI : MonoBehaviour {
    
    public List<RectTransform> targetParents;
    public ArrowUI targetPrefab;

    private List<ArrowUI> targetArrows = new List<ArrowUI>();

    public void Initialize() {
        Messenger.AddListener<QueuedAction>(Messages.OnSetQueuedAction, this.onSetQueuedAction);
    }

    void OnDestroy() {
        Messenger.RemoveListener<QueuedAction>(Messages.OnSetQueuedAction, this.onSetQueuedAction);
    }

    public void UpdateIcons() {
        if (GameManager.Instance.turnController.battlePhase != BattlePhase.MOVE_SELECTION) {
            return;
        } 

        Player[] players = GameManager.Instance.turnController.field.playerParty.members;
        for (int i = 0; i < players.Length; i++) {
            if (players[i] != null && players[i].GetQueuedAction() != null) {
                foreach (int target in players[i].GetQueuedAction()._targetIds) {
                    ArrowUI newArrow = Instantiate<ArrowUI>(targetPrefab, targetParents[target]);
                    newArrow.SetColor(GameManager.Instance.party.IndexToColor(i));
                    targetArrows.Add(newArrow);
                }
            }
        }
    }

    public void ClearTargetArrows() {
        foreach (var arrow in targetArrows) {
            Destroy(arrow.gameObject);
        }

        targetArrows.Clear();
    }

    private void onSetQueuedAction(QueuedAction action) {
        this.ClearTargetArrows();
        this.UpdateIcons();
    }

}