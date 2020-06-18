

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
    public StatType statType {get; set;}

    public override void Initialize() {
        statType = StatType.PHYSICAL; // TODO: Randomize this
        StatBuffAilment statAilment =  GameManager.Instance.models.GetStatBuffAilment(this.statType, percentage);
        statAilment.val = Random.Range(minPercentage, maxPercentage);

        this.percentage = statAilment.val;
        this.ailment = statAilment;
        this.name = GetStatDisplayName(statType) + " " + statAilment.val;
        

        this.description = "Raises " + statType.ToString() + " by: " + statAilment.val;
        if (statAilment.damageType == StatBuffAilment.Type.PERCENTAGE) {
            this.description += "%";
            this.name += "%";
        }
        
        
    }

    public static string GetStatDisplayName(StatType type) {
        return type.ToString() + " Up";
    }
}
