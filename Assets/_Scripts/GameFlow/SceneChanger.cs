using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToChangeTo;

    public void ChangeScene(string sceneName) {
        this.sceneToChangeTo = sceneName;
        this.ChangeScene();
    } 

    public void ChangeScene() {
        SceneManager.LoadScene(this.sceneToChangeTo);
    } 
}
