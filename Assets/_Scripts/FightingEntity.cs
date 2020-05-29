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

    // Message box to display messages. Leave null if you don't want it to be used.
    [Header("Nullable")]
    public FighterMessageBox fighterMessageBox;

	protected QueuedAction _queuedAction;
	protected AnimationController _animController;
	protected FightingEntityAI _ai;
	protected AilmentController _ailmentController;


    // TODO: move this to somewhere more appropriate
    public const string MODIFIER_DEATH = "death";
    public const string MODIFIER_UNTARGETTABLE = "untargetable";
    public const string MODIFIER_CANNOT_USE_ACTION = "cannot move";


    // State of the entity that are not necessarily status ailments, but we would still like to know
    // e.g. untargettable, cannot do actions, death
    protected HashSet<string> modifiers = new HashSet<string>();
    


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
        
        // Sets the default energy for mooks to be always a constant
        // TODO: Do this a more cleaner way
        if (!this.isEnemy() && index != 0) {
            data.stats.SetMana(9);
            data.stats.maxMana = 9;
        }

		SetStats(data.stats);
		SetJob(data.job);
		_ai = new FightingEntityAI(this);
        this.targetName = GameManager.Instance.turnController.field.GetTargetNameFromIndex(index);
		_ailmentController = new AilmentController(this);
        Debug.Log("Initialize: " + Name);
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
            actions = GameManager.Instance.models.GetEnemyJobActions(job);
        } else {
		    actions =  GameManager.Instance.models.GetPlayerJobActions(job);
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

        this.AddModifier(MODIFIER_DEATH);
        this.AddModifier(MODIFIER_UNTARGETTABLE);
        this.AddModifier(MODIFIER_CANNOT_USE_ACTION);

        // TODO: Death animation

        // Important for Hero death
        this.gameObject.SetActive(false);
    }

    public bool HasModifier(string modifier) {
        return this.modifiers.Contains(modifier);
    }

    public void AddModifier(string modifier) {
        this.modifiers.Add(modifier);
    }

    public void RemoveModifier(string modifier) {
        if (!this.modifiers.Contains(modifier)) {
            return;
        }

        this.modifiers.Remove(modifier);
    }

    private void OnBattleEnd(BattleResult result) {
        Debug.Log("AI: " + Name + " " + _ai);
    	List<FightResult> myFights = result.results.Where(r => (r.fighter == this)).ToList();
    	_ai.ReviewFightResult(myFights);
    }

}

