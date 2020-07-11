

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rewards", menuName = "Rewards/StatReward", order = 1)]
public class StatReward : PlayerRewardAilment
{
    [Header("StatReward")]
    [Range(0, 200)]
    public int minPercentage;
    [Range(0, 200)]
    public int maxPercentage;

    // Randomized variables
    public float percentage {get; set;}
    public Stat statType {get; set;}

    public override void Initialize(Fighter player) {
        List<Stat> modifiableStats = new List<Stat>(player.stats.GetModifiableStats().Keys);
        statType = modifiableStats[Random.Range(0, modifiableStats.Count)];

        StatBuffAilment statAilment =  GameManager.Instance.models.GetStatBuffAilment(this.statType, percentage);
        statAilment.val = Random.Range(minPercentage, maxPercentage);

        this.percentage = statAilment.val;
        this.ailment = statAilment;
        this.name = GetStatDisplayName(statType) + " " + statAilment.val;
        

        this.description = "Raises " + statType.ToString() + " by: " + statAilment.val;
        if (statAilment.damageType == StatModifier.Type.ADD_PERCENTAGE) {
            this.description += "%";
            this.name += "%";
        }
    }

    public static string GetStatDisplayName(Stat type) {
        return type.ToString() + " Up";
    }
}
