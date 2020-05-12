
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class FightingEntity : MonoBehaviour
{
    public string Name;
    public PlayerStats stats;

    public HashSet<string> modifiers = new HashSet<string>();

	public Job job;

	public List<ActionBase> actions = new List<ActionBase>();

    public int targetId;

	protected QueuedAction _queuedAction;
	protected AnimationController _animController;
	protected FightingEntityAI _ai;

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
            actions = GameManager.Instance.GetEnemyJobActions(job);
        } else {
		    actions =  GameManager.Instance.GetPlayerJobActions(job);
        }
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
        return this.GetType() == typeof(Enemy);
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

    private void OnBattleEnd(BattleResult result) {
    	List<FightResult> myFights = result.results.Where(r => (r.fighter == this)).ToList();
    	_ai.ReviewFightResult(myFights);
    }
}

