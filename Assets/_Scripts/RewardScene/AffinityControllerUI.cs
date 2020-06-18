using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffinityControllerUI : MonoBehaviour
{

    public Transform affinityParent;
    public AffinityUI affinityPrefab;
    

    public void InitializeAffinities(Action<RewardAffinity> callback) {
        for (int i = 0; i < (int)RewardAffinity.LENGTH; i++) {
            RewardAffinity affinity = (RewardAffinity)i;
            AffinityUI affinityUI = Instantiate(affinityPrefab, affinityParent);
            affinityUI.Initialize(affinity, callback);
        }
    }
}
