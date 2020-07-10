using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestHelper : MonoBehaviour {
    [Tooltip("Set to true to instantly pick the middle path on the map.")]
    public bool autoNavigate;
    [Tooltip("Set to true to skip the connection scene.")]
    public bool autoConnect;

    private void Start() {
        DontDestroyOnLoad(gameObject);
        if (autoNavigate) {
            StartCoroutine(ChooseStage());
        }
    }

    private IEnumerator ChooseStage() {
        yield return new WaitForSeconds(0.01f);
        GameObject.Find("WorldMapCanvas").GetComponent<MapNavigator>().TryMoveTo(new Coord(1, 1));
        if (autoConnect) {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Battle");
        }
    }
}
