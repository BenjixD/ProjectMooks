using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Battle
{
    private TurnController _controller {get; set;}

    public BattleFight currentFight{get; set;}

    public Battle(TurnController controller) {
        _controller = controller;
    }

    public void StartTurn() {

        this.Initialize();
        // Sort by player speed
        List<FightingEntity> orderedPlayers = new List<FightingEntity>(_controller.stage.GetAllFightingEntities());
        orderedPlayers.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        _controller.StartCoroutine(handleTurn(orderedPlayers));

    }

    private void Initialize() {
        this.SetUnsetMookCommands();
        this._controller.ui.commandCardUI.InitializeCommantaryUI();
    }

    private IEnumerator handleTurn(List<FightingEntity> fighters) {
        for (int i = 0; i < fighters.Count; i++) {
            BattleFight fight = new BattleFight(_controller, fighters[i]);
            this.currentFight = fight;
            yield return _controller.StartCoroutine(fight.DoFight());
        }

        this.EndTurn();
    }

    private void EndTurn() {
        this._controller.OnPlayerTurnStart();
        this.currentFight = null;
    }

    private void SetUnsetMookCommands() {
        foreach (var player in _controller.stage.GetPlayers()) {
            if (player.HasSetCommand() == false) {
                player.SetQueuedAction(new QueuedAction(player, player.actions[0], new List<int>{_controller.stage.GetRandomEnemyIndex()}  ));
            }
        }
    }

    private void DoActionHelper(List<FightingEntity> orderedEntities, int curIndex) {
        if (curIndex == orderedEntities.Count) {
            this._controller.OnPlayerTurnStart();
            return;
        }
        
        FightingEntity entity = orderedEntities[curIndex];
        if (entity.GetType() == typeof(Enemy)) {
            entity.SetQueuedAction(new QueuedAction(entity, entity.GetRandomAction(), new List<int>{_controller.stage.GetRandomPlayerIndex()}  ));
        } else {
        }

        _controller.StartCoroutine(dummyAttackAnimation(entity, orderedEntities, curIndex));
    }

    private IEnumerator dummyAttackAnimation(FightingEntity a, List<FightingEntity> orderedEntities, int index) {

        string attackerName = a.Name;
        if (a.GetQueuedAction() == null) {
            Debug.LogError("Cannot find queued action: "  + attackerName);
            yield break;
        }
        string attackName = a.GetQueuedAction()._action.name;
        List<FightingEntity> targets = a.GetQueuedAction()._action.GetTargets(a, a.GetQueuedAction().GetTargetIds());

        string commentaryText;
        if (targets.Count == 1) {
            commentaryText = "" + attackerName + " used " + attackName + " on " + targets[0].Name;
        } else {
            commentaryText = "" + attackerName + " used " + attackName;
        }

        this._controller.ui.commandCardUI.SetCommentaryUIText(commentaryText);
        
        yield return new WaitForSeconds(2f);
        this.DoAction(a);
        this.DoActionHelper(orderedEntities, index + 1);
    }


    private void DoAction(FightingEntity a) {
        a.GetQueuedAction().ExecuteAction();
        this._controller.ui.statusBarsUI.UpdateStatusBarUI();
    }
}
