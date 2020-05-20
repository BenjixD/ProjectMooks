using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Controller for the UI in battle
[RequireComponent (typeof(CommandCardUI))]
public class BattleUI : MonoBehaviour
{
    public CommandCardUI commandCardUI;

    public StatusBarsUI statusBarsUI;
    
    public TargetSelectionUI targetSelectionUI;

    public BattleOrderUI battleOrderUI;
}
