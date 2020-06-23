using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManagerTest : MonoBehaviour
{
    public TimeManager tm;

    public void Start() {

    }

    public void Update() {
        if(Input.GetKeyDown("q")) {
            if(tm.IsPaused()) {
                tm.UnPause();
            } else {
                tm.Pause();
            }
        }
        else if(Input.GetKeyDown("w")) {
            if(!tm.IsPaused()) {
                tm.GetController().PauseFor(5f);
            }
        }
    }

    IEnumerator Test() {
        while(true) {
            yield return tm.GetController().WaitForNextFrame();
        }
    }

    IEnumerator Test2() {
        yield return tm.GetController().WaitForSeconds(3f);
    }
}