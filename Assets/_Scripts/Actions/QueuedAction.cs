using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QueuedAction : ICloneable {
    public FightingEntity user;
    public ActionBase _action;
    public List<int> _targetIds;
    public TargetType _targetIdType = 0;
    public bool isAutoQueued = false;

    private bool isSet = false;

    public QueuedAction(FightingEntity user) {
        this.user = user;
        this._targetIds = new List<int>();
        this.Reset();
    }

    public QueuedAction(FightingEntity user, ActionBase action, List<int> targetIds) {
        this.user = user;
        this._targetIds = new List<int>();
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

    public bool CanQueueAction(ActionBase action) {
        return action != null && _action.CheckCost(this.user) && (GetTargets().Count > 0 || _action.targetInfo.targetType == TargetType.NONE);
    }

    public void SetAction(ActionBase action, List<int> targetIds, bool autoQueued = false) {
        _action = action;
        _targetIds = new List<int>(targetIds);
        this.isAutoQueued = autoQueued;
        this.isSet = true;
    }

    public bool GetIsSet() {
        return isSet;
    }

    public void Reset() {
        _targetIds.Clear();
        this._action = null;
        this.isSet = false;
        this.isAutoQueued = false;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
