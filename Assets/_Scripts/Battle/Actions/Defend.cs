using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defend", menuName = "Actions/Defend", order = 2)]
public class Defend : ActionBase {
    [Header("Defend Values")]
    [SerializeField]
    private DefenseStatusAilment _buff;

    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        if (!base.TryChooseAction(user, splitCommand)) {
            return false;
        }

        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ user.targetId }));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        user.GetAilmentController().AddStatusAilment(Instantiate(_buff));
        return new FightResult(user, this);
    }
}
