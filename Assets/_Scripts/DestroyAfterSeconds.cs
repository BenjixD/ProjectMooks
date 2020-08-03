using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{

    public float duration;

    private float counter = 0;

    // Update is called once per frame
    void Update()
    {
        counter += GameManager.Instance.time.deltaTime;
        if (counter >= duration) {
            counter = 0;
            Destroy(this.gameObject);
        }
    }
}
