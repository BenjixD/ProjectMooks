using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleCommand {
    public BattleOption option = BattleOption.ATTACK;
    public int target = 0; // Remember that chat will enter a number of target + 1
    public bool targetIsEnemy = true; // TODO: Allow for self targetting

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

    public float maxTimeBeforeAction = 15;
    public float timeCounter {get; set;}

    List<BattleCommand> _mookBattleCommands;

    [SerializeField]
    float _chatBubbleAnimationTime = 5.0f;

    List<BubbleAnimationValues> _mookBubbleAnimation;

    List<Player> players;

    BattlePhase battlePhase = BattlePhase.PLAYER;


    

    void Start() {
        players = GameManager.Instance.party.GetPlayersInPosition();

        _mookBattleCommands = new List<BattleCommand>();
        int numPlayers = players.Count;
        for (int i = 0; i < numPlayers; i++) {
            _mookBattleCommands.Add(new BattleCommand());
            _mookBubbleAnimation.Add(new BubbleAnimationValues());
        }

        // TODO: Waves
        GameManager.Instance.GenerateEnemyList();
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


    public void UpdateBattleOptionUI() {
        selectionCursor.transform.parent = battleOptionsUI[(int)(heroBattleCommand.option)].transform;
        selectionCursor.anchoredPosition = selectionCursorOffset;
    }

    public override void OnMessageReceived(string username, string message) {
        for (int i = 0; i < _mookBattleCommands.Count; i++) {
            if (username ==  players[i].Name) {
                this.HandleMessage(i, message);
            }
        }
    }

    public override void OnCommandReceived(string username, string message) {
        for (int i = 0; i < _mookBattleCommands.Count; i++) {
            if (username ==  players[i].Name) {
                this.HandleCommand(i, message);
            }
        }
    }

    public void HandleMessage(int mookIndex, string message) {
        // TODO: this can probably be handled in the Player class.
        _mookBubbleAnimation[mookIndex].counter = 0;
        _mookBubbleAnimation[mookIndex].message = message;

        if (_mookBubbleAnimation[mookIndex].isAnimating == true) {
            // Do nothing
        } else {
            StartCoroutine(displayChatBubble(mookIndex));
        }
    }

    public void HandleCommand(int mookIndex, string message) {
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

        if (target <= 0 || target >= GameManager.Instance.enemies.Count) {
            Debug.Log("Invalid target");
            return;
        }

         _mookBattleCommands[mookIndex].target = target - 1;
        
    }


    public void StartPlayerTurn() {
        
    }

    public void ExecutePlayerTurn() {
        // Sort by player speed
        List<Player> orderedPlayers = new List<Player>(players);
        orderedPlayers.Sort( (Player a, Player b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        foreach (var player in orderedPlayers) {
            int playerIndex = GameManager.Instance.party.GetPlayer(player.Name).Item1;

            BattleCommand playerCommand;
            
            if (playerIndex == 0) {
                playerCommand = heroBattleCommand;
            } else {
                playerCommand = _mookBattleCommands[playerIndex];
            }

            this.DoAction(playerCommand, player, GameManager.Instance.enemies[playerCommand.target]);
            
        }

        this.battlePhase = BattlePhase.ENEMY;
        this.ExecuteEnemyTurn();
    }

    public void ExecuteEnemyTurn() {
        List<Enemy> orderedEnemies = new List<Enemy>(GameManager.Instance.enemies);
        orderedEnemies.Sort( (Enemy a, Enemy b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });


        foreach (var enemy in orderedEnemies) {
            int enemyTarget = Random.Range(0, players.Count);
            BattleCommand command = new BattleCommand();
            command.target = enemyTarget;
            command.option = BattleOption.ATTACK; // TODO: Allow enemies to have other attacks
            FightingEntity playerTarget = players[enemyTarget];
            this.DoAction(command, enemy, playerTarget);
        }

        this.battlePhase = BattlePhase.PLAYER;
    }


    public void OnPlayerTurnStart() {
        foreach (var player in players) {
            if (player.modifiers.Contains("defend")) {
                player.modifiers.Remove("defend");
            }
        }
    }


    public void DoAction(BattleCommand command, FightingEntity a, FightingEntity b) {
        PlayerStats playerStats = a.stats;
        PlayerStats enemyStats = b.stats;
        int attackerAttack = 0;
        int defenderDefence = 0;
        int damage = 0;
        switch (command.option) {
            case BattleOption.ATTACK:
                attackerAttack = playerStats.GetPhysical();
                defenderDefence = playerStats.GetDefense();
                if (b.modifiers.Contains("defend")) {
                    defenderDefence *= 2;
                }

                damage = Mathf.Max(0, attackerAttack - defenderDefence);
                enemyStats.SetHp(enemyStats.GetHp() - damage);

            break;
            case BattleOption.DEFEND:
                a.modifiers.Add("defend");
            break;
            case BattleOption.MAGIC:
                // TODO: Get the spell
                attackerAttack = playerStats.GetSpecial();
                defenderDefence = playerStats.GetResistance();
                if (b.modifiers.Contains("defend")) {
                    defenderDefence *= 2;
                }

                damage = Mathf.Max(0, attackerAttack - defenderDefence);
                enemyStats.SetHp(enemyStats.GetHp() - damage);
            break;
        }
            
    }



    IEnumerator displayChatBubble(int mookIndex) {
        _mookBubbleAnimation[mookIndex].isAnimating = true;
        players[mookIndex].speechCanvasText.text = _mookBubbleAnimation[mookIndex].message;
        players[mookIndex].speechCanvas.gameObject.SetActive(true);


        while (_mookBubbleAnimation[mookIndex].counter < _chatBubbleAnimationTime) {
            players[mookIndex].speechCanvasText.text = _mookBubbleAnimation[mookIndex].message;
            _mookBubbleAnimation[mookIndex].counter += Time.deltaTime;
            yield return null;
        }

        players[mookIndex].speechCanvas.gameObject.SetActive(true);
         _mookBubbleAnimation[mookIndex].isAnimating = false;
    }

}
