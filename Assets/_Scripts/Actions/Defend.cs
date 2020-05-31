using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defend", menuName = "Actions/Defend", order = 5)]
public class Defend : ActionBase {
    [Header("Defend Values")]
    [SerializeField]
    private StatusAilment _buff;

    protected override bool QueueAction(FightingEntity user, string[] splitCommand) {
        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ user.targetId }));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        user.GetAilmentController().AddStatusAilment(Instantiate(_buff));
        return new FightResult(user, this);
    }
}
