using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlideType {
    STEP_FORWARD,
    MELEE,
    NONE
}

[CreateAssetMenu(fileName = "NewActionAnimation", menuName = "ActionsAnimations/DefaultActionAnimation", order = 0)]
public class ActionAnimation : ScriptableObject {
    [Tooltip("The name of the default animation played for the user of this action. Alternatively, you can override Animate().")]
    public string userAnimName;
    [Tooltip("The prefab that will be instantiated atop the target(s).")]
    public GameObject targetHitEffect;
    [Tooltip("The type of slide that should precede this animation: none, a small slide forward, or a long slide into melee range of the target(s).")]
    public SlideType slideType;
    [Tooltip("Time it takes for the user to slide to the target(s).")]
    public float slideDuration = 0.5f;
    [Tooltip("The distance to stop away from the target(s) after a slide and before attacking.")]
    public float meleeReach = 10f;
    [SerializeField, Tooltip("Time into the user animation before the effect takes place.")]
    private float _timeBeforeEffect;
    [SerializeField, Tooltip("Time to wait after the action effect takes place.")]
    private float _timeAfterEffect;

    public IEnumerator Animate(FightingEntity user, List<FightingEntity> targets) {
        // Determine the start and end positions for the melee slide
        Vector3 userStartingPos = user.transform.position;
        Vector3 userDestination = Vector3.zero;
        
        // Perform slide
        if (slideType != SlideType.NONE) {
            SetSlidePositions(user, targets, ref userDestination);
            float elapsedTime = 0;
            float t = 0;
            while (user.transform.position != userDestination) {
                elapsedTime += GameManager.Instance.time.deltaTime;
                t = Mathf.SmoothStep(0, 1, elapsedTime / slideDuration);
                user.transform.position = Vector3.Lerp(userStartingPos, userDestination, t);
                yield return null;
            }
        }

        // Perform action animation
        AnimateUser(user);
        yield return GameManager.Instance.time.GetController().WaitForSeconds(_timeBeforeEffect);

        // Perform hit effect animations on target(s)
        if (targetHitEffect != null) {
            foreach (FightingEntity target in targets) {
                FighterPositions positions = target.GetComponent<FighterPositions>();
                if (positions != null) {
                    Instantiate(targetHitEffect, positions.damagePopup);
                }
                // TODOL: make targets flash white
            }
        }
        yield return GameManager.Instance.time.GetController().WaitForSeconds(_timeAfterEffect);

        // Slide back to starting position if necessary
        if (slideType != SlideType.NONE) {
            float elapsedTime = 0;
            float t = 0;
            while (user.transform.position != userStartingPos) {
                elapsedTime += GameManager.Instance.time.deltaTime;
                t = Mathf.SmoothStep(0, 1, elapsedTime / slideDuration);
                user.transform.position = Vector3.Lerp(userDestination, userStartingPos, t);
                yield return null;
            }
        }
    }

    private void SetSlidePositions(FightingEntity user, List<FightingEntity> targets, ref Vector3 userDestination) {
        if (slideType == SlideType.STEP_FORWARD) {
            // TODOL
            userDestination = user.transform.position;
            userDestination += user.IsHeroTeam() ? new Vector3(-5f, 0, 0) : -new Vector3(-5f, 0, 0);
        } else if (slideType == SlideType.MELEE) {
            float averageTargetRadius = 0;
            foreach (FightingEntity target in targets) {
                userDestination += target.transform.position;
                averageTargetRadius += target.GetComponent<FighterPositions>().fighterRadius;
            }
            userDestination /= targets.Count;
            averageTargetRadius /= targets.Count;
            // Distance target and destination based on the reach of this attack and the width of the target
            float distance = meleeReach + averageTargetRadius;
            userDestination.x += user.IsHeroTeam() ? distance : -distance;
        } else {
            Debug.Log("No slide position specified for SlideType " + slideType);
        }
    }

    protected virtual void AnimateUser(FightingEntity user) {
        AnimationController controller = user.GetAnimController();
        int track = controller.TakeFreeTrack();
        if (track != -1) {
            controller.AddToTrack(track, userAnimName, false, 0);
            controller.EndTrackAnims(track);
        }
    }

    public float GetAnimWindup() {
        float time = _timeBeforeEffect;
        if (slideType != SlideType.NONE) {
            time += slideDuration;
        }
        return time;
    }

    public float GetAnimCooldown() {
        float time = _timeAfterEffect;
        if (slideType != SlideType.NONE) {
            time += slideDuration;
        }
        return time;
    }
}
