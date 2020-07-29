using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FocusOverlay : MonoBehaviour
{
    const string FOCUS_LAYER_NAME = "Focus Layer";

    public float transitionTime = 0.25f;

    public Color transitionColorFrom;
    public Color transitionColorTo;
    public Image image;

    public float nonFocusedAlpha = 0.5f;


    private Dictionary<GameObject, int> previousLayers;

    public void Initialize() {
        this.previousLayers = new Dictionary<GameObject, int>();
    }


    public IEnumerator EnableOverlay() {
        this.gameObject.SetActive(true);

        float counter = 0;
        float t = 0;
        this.image.color = Color.Lerp(this.transitionColorFrom, this.transitionColorTo, t);
        yield return null;

        while (counter < this.transitionTime) {
            counter += GameManager.Instance.time.deltaTime;
            t = counter / this.transitionTime;
            this.image.color = Color.Lerp(this.transitionColorFrom, this.transitionColorTo, t);


            yield return null;
        }
    }

    public IEnumerator DisableOverlay() {
        float counter = 0;


        float t = 0;
        this.image.color = Color.Lerp(this.transitionColorTo, this.transitionColorFrom, t);
        yield return null;

        while (counter < this.transitionTime) {
            counter += GameManager.Instance.time.deltaTime;
            t = counter / this.transitionTime;
            this.image.color = Color.Lerp(this.transitionColorTo, this.transitionColorFrom, t);

            yield return null;
        }

        this.UnsetAllFocusOverlays(); 
        this.gameObject.SetActive(false);
    }

    public void AddToFocusLayer(GameObject obj, bool isFocused, bool setChildren = false) {
        if (obj == null || previousLayers.ContainsKey(obj)) {
            return;
        }

        if (!isFocused) {
            SkeletonAnimation[] skeletonAnimations = obj.GetComponentsInChildren<SkeletonAnimation>();

            foreach (SkeletonAnimation skeletonAnimation in skeletonAnimations) {
                skeletonAnimation.skeleton.SetColor(new Color(1, 1, 1, this.nonFocusedAlpha));
            }
        }

        previousLayers[obj] = obj.layer;
        this.RecSetFocusLayer(obj, setChildren);
    }

    public void RecSetFocusLayer(GameObject obj, bool setChildren) {
        obj.layer = LayerMask.NameToLayer(FOCUS_LAYER_NAME);

        if (!setChildren) {
            return;
        }

        foreach (Transform child in obj.transform) {
            RecSetFocusLayer(child.gameObject, setChildren);
        }
    }

    private void UnsetAllFocusOverlays() {
        List<GameObject> keys = new List<GameObject>(this.previousLayers.Keys);
        foreach (GameObject key in keys) {
            RemoveFromFocusLayer(key, false); // true might have unintended side effects
        }

        this.previousLayers.Clear();
    }

    public void RemoveFromFocusLayer(GameObject obj, bool setChildren = false) {
        if (obj == null || !previousLayers.ContainsKey(obj)) {
            return;
        }

        SkeletonAnimation[] skeletonAnimations = obj.GetComponentsInChildren<SkeletonAnimation>();

        foreach (SkeletonAnimation skeletonAnimation in skeletonAnimations) {
            skeletonAnimation.skeleton.SetColor(Color.white);
        }

        RecUnsetFocusLayer(obj, previousLayers[obj], setChildren);


        previousLayers.Remove(obj);
    }

    private void RecUnsetFocusLayer(GameObject obj, int layer, bool setChildren) {
        obj.layer = layer;

        if (!setChildren) {
            return;
        }

        foreach (Transform child in obj.transform) {
            RecUnsetFocusLayer(child.gameObject, layer, setChildren);
        }
    }

}
