using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class FightingEntity : MonoBehaviour
{
    public int targetId;

    public string targetName;

    public FighterSlot fighterSlot {get; set;}

    public Fighter persistentFighter;

    public string Name;
    public PlayerStats stats;

    public Job job { get { return this.persistentFighter.job; }}

    public List<ActionBase> actions { get { return this.persistentFighter.actions; } }
    public bool isRare { get { return this.persistentFighter.isRare; }}


    public AilmentController ailmentController { get { return this.persistentFighter.ailmentController; }}

    // Message box to display messages. Leave null if you don't want it to be used.
    [Header("Nullable")]
    public FighterMessageBox fighterMessageBox;

    protected QueuedAction _queuedAction;
    private AnimationController _animController;
    protected FightingEntityAI _ai;

    

    protected virtual void Awake() {
        _animController = GetComponent<AnimationController>();
        Messenger.AddListener<BattleResult>(Messages.OnBattleEnd, this.OnBattleEnd);
    }

    protected virtual void OnDestroy() {
        Messenger.RemoveListener<BattleResult>(Messages.OnBattleEnd, this.OnBattleEnd);
        ailmentController.RemoveOnBattleEndAilments();
        //for (int i = 0; i < actions.Count; i++) {
        //    Destroy(actions[i]);
        //}
    }
    
    public void Initialize(int index, Fighter persistentFighter) {
        this.targetId = persistentFighter.index;
        this.Name = persistentFighter.Name;
        this.persistentFighter = persistentFighter;
        this.stats = persistentFighter.stats;
        this._queuedAction = new QueuedAction(this);

        _ai = new FightingEntityAI(this);
        this.targetName = GameManager.Instance.battleComponents.field.GetTargetNameFromIndex(index);
	}

    // Getters / Setters 
    public string GetJobName() {
        return job.ToString();
    }

    public ActionBase GetActionFromName(string name) {
        foreach(ActionBase action in actions) {
            if(action.commandKeyword == name) {
                return action;
            }
        }
        return null;
    }

    public ActionChoiceResult TryActionCommand(string message) {
        string[] splitCommand = message.Split(' ');
        string firstCommand = splitCommand[0];

        bool shortCut = firstCommand.Length >= 5 && firstCommand.Substring(0, 4) == "move" && firstCommand[4] >= '1' && firstCommand[4] <= '4';
        ActionBase action = null;

        if (shortCut) {
            int index = int.Parse(firstCommand[4].ToString()) - 1;
            if (index < this.actions.Count) {
                action = this.actions[index];
                splitCommand[0] = action.commandKeyword;
            }
        } else {
            action = GetActionFromName(firstCommand);
        }

        if(action != null) {
            return action.TryChooseAction(this, splitCommand);
        }
        else {
            Debug.Log("Invalid action command for player " + Name + ": " + message);
            return new ActionChoiceResult(ActionChoiceResult.State.UNMATCHED);
        }
    }

    public ActionBase GetRecommendedAction() {
        return _ai.GetSuggestion();
    }

    public void SetQueuedAction(ActionBase action, List<int> targetIds, bool isAutoQueued = false) {
        _queuedAction.SetAction(action, targetIds, isAutoQueued);
        Messenger.Broadcast<QueuedAction>(Messages.OnSetQueuedAction, _queuedAction);
    }

    public QueuedAction GetQueuedAction() {
        return _queuedAction;
    }

    public bool HasSetCommand() {
        return _queuedAction.GetIsSet() || this.HasModifier(ModifierAilment.MODIFIER_CANNOT_USE_ACTION);
    }

    public void ResetCommand() {
        _queuedAction.Reset();
        Messenger.Broadcast<QueuedAction>(Messages.OnSetQueuedAction, _queuedAction);
    }

    public bool isEnemy() {
        return this.GetType() == typeof(EnemyObject);
    }
    
    public bool IsHeroTeam() {
        return !this.isEnemy();
    }

    public bool IsHero() {
        return !this.isEnemy() && this.targetId == 0;
    }

    public ActionBase GetRandomAction() {
        return actions[Random.Range(0, actions.Count)];
    }

    public AnimationController GetAnimController() {
        return _animController;
    }

    public List<ActionBase> GetFilteredActions(ActionType actionType) {
        return this.actions.Filter( (ActionBase action) => action.actionType == actionType );
    }

    public Color GetOrderColor() {
        if (this.isEnemy()) {
            return GameManager.Instance.gameState.enemyParty.IndexToColor(this.targetId);
        } else {
            return GameManager.Instance.gameState.playerParty.IndexToColor(this.targetId);
        }
    }

    public void DoDeathAnimation() {

        this.AddModifier(ModifierAilment.MODIFIER_DEATH);
        this.AddModifier(ModifierAilment.MODIFIER_UNTARGETTABLE);
        this.AddModifier(ModifierAilment.MODIFIER_CANNOT_USE_ACTION);

        // TODO: Death animation

        // Important for Hero death
        this.gameObject.SetActive(false);
    }

    // Modifer helpers
    public bool HasModifier(string modifier) {
        return this.ailmentController.GetAilment(modifier) != null;
    }

    // Adds a text-only ailment
    public void AddModifier(string modifier) {
        ModifierAilment ailment = (ModifierAilment)GameManager.Instance.models.GetCommonStatusAilment("ModifierAilment");
        this.ailmentController.AddStatusAilment(Instantiate(ailment).SetName(modifier));
    }

    public void RemoveModifier(string modifier) {
        if (!this.HasModifier(modifier)) {
            return;
        }


        this.ailmentController.RemoveStatusAilment(modifier);
    }

    private void OnBattleEnd(BattleResult result) {
        List<FightResult> myFights = result.results.Where(r => (r.fighter == this)).ToList();
        _ai.ReviewFightResult(myFights);
    }

}

