﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    public RectTransform ActionMenu;
    public RectTransform CommentaryMenu;
    public Text CommentaryText;


    List<Player> _players;
    Player _heroPlayer;


    bool startActions = false;


    void Start() {
        _players = GameManager.Instance.party.GetPlayersInPosition();
        _heroPlayer = _players[0];

        Debug.Log("Players: " + _players.Count);
        for (int i = 0; i < _players.Count; i++) {
            Debug.Log(_players[i].Name);
        }

        // TODO: Waves
        GameManager.Instance.GenerateEnemyList();
        this.OnPlayerTurnStart();
    }

    // Update is called once per frame
    void Update()
    {

        if (startActions == false) {
            if (_heroPlayer.hasSetCommand == false) {
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


                if (Input.GetKeyDown(KeyCode.Z)) {
                    _heroPlayer.hasSetCommand = true;
                }
            } else {
                
                if (Input.GetKeyDown(KeyCode.Z)) {
                    _heroPlayer.hasSetCommand = false;
                }
            }



            playerActionCounter += Time.deltaTime;

            if (playerActionCounter >= this.maxTimeBeforeAction || hasEveryoneEnteredActions()) {
                playerActionCounter = 0;
                this.ExecutePlayerTurn();
            }
        }
    }

    private bool hasEveryoneEnteredActions() {
        foreach (var player in _players) {
            if (player.hasSetCommand == false) {
                return false;
            }
        }

        return true;
    }


    public void UpdateBattleOptionUI() {
        selectionCursor.transform.parent = battleOptionsUI[(int)(_heroPlayer.command.option)].transform;
        selectionCursor.anchoredPosition = selectionCursorOffset;
    }


    public void ExecutePlayerTurn() {
        this.startActions = true;
        this.ActionMenu.gameObject.SetActive(false);
        this.CommentaryMenu.gameObject.SetActive(true);

        // Sort by player speed
        List<FightingEntity> orderedPlayers = new List<FightingEntity>(GameManager.Instance.getAllFightingEntities());
        orderedPlayers.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        this.DoActionHelper(orderedPlayers, 0);
        
    }


    public void DoActionHelper(List<FightingEntity> orderedEntities, int curIndex) {
        if (curIndex == orderedEntities.Count) {
            this.OnPlayerTurnStart();
            return;
        }
        
        FightingEntity entity = orderedEntities[curIndex];
        FightingEntity enemyEntity;
        if (entity.GetType() == typeof(Enemy)) {
            entity.command.target = getRandomEnemyTarget();
            enemyEntity = _players[entity.command.target];
        } else {
            enemyEntity = GameManager.Instance.enemies[entity.command.target];
        }

        StartCoroutine(dummyAttackAnimation(entity, enemyEntity, orderedEntities, curIndex));
        
    }

    public int getRandomEnemyTarget() {
        int enemyTarget = Random.Range(0, _players.Count);
        return enemyTarget;
    }

    IEnumerator dummyAttackAnimation(FightingEntity a, FightingEntity b, List<FightingEntity> orderedEntities, int index) {

        string attackerName = a.Name;
        string attackName = a.command.option.ToString(); // TODO: Better names

        CommentaryText.text = "" + attackerName + " used " + attackName;
        yield return new WaitForSeconds(2f);
        this.DoAction(a, b);
        this.DoActionHelper(orderedEntities, index + 1);
    }

    public void OnPlayerTurnStart() {
        this.startActions = false;
        
        this.ActionMenu.gameObject.SetActive(true);
        this.CommentaryMenu.gameObject.SetActive(false);

        this.playerActionCounter = 0;
        foreach (var player in _players) {
            player.hasSetCommand = false;

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
