using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType{
    PLAYER,
    ENEMY
}

public class QueuedAction {
    public FightingEntity user;
    public ActionBase _action;
    public List<int> _targetIds;
    public TargetType _targetIdType = 0;

    public QueuedAction(FightingEntity user, ActionBase action, List<int> targetIds, TargetType targetIdType) {
        this.user = user;
        _action = action;
        _targetIds = targetIds;
        _targetIdType = targetIdType;
    }

    public ActionBase GetAction() {
        return _action;
    }

    public List<int> GetTargetIds() {
        return _targetIds;
    }

    public void ExecuteAction() {
        List<FightingEntity> potentialTargets;
        if (_targetIdType == TargetType.PLAYER) {
            potentialTargets = new List<FightingEntity>(GameManager.Instance.party.GetPlayersInPosition());
        } else {
            Debug.Log("List: " + GameManager.Instance.battleController.enemies);
            potentialTargets = new List<FightingEntity>(GameManager.Instance.battleController.enemies);
        }

        List<FightingEntity> targets = new List<FightingEntity>();
        foreach (int target in _targetIds) {
            targets.Add(potentialTargets[target]);
        }

        _action.ExecuteAction(user, targets);
    }
}
