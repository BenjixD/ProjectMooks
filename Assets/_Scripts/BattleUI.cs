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

    [SerializeField]
    private Text stateText;

    public void SetStateText(string text) {
        this.stateText.text = text;
    }

}
