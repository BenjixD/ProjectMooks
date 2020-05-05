
using UnityEngine;
using System.Collections.Generic;





public class FightingEntity : MonoBehaviour
{
    public string Name;
    public PlayerStats stats;

    public HashSet<string> modifiers = new HashSet<string>();

	public Job job;

	private List<ActionBase> _actions = new List<ActionBase>();
	private QueuedAction _queuedAction;


	public void SetStats(PlayerStats stats) {
		this.stats = stats;
	}

	// Getters / Setters 
	public string GetJobName() {
		return job.ToString();
	}


	public void SetJob(Job job) {
		this.job = job;
		_actions = GameManager.Instance.GetJobActionsList(job);
	}


	public void TryActionCommand(string message) {
		string[] splitCommand = message.Split(' ');
		foreach (ActionBase action in _actions) {
			if (action.TryChooseAction(this, splitCommand)) {
				return;
			}
		}
		Debug.Log("Invalid action command for player " + Name + ": " + message);
	}



	public void SetQueuedAction(QueuedAction queuedAction) {
		Debug.Log("Sucessfully set action " + queuedAction.GetAction().name + " for player " + this.Name);
		_queuedAction = queuedAction;
	}

    public QueuedAction  GetQueuedAction() {
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
    
}

