using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleCommand {
    public BattleOption option = BattleOption.ATTACK;
    int target = 0; // Remember that chat will enter a number of target + 1
    bool targetIsEnemy = true;

}

public enum BattleOption {
    ATTACK = 0,
    MAGIC = 1,
    DEFEND = 2,
    LENGTH = 3
};

public enum BattlePhase {
    PLAYER,
    ENEMY

};

public class BubbleAnimationValues {
    public float counter = 0;
    public bool isAnimating = false;
    public string message = "";
}


public class BattleController : TwitchChatListenerBase
{

    public BattleCommand heroBattleCommand = new BattleCommand();
    public List<RectTransform> battleOptionsUI;
    public RectTransform selectionCursor;
    public Vector2 selectionCursorOffset = new Vector2(0, 0);


    List<BattleCommand> _mookBattleCommands;

    [SerializeField]
    float _chatBubbleAnimationTime = 5.0f;

    List<BubbleAnimationValues> _mookBubbleAnimation;



    void Start() {
        _mookBattleCommands = new List<BattleCommand>();
        int numPlayers = GameManager.Instance.players.Count;
        for (int i = 0; i < numPlayers; i++) {
            _mookBattleCommands.Add(new BattleCommand());
            _mookBubbleAnimation.Add(new BubbleAnimationValues());
        }
    }

    public void UpdateBattleOptionUI() {
        selectionCursor.transform.parent = battleOptionsUI[(int)(heroBattleCommand.option)].transform;
        selectionCursor.anchoredPosition = selectionCursorOffset;
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            BattleOption nextBattleOption = (BattleOption)((int) heroBattleCommand.option + 1);
            if (nextBattleOption == BattleOption.LENGTH) {
                nextBattleOption = BattleOption.ATTACK;
            }

            heroBattleCommand.option = nextBattleOption;

            this.UpdateBattleOptionUI();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            BattleOption nextBattleOption = heroBattleCommand.option;
            if ((int) heroBattleCommand.option == 0) {
                nextBattleOption = (BattleOption) ((int)BattleOption.LENGTH - 1);
            } else {
                nextBattleOption = (BattleOption) ((int)nextBattleOption - 1);
            }

            heroBattleCommand.option = nextBattleOption;
            this.UpdateBattleOptionUI();
        }
    }

    public override void OnMessageReceived(string username, string message) {
        for (int i = 0; i < _mookBattleCommands.Count; i++) {
            if (username ==  GameManager.Instance.players[i].Name) {
                this.handleMessage(i, message);
            }
        }
    }

    public override void OnCommandReceived(string username, string message) {
        for (int i = 0; i < _mookBattleCommands.Count; i++) {
            if (username ==  GameManager.Instance.players[i].Name) {
                this.handleCommand(i, message);
            }
        }
    }

    public void handleMessage(int mookIndex, string message) {
        _mookBubbleAnimation[mookIndex].counter = 0;
        _mookBubbleAnimation[mookIndex].message = message;


        if (_mookBubbleAnimation[mookIndex].isAnimating == true) {
            // Do nothing
        } else {
            StartCoroutine(displayChatBubble(mookIndex));
        }
        
    }

    IEnumerator displayChatBubble(int mookIndex) {
        
        _mookBubbleAnimation[mookIndex].isAnimating = true;
        GameManager.Instance.players[mookIndex].speechCanvas.gameObject.SetActive(true);


        while (_mookBubbleAnimation[mookIndex].counter < _chatBubbleAnimationTime) {
            _mookBubbleAnimation[mookIndex].counter += Time.deltaTime;
            yield return null;
        }

        GameManager.Instance.players[mookIndex].speechCanvas.gameObject.SetActive(true);
         _mookBubbleAnimation[mookIndex].isAnimating = false;

    }


    public void handleCommand(int mookIndex, string message) {
        string[] split = message.Split(' ');
        if (split.Length < 1) {
            Debug.Log("Invalid battle command");
            return;
        }

        // Right now, format is: action target
        // action is one of: Attack(A), Magic(M), Defend(D)
        // target is an integer 1, 2, 3...

        string action = split[0];

        int target = 1;

        int.TryParse(split[1], out target);

        switch (action) {
            case "a":
            case "attack":
                _mookBattleCommands[mookIndex].option = BattleOption.ATTACK;
            break;

            case "magic":
            case "m":
                _mookBattleCommands[mookIndex].option = BattleOption.MAGIC;
            break;


            case "defend":
            case "d":
                _mookBattleCommands[mookIndex].option = BattleOption.DEFEND;
            break;

            default:
                Debug.Log("Invalid battle command");
            break;
        }

        
        

    }

    
}
