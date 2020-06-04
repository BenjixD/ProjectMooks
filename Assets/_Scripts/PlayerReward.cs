

using UnityEngine;

[CreateAssetMenu(fileName = "Rewards", menuName = "Rewards/PlayerReward", order = 0)]
public class PlayerReward : ScriptableObject
{
    public string name;
    public string description;
    public StatusAilment ailment;
}
