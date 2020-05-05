using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Job Actions List", menuName = "Actions/Job Actions List", order = 1)]
public class JobActionsList : ScriptableObject {
    public Job job;
	public ActionBase[] availableActions;
	private Dictionary<ActionType, ActionBase> _actions = new Dictionary<ActionType, ActionBase>();
    
    public void Initialize() {
		foreach(ActionBase action in availableActions) {
			_actions.Add(action.actionType, action);
		}
    }

    public Dictionary<ActionType, ActionBase> GetActions() {
        return _actions;
    }
}
