
using UnityEngine;
using System.Collections.Generic;


public class Fighter
{
    public string Name;

    public PlayerCreationData playerCreationData;

    // Player object (null if not on player scene)
    public FightingEntity fighter;
   
    public PlayerStats stats;
    public Job job;
    public List<ActionBase> actions = new List<ActionBase>();
    public bool isRare = false;

    [SerializeField]
    protected float rareRate = 0.01f;
    private List<PlayerReward> permanentRewards = new List<PlayerReward>();

    // Hero: Need to know that it is the hero (job)
    // Enemy: Need to know that it is the enemy (job). Need to generate stats on the fly
    // Mook: Generated based on PlayercreationData.

    public void Initialize(JobActionsList jobList, string name) {
        // Will be null for enemies, but not for players because we want to keep stats
       // if (creationData == null) {
            PlayerStats stats = (PlayerStats)jobList.prefab.stats.Clone();

            stats.RandomizeStats();

            this.playerCreationData = new PlayerCreationData(name, stats, jobList.job);
       // }
        
        this.CommonInitialization(name);
    }

    public void Initialize(PlayerCreationData data, string name) {
        this.playerCreationData = data;
        this.CommonInitialization(name);
    }

    private void CommonInitialization(string name) {
        this.Name = name;

        this.stats = (PlayerStats)this.playerCreationData.stats.Clone();
        this.stats.hp.SetValue(this.stats.maxHp.GetValue());
        this.SetJobAndActions(this.playerCreationData.job);
    }

    public T InstantiateFromJob<T>(JobActionsList jobList, string name, int targetId) where T : FightingEntity  {
        T prefab = (T)jobList.prefab;
        T instantiatedFighter = GameObject.Instantiate<T>(prefab) as T;

        instantiatedFighter.Initialize(targetId, this);

        foreach (PlayerReward reward in this.GetPermanentRewards()) {

            switch (reward.rewardType) {
                case PlayerRewardType.AILMENT:
                    PlayerRewardAilment ailmentReward = (PlayerRewardAilment)reward;
                    instantiatedFighter.GetAilmentController().AddStatusAilment(GameObject.Instantiate(ailmentReward.ailment));
                    break;

                default:
                    break;
            }
        }

        this.fighter = instantiatedFighter;
        return instantiatedFighter;
    }

    public List<PlayerReward> GetPermanentRewards() {
        return this.permanentRewards;
    }

    public void AddPermanentReward(PlayerReward reward) {
        this.permanentRewards.Add(reward);
    }

    public void SetJobAndActions(Job job) {
        this.job = job;
        if (this.isEnemy()) {
            actions = new List<ActionBase>(GameManager.Instance.models.GetEnemyJobActions(job));
        } else if (this.IsHero()) {
            actions = new List<ActionBase>(GameManager.Instance.models.GetPlayerJobActions(job));
        } else {
            float randomSeed = Random.value;

            if (randomSeed <= this.rareRate) {
                this.isRare = true; // TODO: Modify Mook based on rare status
            }

            // Mooks have more specific logic regarding skills because they can only have at most 3 specific, 1 general.
            List<ActionBase> actionPool = new List<ActionBase>(GameManager.Instance.models.GetPlayerJobActions(job));
            if (actionPool.Count <= 3) {
                actions = actionPool;
            } else {
                actions = new List<ActionBase>();
                while (actions.Count < 3) {
                    int randIndex = Random.Range(0, actionPool.Count);
                    actions.Add( actionPool[randIndex] );
                    actionPool.RemoveAt(randIndex);
                }
            }

            List<ActionBase> commonMookActionPool = GameManager.Instance.models.GetCommonMookActionPool();
            int commonActionIndex = Random.Range(0, commonMookActionPool.Count);
            actions.Add(commonMookActionPool[commonActionIndex]);
        }
    }

    public bool isEnemy() {
        return this.GetType() == typeof(Enemy);
    }


    public bool IsHero() {
        return this.job == Job.HERO;
    }
}
