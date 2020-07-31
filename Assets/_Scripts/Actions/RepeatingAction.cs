using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * An action that repeats its effect multiple times.
 */
[CreateAssetMenu(fileName = "NewRepeatingAction", menuName = "Actions/Repeating Action", order = 5)]
public class RepeatingAction : ActionBase {
    [SerializeField, Tooltip("Number of times to repeat the action.")]
    private int _numHits = 0;

    public override IEnumerator ExecuteAction(FightingEntity user, List<FightingEntity> targets, BattleFight battleFight) {
        _battleFight = battleFight;
        FightResult result = new FightResult(user, this);
        if (CheckCost(user)) {
            PayCost(user);
            targets = GetAllPossibleActiveTargets(user);

            Vector3 userStartingPos = user.transform.position;

            // Slide in
            if (animation != null && animation.slideType != SlideType.NONE) {
                yield return GameManager.Instance.time.GetController().StartCoroutine(animation.SlideIn(user, targets));
            }

            List<FightingEntity> potentialTargets = targets;
            for (int i = 0; i < _numHits; i++) {
                potentialTargets = targets.Filter(target => target != null);
                if (potentialTargets.Count == 0) {
                    break;
                }
                List<FightingEntity> currTargets = SetTargets(user, targets);
                // Play animation
                GameManager.Instance.time.GetController().StartCoroutine(animation.AnimateAction(user, currTargets));
                yield return GameManager.Instance.time.GetController().WaitForSeconds(animation.GetAnimWindup(false));
                result = ApplyEffect(user, currTargets);
                OnPostEffect(result);
                _battleFight.EndFight(result, this);
                yield return GameManager.Instance.time.GetController().WaitForSeconds(animation.GetAnimCooldown(false));
            }

            // Slide back
            if (animation != null && animation.slideType != SlideType.NONE) {
                yield return GameManager.Instance.time.GetController().StartCoroutine(animation.Slide(user.transform, user.transform.position, userStartingPos));
            }
        }
    }
}
