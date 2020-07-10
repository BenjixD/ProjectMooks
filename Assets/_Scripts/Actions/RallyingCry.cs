using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RallyingCry", menuName = "Actions/Quick Time Events/Rallying Cry", order = 3)]
public class RallyingCry : ActionBase {
    [SerializeField] private GameObject _rallyingCryQTE = null;

    public override bool QueueAction(FightingEntity user, string[] splitCommand) {
        List<int> targetIds = this.GetAllPossibleActiveTargets(user).Map((FightingEntity target) => target.targetId );
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
