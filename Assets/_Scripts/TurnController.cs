using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattlePhase{
    PARTY_SETUP,
    MOVE_SELECTION,
    ENTER_BATTLE,
    START_TURN,
    END_TURN
};

public enum HeroInputActionState {
    SELECT_ACTION,
    SELECT_ENEMY_TARGET,
    SELECT_PLAYER_TARGET,
    BATTLE_START
}

[RequireComponent(typeof(BattleUI))]
[RequireComponent(typeof(StageInfo))]
public class TurnController : MonoBehaviour
{

    public BattleUI ui {get; set; }

    public StageInfo stage {get; set; }
    public Battle turn {get; set; }


    [Header ("References")]

    public CommandSelector commandSelector;


    [Header ("Other")]
    public float maxTimeBeforeAction = 15;
    public float playerActionCounter {get; set;}


    private int _heroActionIndex = 0;
    

    public bool inputActionsPhase{get; set; }


    private HeroInputActionState heroInputActionState = HeroInputActionState.SELECT_ACTION;
    private int _heroTargetIndex = 0;


    void Awake() {
        ui = GetComponent<BattleUI>();
        stage = GetComponent<StageInfo>();
    }

    void Start() {
        GameManager.Instance.TurnController = this;
        this.initialize();
    }

    private void initialize() {
        this.stage.Initialize();
        this.ui.statusBarsUI.Initialize();
        this.OnPlayerTurnStart();
    }


    // Update is called once per frame
    void Update()
    {

        if (inputActionsPhase == true) {

            switch (heroInputActionState) {
                case HeroInputActionState.SELECT_ACTION:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                       this.setHeroAction();
                    }
                    break;

                case HeroInputActionState.SELECT_ENEMY_TARGET:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.setHeroTarget();
                    }
                    break;

                default:

                    break;
            }

            playerActionCounter += Time.deltaTime;
            this.checkExecuteTurn();
        } else {
        }
    }


    public void OnPlayerTurnStart() {
        this.inputActionsPhase = true;
        this.turn = null;

        this.heroInputActionState = HeroInputActionState.SELECT_ACTION;

        this.initializeCommandCardActionUI();


        this.playerActionCounter = 0;
        
        // Reset defence state
        foreach (var player in stage.GetPlayers()) {
            player.ResetCommand();

            if (player.modifiers.Contains("defend")) {
                player.modifiers.Remove("defend");
            }
        }
    }

    private void checkExecuteTurn() {
        //bool timeOutIfChatTooSlow = (stage.GetHeroPlayer().HasSetCommand() && playerActionCounter >= this.maxTimeBeforeAction);
        bool timeOutIfChatTooSlow = false;
        bool startTurn = timeOutIfChatTooSlow || hasEveryoneEnteredActions();
        if (startTurn) {
            playerActionCounter = 0;
            this.ui.SetStateText("");
            this.executePlayerTurn();
        } else {
            this.UpdateStateText();

        }
    }

    private void executePlayerTurn() {
        this.inputActionsPhase = false;
        this.turn = new Battle(this);
        this.turn.StartTurn();
    }

    private void UpdateStateText() {
        if (!stage.GetHeroPlayer().HasSetCommand()) {
            this.ui.SetStateText("Waiting on streamer input");
        } else {
            int timer = (int)(this.maxTimeBeforeAction - playerActionCounter);
            this.ui.SetStateText("Waiting on Chat: " + timer);
        }
    }

    private void setHeroAction() {
        this.heroInputActionState = HeroInputActionState.SELECT_ENEMY_TARGET;
        _heroActionIndex = this.commandSelector.GetChoice();

        ActionBase heroAction = stage.GetHeroPlayer().actions[_heroActionIndex];
    
        // TODO: Differentiate between ALL and single target.
        this.ui.targetSelectionUI.InitializeTargetSelectionSingle(heroAction.GetPotentialTargets(stage.GetHeroPlayer()), 0, this.commandSelector);
    }

    private void setHeroTarget() {
        heroInputActionState = HeroInputActionState.BATTLE_START;
        this._heroTargetIndex = this.commandSelector.GetChoice();
        this.ui.targetSelectionUI.ClearSelection();
        stage.GetHeroPlayer().SetQueuedAction(new QueuedAction(stage.GetHeroPlayer(), stage.GetHeroPlayer().actions[_heroActionIndex], new List<int>{_heroTargetIndex}  ));
    }

    // Initializes the command card to display the hero's actions
    private void initializeCommandCardActionUI() {
        List<string> actionNames = stage.GetHeroPlayer().actions.Map((ActionBase action) => { return action.name; });
        this.ui.commandCardUI.InitializeCommandSelection(actionNames, 0, this.commandSelector);
    }

    private bool hasEveryoneEnteredActions() {
        foreach (var player in stage.GetPlayers()) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }

        return true;
    }
}
