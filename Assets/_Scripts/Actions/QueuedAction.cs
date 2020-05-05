using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueuedAction {
    private ActionBase _action;
    private List<int> _targetIds;

    public QueuedAction(ActionBase action, List<int> targetIds) {
        _action = action;
        _targetIds = targetIds;
    }

    public ActionBase GetAction() {
        return _action;
    }

    public List<int> GetTargetIds() {
        return _targetIds;
    }
}
