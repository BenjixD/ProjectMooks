using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActionAnimation", menuName = "ActionsAnimations/DefaultActionAnimation", order = 0)]
public class ActionAnimation : ScriptableObject {
    [Tooltip("The name of the default animation played for the user of this action. Alternatively, you can override Animate().")]
    public string userAnimName;
    [Tooltip("The prefab that will be instantiated atop the target(s).")]
    public GameObject targetHitEffect;
    [Tooltip("Set to true if user should slide to the target(s) before attacking.")]
    public bool slideIn;
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
        float fighterRadius = 0;
        foreach (FightingEntity target in targets) {
            userDestination += target.transform.position;
            fighterRadius += target.GetComponent<FighterPositions>().fighterRadius;
        }
        userDestination /= targets.Count;
        fighterRadius /= targets.Count;

        // Distance target and destination based on the reach of this attack and the width of the target
        float distance = meleeReach + fighterRadius;
        userDestination.x += user.IsHeroTeam() ? distance : -distance;
        if (slideIn) {
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
        if (slideIn) {
            float elapsedTime = 0;
            float t = 0;
            while (user.transform.position != userStartingPos) {
                elapsedTime += GameManager.Instance.time.deltaTime;
                // elapsedTime += Time.deltaTime;
                t = Mathf.SmoothStep(0, 1, elapsedTime / slideDuration);
                user.transform.position = Vector3.Lerp(userDestination, userStartingPos, t);
                yield return null;
            }
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
        if (slideIn) {
            time += slideDuration;
        }
        return time;
    }

    public float GetAnimCooldown() {
        float time = _timeAfterEffect;
        if (slideIn) {
            time += slideDuration;
        }
        return time;
    }
}
