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

    [Header("Impact Properties")]
    [Tooltip("The key for the user's sound that plays when the effect happens.")]
    public string effectSoundName;
    [Tooltip("The prefab that will be instantiated atop the target(s).")]
    public GameObject targetHitEffect;
    [Tooltip("Set to true to make the target(s) flash when the effect happens.")]
    public bool targetFlash = true;
    [Tooltip("Strength of camera shake on effect, if any.")]
    public ShakeStrength shakeStrength;
    [Tooltip("Set to true to add a slight delay between the action effect and the effect animation. Useful for QTE animations.")]
    public bool delayHitEffects;
    [SerializeField, Tooltip("Time into the user animation before the effect takes place.")]
    protected float _timeBeforeEffect;
    [SerializeField, Tooltip("Time to wait after the action effect takes place.")]
    protected float _timeAfterEffect;

    [Header("Slide Properties")]
    [Tooltip("The type of slide that should precede this animation: none, a small slide forward, or a long slide into melee range of the target(s).")]
    public SlideType slideType;
    [Tooltip("Time it takes for the user to slide to the target(s).")]
    public float slideDuration = 0.5f;
    private float stepForwardDistance = 5f;     // The distance of the STEP_FORWARD slide type
    [Tooltip("The distance to stop away from the target(s) after a slide and before attacking.")]
    public float meleeReach = 10f;

    public virtual IEnumerator Animate(FightingEntity user, List<FightingEntity> targets) {
        // Save the start position for the melee slide
        Vector3 userStartingPos = user.transform.position;
        
        // Perform slide
        if (slideType != SlideType.NONE) {
            yield return GameManager.Instance.time.GetController().StartCoroutine(SlideIn(user, targets));
        }

        // Perform action animation
        yield return GameManager.Instance.time.GetController().StartCoroutine(AnimateAction(user, targets));

        // Slide back to starting position if necessary
        if (slideType != SlideType.NONE && user != null) {
            yield return GameManager.Instance.time.GetController().StartCoroutine(Slide(user.transform, user.transform.position, userStartingPos));
        }
    }

    public virtual IEnumerator SlideIn(FightingEntity user, List<FightingEntity> targets) {
        if (slideType == SlideType.STEP_FORWARD) {
            Vector3 destination = user.transform.position;
            destination.x += user.IsHeroTeam() ? -stepForwardDistance : stepForwardDistance;
            yield return GameManager.Instance.time.GetController().StartCoroutine(Slide(user.transform, user.transform.position, destination));
        } else if (slideType == SlideType.MELEE) {
            Vector3 destination = Vector3.zero;
            float averageTargetRadius = 0;
            foreach (FightingEntity target in targets) {
                destination += target.transform.position;
                averageTargetRadius += target.GetComponent<FighterPositions>().fighterRadius;
            }
            destination /= targets.Count;
            averageTargetRadius /= targets.Count;
            // Distance target and destination based on the reach of this attack and the width of the target
            float distance = meleeReach + averageTargetRadius;
            destination.x += user.IsHeroTeam() ? distance : -distance;
            yield return GameManager.Instance.time.GetController().StartCoroutine(Slide(user.transform, user.transform.position, destination));
        }
    }

    public IEnumerator Slide(Transform transform, Vector3 start, Vector3 end) {
        float elapsedTime = 0;
        float t = 0;
        while (transform.position != end) {
            elapsedTime += GameManager.Instance.time.deltaTime;
            t = Mathf.SmoothStep(0, 1, elapsedTime / slideDuration);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    public IEnumerator AnimateAction(FightingEntity user, List<FightingEntity> targets) {
        // Perform user's animation
        user.PlaySound("action");
        AnimateUser(user);
        yield return GameManager.Instance.time.GetController().WaitForSeconds(_timeBeforeEffect);
        if (delayHitEffects) {
            yield return GameManager.Instance.time.GetController().WaitForSeconds(0.05f);
        }

        // Perform hit sounds and visuals on target(s)
        PlayTargetEffects(user, targets);
        yield return GameManager.Instance.time.GetController().WaitForSeconds(_timeAfterEffect);
    }

    protected virtual void AnimateUser(FightingEntity user) {
        AnimationController controller = user.GetAnimController();
        int track = controller.TakeFreeTrack();
        if (track != -1) {
            controller.AddToTrack(track, userAnimName, false, 0);
            controller.EndTrackAnims(track);
        }
    }

    protected virtual void PlayTargetEffects(FightingEntity user, List<FightingEntity> targets) {
        if (shakeStrength != ShakeStrength.NONE) {
            GameManager.Instance.battleComponents.GetCameraController().Shake(shakeStrength);
        }
        user.PlaySound(effectSoundName);
        foreach (FightingEntity target in targets) {
            if (target != null) {
                InstantiateHitEffect(target);
                TargetFlash(target);
                target.PlaySound("hit");
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

    public float GetAnimWindup(bool includeSlideTime = true) {
        float time = _timeBeforeEffect;
        if (slideType != SlideType.NONE && includeSlideTime) {
            time += slideDuration;
        }
        return time;
    }

    public float GetAnimCooldown(bool includeSlideTime = true) {
        float time = _timeAfterEffect;
        if (slideType != SlideType.NONE && includeSlideTime) {
            time += slideDuration;
        }
        return time;
    }
}
