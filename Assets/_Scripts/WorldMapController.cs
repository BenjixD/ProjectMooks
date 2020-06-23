using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMapController : MonoBehaviour
{
    public Camera mainCamera;
    public List<Transform> locations;

    public float transitionSpeed = 1f;

    public string battleScene;

    void Start() {
        Vector3 pos1;
        Vector3 pos2;
        if (GameManager.Instance.gameState.progressData.nextStageIndex == 0) {
            pos1 = locations[0].position;
            pos2 = locations[1].position;
        } else {
            pos1 = locations[GameManager.Instance.gameState.progressData.currentStageIndex+1].position;
            pos2 = locations[GameManager.Instance.gameState.progressData.nextStageIndex+1].position;
        }

        StartCoroutine(doTransitionAnimation(pos1, pos2));
    }

    private IEnumerator doTransitionAnimation(Vector3 pos1, Vector3 pos2) {

        float counter = 0;
        while (counter <= 1) {
            Vector3 slerp = Vector3.Slerp(pos1, pos2, counter);
            mainCamera.transform.position = new Vector3(slerp.x, slerp.y, mainCamera.transform.position.z);

            counter += GameManager.Instance.time.deltaTime * transitionSpeed;
            yield return null;
        }

        yield return GameManager.Instance.time.GetController().WaitForSeconds(1.0f);

        GameManager.Instance.gameState.progressData.currentStageIndex = GameManager.Instance.gameState.progressData.nextStageIndex;
        SceneManager.LoadScene(battleScene);
    }

}
