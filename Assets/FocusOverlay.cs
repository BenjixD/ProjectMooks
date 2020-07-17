using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusOverlay : MonoBehaviour
{

    const string FOCUS_LAYER_NAME = "Focus Layer";

    private Dictionary<GameObject, int> previousLayers;

    public void EnableOverlay() {
        this.gameObject.SetActive(true);
    }

    public void DisableOverlay() {
        this.UnsetAllFocusOverlays(); 
        this.gameObject.SetActive(false);
    }

    public void SetFocusLayer(GameObject obj, bool setChildren = false) {
        if (previousLayers.ContainsKey(obj)) {
            return;
        }

        previousLayers[obj] = obj.layer;
        this.RecSetFocusLayer(obj, setChildren);
    }

    public void RecSetFocusLayer(GameObject obj, bool setChildren) {
        obj.layer = LayerMask.GetMask(FOCUS_LAYER_NAME);

        if (!setChildren) {
            return;
        }

        foreach (Transform child in obj.transform) {
            RecSetFocusLayer(child.gameObject, setChildren);
        }
    }

    private void UnsetAllFocusOverlays() {
        foreach (KeyValuePair<GameObject, int> pair in previousLayers) {
            UnsetFocusLayer(pair.Key, false); // true might have unintended side effects
        }
    }

    private void UnsetFocusLayer(GameObject obj, bool setChildren = false) {
        if (!previousLayers.ContainsKey(obj)) {
            return;
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
