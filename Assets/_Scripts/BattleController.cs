using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BattlePhase {
    PLAYER,
    ENEMY

};


public class BattleController : MonoBehaviour
{
    public List<RectTransform> battleOptionsUI;
    public RectTransform selectionCursor;
    public Vector2 selectionCursorOffset = new Vector2(0, 0);

    public float maxTimeBeforeAction = 15;
    public float playerActionCounter {get; set;}


    List<Player> _players;
    Player _heroPlayer;

    BattlePhase battlePhase = BattlePhase.PLAYER;
    

    void Start() {
        _players = GameManager.Instance.party.GetPlayersInPosition();
        _heroPlayer = _players[0];

        // TODO: Waves
        GameManager.Instance.GenerateEnemyList();
        this.OnPlayerTurnStart();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            BattleOption nextBattleOption = (BattleOption)((int) _heroPlayer.command.option + 1);
            if (nextBattleOption == BattleOption.LENGTH) {
                nextBattleOption = BattleOption.ATTACK;
            }

            _heroPlayer.command.option = nextBattleOption;

            this.UpdateBattleOptionUI();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            BattleOption nextBattleOption = _heroPlayer.command.option;
            if ((int) _heroPlayer.command.option == 0) {
                nextBattleOption = (BattleOption) ((int)BattleOption.LENGTH - 1);
            } else {
                nextBattleOption = (BattleOption) ((int)nextBattleOption - 1);
            }

            _heroPlayer.command.option = nextBattleOption;
            this.UpdateBattleOptionUI();
        }

        if (battlePhase == BattlePhase.PLAYER) {
            playerActionCounter += Time.deltaTime;
            if (playerActionCounter >= this.maxTimeBeforeAction) {
                playerActionCounter = 0;
                this.ExecutePlayerTurn();
            }
        }
    }


    public void UpdateBattleOptionUI() {
        selectionCursor.transform.parent = battleOptionsUI[(int)(_heroPlayer.command.option)].transform;
        selectionCursor.anchoredPosition = selectionCursorOffset;
    }


    public void ExecutePlayerTurn() {
        // Sort by player speed
        List<Player> orderedPlayers = new List<Player>(_players);
        orderedPlayers.Sort( (Player a, Player b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        foreach (var player in orderedPlayers) {
            this.DoAction(player, GameManager.Instance.enemies[player.command.target]);
        }

        this.battlePhase = BattlePhase.ENEMY;
        this.ExecuteEnemyTurn();
    }

    public void ExecuteEnemyTurn() {
        List<Enemy> orderedEnemies = new List<Enemy>(GameManager.Instance.enemies);
        orderedEnemies.Sort( (Enemy a, Enemy b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });


        foreach (var enemy in orderedEnemies) {
            int enemyTarget = Random.Range(0, _players.Count);
            BattleCommand command = new BattleCommand();
            command.target = enemyTarget;
            command.option = BattleOption.ATTACK; // TODO: Allow enemies to have other attacks
            enemy.command = command;
            FightingEntity playerTarget = _players[enemyTarget];
            this.DoAction( enemy, playerTarget);
        }

        this.battlePhase = BattlePhase.PLAYER;
    }


    public void OnPlayerTurnStart() {

        this.playerActionCounter = 0;
        foreach (var player in _players) {
            if (player.modifiers.Contains("defend")) {
                player.modifiers.Remove("defend");
            }
        }
    }

    public void DoAction(FightingEntity a, FightingEntity b) {
        PlayerStats playerStats = a.stats;
        PlayerStats enemyStats = b.stats;
        int attackerAttack = 0;
        int defenderDefence = 0;
        int damage = 0;
        switch (a.command.option) {
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
}
