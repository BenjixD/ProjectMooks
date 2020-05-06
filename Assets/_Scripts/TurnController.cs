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


    int heroActionIndex = 0;
    

    public bool inputActionsPhase{get; set; }


    public Text stateText;



    private HeroInputActionState heroInputActionState = HeroInputActionState.SELECT_ACTION;
    private int heroTargetIndex = 0;


    void Awake() {
        ui = GetComponent<BattleUI>();
        stage = GetComponent<StageInfo>();
    }

    void Start() {
        GameManager.Instance.TurnController = this;
        this.Initialize();
    }

    private void Initialize() {
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
                       this.SetHeroAction();
                    }
                    break;

                case HeroInputActionState.SELECT_ENEMY_TARGET:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.SetHeroTarget();
                    }
                    break;

                default:

                    break;
            }

            playerActionCounter += Time.deltaTime;
            this.CheckExecuteTurn();
        } else {
            stateText.text = "";
        }
    }

    private void CheckExecuteTurn() {
        //bool timeOutIfChatTooSlow = (stage.GetHeroPlayer().HasSetCommand() && playerActionCounter >= this.maxTimeBeforeAction);
        bool timeOutIfChatTooSlow = false;
        bool startTurn = timeOutIfChatTooSlow || hasEveryoneEnteredActions();
        if (startTurn) {
            playerActionCounter = 0;
            this.ExecutePlayerTurn();
        } else {
            this.UpdateStateText();

        }
    }

    private void ExecutePlayerTurn() {
        this.inputActionsPhase = false;
        this.turn = new Battle(this);
        this.turn.StartTurn();
    }

    private void UpdateStateText() {
        if (!stage.GetHeroPlayer().HasSetCommand()) {
            stateText.text = "Waiting on streamer input";
        } else {
            stateText.text = "Waiting on Chat: " + (int)(this.maxTimeBeforeAction - playerActionCounter);
        }
    }

    public void OnPlayerTurnStart() {
        this.inputActionsPhase = true;
        this.turn = null;

        this.heroInputActionState = HeroInputActionState.SELECT_ACTION;

        this.InitializeCommandCardActionUI();


        this.playerActionCounter = 0;
        
        // Reset defence state
        foreach (var player in stage.GetPlayers()) {
            player.ResetCommand();

            if (player.modifiers.Contains("defend")) {
                player.modifiers.Remove("defend");
            }
        }
    }

    private void SetHeroAction() {
        this.heroInputActionState = HeroInputActionState.SELECT_ENEMY_TARGET;
        heroActionIndex = this.commandSelector.GetChoice();

        ActionBase heroAction = stage.GetHeroPlayer().actions[heroActionIndex];
    
        // TODO: Differentiate between ALL and single target.
        this.ui.targetSelectionUI.InitializeTargetSelectionSingle(heroAction.GetPotentialTargets(stage.GetHeroPlayer()), 0, this.commandSelector);
    }

    private void SetHeroTarget() {
        heroInputActionState = HeroInputActionState.BATTLE_START;
        this.heroTargetIndex = this.commandSelector.GetChoice();
        this.ui.targetSelectionUI.ClearSelection();
        stage.GetHeroPlayer().SetQueuedAction(new QueuedAction(stage.GetHeroPlayer(), stage.GetHeroPlayer().actions[heroActionIndex], new List<int>{heroTargetIndex}  ));
    }

    // Initializes the command card to display the hero's actions
    private void InitializeCommandCardActionUI() {
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
