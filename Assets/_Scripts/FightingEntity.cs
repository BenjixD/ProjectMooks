
using UnityEngine;
using System.Collections.Generic;



public class BattleCommand {
    public BattleOption option = BattleOption.ATTACK;
    public int target = 0; // Remember that chat will enter a number of target + 1
    public bool targetIsEnemy = true; // TODO: Allow for self targetting

}

public enum BattleOption {
    ATTACK = 0,
    MAGIC = 1,
    DEFEND = 2,
    LENGTH = 3
};


public class FightingEntity : MonoBehaviour
{
    public string Name;
    public PlayerStats stats;

    public BattleCommand command = new BattleCommand();

    public HashSet<string> modifiers = new HashSet<string>();

    public bool hasSetCommand = false;

	public Job job;

	private Dictionary<ActionType, ActionBase> _actions = new Dictionary<ActionType, ActionBase>();
	private QueuedAction _queuedAction;


	public void SetStats(PlayerStats stats) {
		this.stats = stats;
	}

	public void SetJob(Job job) {
		this.job = job;
		_actions = GameManager.Instance.GetJobActionsList(job);
	}


	public void TryActionCommand(ActionType actionType, string[] splitCommand) {
		if (_actions.ContainsKey(actionType)) {
			if (!_actions[actionType].TryChooseAction(this, splitCommand)) {
				Debug.Log("Trying to use Action of type " + actionType + " failed");
			}
		} else {
			Debug.Log("Player does not have Action of type " + actionType);
		}
	}

	public void SetQueuedAction(QueuedAction queuedAction) {
		Debug.Log("Sucessfully set action " + queuedAction.GetAction() + " for player " + this.Name);
		_queuedAction = queuedAction;
	}
}

