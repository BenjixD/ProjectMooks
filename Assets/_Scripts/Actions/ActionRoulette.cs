using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Selects a random move from the list and uses it.
 */
[CreateAssetMenu(fileName = "NewActionRoulette", menuName = "Actions/Action Roulette", order = 5)]
public class ActionRoulette : ActionBase {

    [SerializeField, Tooltip("List of potential actions that can be used.")]
    private ActionBase[] _actions = null;
    
    public override IEnumerator ExecuteAction(FightingEntity user, List<FightingEntity> targets, BattleFight battleFight) {
        _battleFight = battleFight;
        FightResult result = new FightResult(user, this);
        if (CheckCost(user)) {
            PayCost(user);
            ActionBase selectedAction = _actions[Random.Range(0, _actions.Length)];
            targets = selectedAction.GetNewTargets(user);

            // Play animation
            yield return GameManager.Instance.time.GetController().StartCoroutine(selectedAction.PlayAnimation(user, targets));

            result = selectedAction.ApplyEffect(user, targets);
            OnPostEffect(result);
            _battleFight.EndFight(result, this);
            yield return GameManager.Instance.time.GetController().WaitForSeconds(selectedAction.animation.GetAnimCooldown());
        }
    }
}
