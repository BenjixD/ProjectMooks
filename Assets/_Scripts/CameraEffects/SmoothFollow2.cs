using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {
        public Transform from;
        public Transform to;
        public float damping = 5.0f;

        void Update () {
            if (from == null || to == null) {
                return;
            }
            
            transform.position = Vector3.Slerp (from.transform.position, to.transform.position, GameManager.Instance.time.deltaTime * damping);
        }
}