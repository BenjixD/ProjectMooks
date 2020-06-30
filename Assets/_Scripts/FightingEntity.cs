using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class FightingEntity : MonoBehaviour
{
    public string Name;
    public PlayerStats stats;

    public Job job;

    public List<ActionBase> actions;
    public bool isRare = false;

    public int targetId;
    public string targetName;

    public FighterSlot fighterSlot {get; set;}

    // Message box to display messages. Leave null if you don't want it to be used.
    [Header("Nullable")]
    public FighterMessageBox fighterMessageBox;

    protected QueuedAction _queuedAction;
    private AnimationController _animController;
    protected FightingEntityAI _ai;
    protected AilmentController _ailmentController;

    protected virtual void Awake() {
        _animController = GetComponent<AnimationController>();
        Messenger.AddListener<BattleResult>(Messages.OnBattleEnd, this.OnBattleEnd);
    }

    protected virtual void OnDestroy() {
        Messenger.RemoveListener<BattleResult>(Messages.OnBattleEnd, this.OnBattleEnd);
    }
    
    public void Initialize(int index, Fighter persistentFighter) {
        this.targetId = index;
		this.Name = persistentFighter.Name;
        this.stats = persistentFighter.stats;
        this.job = persistentFighter.job;
        this.actions = persistentFighter.actions;
        this.isRare = persistentFighter.isRare;

        _ai = new FightingEntityAI(this);
        this.targetName = GameManager.Instance.battleComponents.field.GetTargetNameFromIndex(index);
        _ailmentController = new AilmentController(this);
        Debug.Log("Initialize: " + Name);

        // Sets the default energy for mooks to be always a constant
        // TODO: Do this a more cleaner way
        if (!this.isEnemy() && index != 0) {
            OutOfJuiceAilment outOfJuicePrefab = (OutOfJuiceAilment)GameManager.Instance.models.GetCommonStatusAilment("Out of Juice");
            this._ailmentController.AddStatusAilment(Instantiate(outOfJuicePrefab));
            this.stats.mana.SetValue(outOfJuicePrefab.duration);
            this.stats.maxMana.SetValue(outOfJuicePrefab.duration);
        }

	}

    // Getters / Setters 
    public string GetJobName() {
        return job.ToString();
    }

    public AilmentController GetAilmentController() {
        return _ailmentController;
    }

    public void TryActionCommand(string message) {
        string[] splitCommand = message.Split(' ');
        foreach (ActionBase action in actions) {
            if (action.TryChooseAction(this, splitCommand)) {
                return;
            }
        }
        
        Debug.Log("Invalid action command for player " + Name + ": " + message);
    }

    public ActionBase GetRecommendedAction() {
        return _ai.GetSuggestion();
    }

    public void SetQueuedAction(QueuedAction queuedAction) {
        _queuedAction = queuedAction;
        Messenger.Broadcast<QueuedAction>(Messages.OnSetQueuedAction, _queuedAction);
    }

    public QueuedAction GetQueuedAction() {
        return _queuedAction;
    }

    public bool HasSetCommand() {
        return _queuedAction != null;
    }

    public void ResetCommand() {
        _queuedAction = null;
    }

    public bool isEnemy() {
        return this.GetType() == typeof(EnemyObject);
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
        return this._ailmentController.GetAilment(modifier) != null;
    }

    // Adds a text-only ailment
    public void AddModifier(string modifier) {
        ModifierAilment ailment = (ModifierAilment)GameManager.Instance.models.GetCommonStatusAilment("ModifierAilment");
        this._ailmentController.AddStatusAilment(Instantiate(ailment).SetName(modifier));
    }

    public void RemoveModifier(string modifier) {
        if (!this.HasModifier(modifier)) {
            return;
        }


        this._ailmentController.RemoveStatusAilment(modifier);
    }

    private void OnBattleEnd(BattleResult result) {
        Debug.Log("AI: " + Name + " " + _ai);
        List<FightResult> myFights = result.results.Where(r => (r.fighter == this)).ToList();
        _ai.ReviewFightResult(myFights);
    }

}

