using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RallyingCry", menuName = "Actions/Rallying Cry", order = 2)]
public class RallyingCry : ActionBase {
    [SerializeField] private GameObject _rallyingCryQTE;

    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        if (!base.TryChooseAction(user, splitCommand)) {
            return false;
        }

        List<int> targetIds = this.GetAllPossibleTargets(user).Map((FightingEntity target) => target.targetId );
        user.SetQueuedAction(new QueuedAction(user, this, targetIds));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        Instantiate(_rallyingCryQTE);

        // TODO: integration with battle controller
        FinishQTE(user, targets, 0);

        return new FightResult(user, this);
    }
    
    public FightResult FinishQTE(FightingEntity user, List<FightingEntity> targets, float power) {
        foreach (FightingEntity target in targets) {
            // TODO: apply rallying cry effect
        }
        return new FightResult(user, this);
    }
}
