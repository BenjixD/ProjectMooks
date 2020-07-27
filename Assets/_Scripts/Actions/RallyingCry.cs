using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RallyingCry", menuName = "Actions/Quick Time Events/Rallying Cry", order = 3)]
public class RallyingCry : ActionBase {
    [SerializeField] private GameObject _rallyingCryQTE = null;

    public override bool QueueAction(FightingEntity user, string[] splitCommand) {
        List<int> targetIds = this.GetAllPossibleActiveTargets(user).Map((FightingEntity target) => target.targetId );
        user.SetQueuedAction(this, targetIds);
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        RallyingCryQTE qte = Instantiate(_rallyingCryQTE).GetComponent<RallyingCryQTE>();
        qte.Initialize(user, targets, this);
        return new FightResult(user, this);
    }
    
    public void FinishQTE(FightingEntity user, List<FightingEntity> targets, float power) {
        PlayerStats before, after;
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        
        foreach (FightingEntity target in targets) {
            before = (PlayerStats)target.stats.Clone();
            // TODO: increase strength of buff based on power
            List<StatusAilment> inflicted = InflictStatuses(target);
            after = (PlayerStats)target.stats.Clone();
            receivers.Add(new DamageReceiver(target, before, after, inflicted));
        }
        
        FightResult result = new FightResult(user, this, receivers);
        _battleFight.EndFight(result, this);
    }

    protected override void PlaySound(FightingEntity user) {
        SoundController sc = user.GetSoundController();
        sc.PlayClip(sc.GetRandomClipFromKey("rallying_cry"));
    }
}
