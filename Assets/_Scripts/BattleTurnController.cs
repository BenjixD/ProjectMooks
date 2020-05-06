using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent (typeof(BattleController))]
public class BattleTurnController : MonoBehaviour
{
    private BattleController _controller {get; set;}

    void Awake() {
        _controller = GetComponent<BattleController>();
    }

    public void StartTurn() {

    }
}
