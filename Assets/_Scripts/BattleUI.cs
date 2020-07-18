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

    public PlayerQueueUI playerQueueUI;

    public FighterSlotUI targetIconsUI;

    public BasicText tmp_TimerText;

    public FocusOverlay focusOverlay;

    public void Initialize() {
        this.commandCardUI.Initialize();
        this.statusBarsUI.Initialize();
        this.targetSelectionUI.Initialize();
        this.battleOrderUI.Initialize();
        this.playerQueueUI.Initialize();
        this.targetIconsUI.Initialize();
        this.focusOverlay.Initialize();
    }
}
