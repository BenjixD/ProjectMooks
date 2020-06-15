using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class FightingEntity : MonoBehaviour
{
    public string Name;
    public PlayerStats stats;

    public Job job;

    public List<ActionBase> actions = new List<ActionBase>();

    public int targetId;
    public string targetName;

    public FighterSlot fighterSlot {get; set;}

    public bool isRare = false;

    [SerializeField]
    protected float rareRate = 0.01f;

    // Message box to display messages. Leave null if you don't want it to be used.
    [Header("Nullable")]
    public FighterMessageBox fighterMessageBox;

    protected QueuedAction _queuedAction;
    protected AnimationController _animController;
    protected FightingEntityAI _ai;
    protected AilmentController _ailmentController;

    protected virtual void Awake() {
        _animController = GetComponent<AnimationController>();
        Messenger.AddListener<BattleResult>(Messages.OnBattleEnd, this.OnBattleEnd);
    }

    protected virtual void OnDestroy() {
        Messenger.RemoveListener<BattleResult>(Messages.OnBattleEnd, this.OnBattleEnd);
    }
    
    public void Initialize(int index, PlayerCreationData data) {
        this.targetId = index;
		Name = data.name;

        SetStats(data.stats);
        SetJob(data.job);
        _ai = new FightingEntityAI(this);
        this.targetName = GameManager.Instance.battleComponents.field.GetTargetNameFromIndex(index);
        _ailmentController = new AilmentController(this);
        Debug.Log("Initialize: " + Name);

        // Sets the default energy for mooks to be always a constant
        // TODO: Do this a more cleaner way
        if (!this.isEnemy() && index != 0) {
            OutOfJuiceAilment outOfJuicePrefab = (OutOfJuiceAilment)GameManager.Instance.models.GetCommonStatusAilment("Out of Juice");
            this._ailmentController.AddStatusAilment(Instantiate(outOfJuicePrefab));
            data.stats.SetMana(outOfJuicePrefab.duration);
            data.stats.maxMana = outOfJuicePrefab.duration;
        }

	}

    public void SetStats(PlayerStats stats) {
        this.stats = stats;
    }

    // Getters / Setters 
    public string GetJobName() {
        return job.ToString();
    }


    public void SetJob(Job job) {
        this.job = job;
        if (this.isEnemy()) {
            actions = new List<ActionBase>(GameManager.Instance.models.GetEnemyJobActions(job));
        } else if (this.IsHero()) {
            actions = new List<ActionBase>(GameManager.Instance.models.GetPlayerJobActions(job));
        } else {
            float randomSeed = Random.value;

            if (randomSeed <= this.rareRate) {
                this.isRare = true; // TODO: Modify Mook based on rare status
            }

            // Mooks have more specific logic regarding skills because they can only have at most 3 specific, 1 general.
            List<ActionBase> actionPool = new List<ActionBase>(GameManager.Instance.models.GetPlayerJobActions(job));
            if (actionPool.Count <= 3) {
                actions = actionPool;
            } else {
                actions = new List<ActionBase>();
                while (actions.Count < 3) {
                    int randIndex = Random.Range(0, actionPool.Count);
                    actions.Add( actionPool[randIndex] );
                    actionPool.RemoveAt(randIndex);
                }

                
            }

            List<ActionBase> commonMookActionPool = GameManager.Instance.models.GetCommonMookActionPool();
            int commonActionIndex = Random.Range(0, commonMookActionPool.Count);
            actions.Add(commonMookActionPool[commonActionIndex]);
        }
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

    public void Animate(string animationName, bool loop) {
        if (_animController != null) {
            _animController.SetAnimation(animationName, loop);
        }
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

