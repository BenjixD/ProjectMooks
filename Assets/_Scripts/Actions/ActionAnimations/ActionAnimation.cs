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
    [Tooltip("Set to true to make the target(s) flash when the effect happens.")]
    public bool targetFlash = true;
    [Tooltip("Set to true to add a slight delay between the action effect and the effect animation. Useful for QTE animations.")]
    public bool delayHitEffects;
    [Tooltip("The type of slide that should precede this animation: none, a small slide forward, or a long slide into melee range of the target(s).")]
    public SlideType slideType;
    [Tooltip("Time it takes for the user to slide to the target(s).")]
    public float slideDuration = 0.5f;
    private float stepForwardDistance = 5f;     // The distance of the STEP_FORWARD slide type
    [Tooltip("The distance to stop away from the target(s) after a slide and before attacking.")]
    public float meleeReach = 10f;
    [SerializeField, Tooltip("Time into the user animation before the effect takes place.")]
    protected float _timeBeforeEffect;
    [SerializeField, Tooltip("Time to wait after the action effect takes place.")]
    protected float _timeAfterEffect;

    public virtual IEnumerator Animate(FightingEntity user, List<FightingEntity> targets) {
        // Determine the start and end positions for the melee slide
        Vector3 userStartingPos = user.transform.position;
        Vector3 userDestination = Vector3.zero;
        
        // Perform slide
        if (slideType != SlideType.NONE) {
            SetSlidePositions(user, targets, ref userDestination);
            yield return GameManager.Instance.time.GetController().StartCoroutine(Slide(user.transform, userStartingPos, userDestination));
        }

        // Perform action animation
        AnimateUser(user);
        yield return GameManager.Instance.time.GetController().WaitForSeconds(_timeBeforeEffect);
        if (delayHitEffects) {
            yield return GameManager.Instance.time.GetController().WaitForSeconds(0.05f);
        }

        // Perform hit visuals on target(s)
        AnimateTargetEffects(user, targets);
        yield return GameManager.Instance.time.GetController().WaitForSeconds(_timeAfterEffect);

        // Slide back to starting position if necessary
        if (slideType != SlideType.NONE && user != null) {
            yield return GameManager.Instance.time.GetController().StartCoroutine(Slide(user.transform, userDestination, userStartingPos));
        }
    }

    protected virtual void SetSlidePositions(FightingEntity user, List<FightingEntity> targets, ref Vector3 userDestination) {
        if (slideType == SlideType.STEP_FORWARD) {
            userDestination = user.transform.position;
            userDestination.x += user.IsHeroTeam() ? -stepForwardDistance : stepForwardDistance;
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

    protected IEnumerator Slide(Transform transform, Vector3 start, Vector3 end) {
        float elapsedTime = 0;
        float t = 0;
        while (transform.position != end) {
            elapsedTime += GameManager.Instance.time.deltaTime;
            t = Mathf.SmoothStep(0, 1, elapsedTime / slideDuration);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
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

    protected virtual void AnimateTargetEffects(FightingEntity user, List<FightingEntity> targets) {
        Debug.Log("TARGET EFFECTS");
        foreach (FightingEntity target in targets) {
            if (target != null) {
                InstantiateHitEffect(target);
                TargetFlash(target);
            }
        }
    }

    protected virtual void InstantiateHitEffect(FightingEntity target) {
        FighterPositions positions = target.GetComponent<FighterPositions>();
        if (targetHitEffect != null && positions != null) {
            GameObject hitEffect = Instantiate(targetHitEffect, positions.damagePopup);
            hitEffect.transform.localScale = target.transform.localScale;
        }
    }

    protected void TargetFlash(FightingEntity target) {
        if (targetFlash) {
            GameManager.Instance.time.GetController().StartCoroutine(target.GetAnimController().Flash());
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
