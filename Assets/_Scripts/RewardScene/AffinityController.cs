using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardAffinity {
    DEFAULT = 0,
    STAT_FOCUS = 1,
    ABILITY_FOCUS = 2,
    SPECIAL_FOCUS = 3,
    MOOK_FOCUS = 4,
    LENGTH = 5,
};


public class AffinityController : MonoBehaviour
{

   public AffinityControllerUI ui;

   public Action callback;

   public void Initialize(Action callback) {
       this.callback = callback;
       this.ui.InitializeAffinities(this.SelectAffinity);
   }

   public void SelectAffinity(RewardAffinity affinity) {
       GameManager.Instance.gameState.progressData.affinity = affinity; 

       // Advance to next
       this.callback();
   }
}
