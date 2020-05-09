
using UnityEngine;
using System.Collections.Generic;





public class FightingEntity : MonoBehaviour
{
    public string Name;
    public PlayerStats stats;

    public HashSet<string> modifiers = new HashSet<string>();

	public Job job;

	public List<ActionBase> actions = new List<ActionBase>();

    public int targetId;
	private QueuedAction _queuedAction;
	private AnimationController _animController;

	private void Awake() {
		_animController = GetComponent<AnimationController>();
	}
	
	public void Initialize(int index, PlayerCreationData data) {
        this.targetId = index;
		Name = data.name;
		SetStats(data.stats);
		SetJob(data.job);
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
		    _animController.AddAnimation(animationName, loop);
        }
	}

    public List<ActionBase> GetFilteredActions(ActionType actionType) {
        return this.actions.Filter( (ActionBase action) => action.actionType == actionType );
    }
}

