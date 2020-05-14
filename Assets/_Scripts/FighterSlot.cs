using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterSlot : MonoBehaviour
{
    public Transform instantiationTransform;

    public bool IsXFlipped() {
        return instantiationTransform.transform.localScale.x < 0; 
    }
}
