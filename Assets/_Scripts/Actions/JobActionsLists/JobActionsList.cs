using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Job Actions List", menuName = "Actions/Job Actions List", order = 1)]
public class JobActionsList : ScriptableObject {
    public Job job;
	[SerializeField] private List<ActionBase> _actions = null;

    public FightingEntity prefab;
    
    public List<ActionBase> GetActions() {
        return _actions;
    }
}
