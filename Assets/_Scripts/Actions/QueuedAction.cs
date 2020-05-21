using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QueuedAction {
    public FightingEntity user;
    public ActionBase _action;
    public List<int> _targetIds;
    public TargetType _targetIdType = 0;

    public QueuedAction(FightingEntity user, ActionBase action, List<int> targetIds) {
        this.user = user;
        _action = action;
        _targetIds = targetIds;
    }

    public ActionBase GetAction() {
        return _action;
    }

    public List<int> GetTargetIds() {
        return _targetIds;
    }

    public List<FightingEntity> GetTargets() {
        return _action.GetTargets(this.user, _targetIds);
    }
    
    public FightResult ExecuteAction() {
        return _action.ExecuteAction(user, GetTargets());
    }
}
