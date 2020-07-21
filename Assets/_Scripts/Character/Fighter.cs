
using UnityEngine;
using System.Collections.Generic;


public class Fighter
{
    public string Name;
    public int index;

    public PlayerCreationData playerCreationData;

    // Player object (null if not on player scene)
    public FightingEntity fighter;
   
    public PlayerStats stats;
    public Job job;
    public List<ActionBase> actions = new List<ActionBase>();
    public bool isRare = false;

    [SerializeField]
    protected float rareRate = 0.01f;

    public AilmentController ailmentController;


    // Hero: Need to know that it is the hero (job)
    // Enemy: Need to know that it is the enemy (job). Need to generate stats on the fly
    // Mook: Generated based on PlayercreationData.

    public void Initialize(JobActionsList jobList, string name) {
        this.playerCreationData = new PlayerCreationData(name, jobList.job);
    
        this.CommonInitialization(name);
    }

    public void Initialize(PlayerCreationData data, string name) {
        this.playerCreationData = data;
        this.CommonInitialization(name);
    }

    private void CommonInitialization(string name) {
        this.Name = name;
        this.ailmentController = new AilmentController(this);
        this.stats = (PlayerStats)this.playerCreationData.stats.Clone();
        this.stats.ResetStats();
        this.SetJobAndActions(this.playerCreationData.job);
    }

    public T InstantiateFromJob<T>(JobActionsList jobList, string name, int targetId) where T : FightingEntity  {
        T prefab = (T)jobList.prefab;
        T instantiatedFighter = GameObject.Instantiate<T>(prefab) as T;

        instantiatedFighter.Initialize(targetId, this);


        this.fighter = instantiatedFighter;
        return instantiatedFighter;
    }

    public void SetJobAndActions(Job job) {
        this.job = job;
        if (this.isEnemy()) {
            actions = GameManager.Instance.models.GetEnemyJobActionsInstance(job);
        } else if (this.IsHero()) {
            actions = GameManager.Instance.models.GetPlayerJobActionsInstance(job);
        } else {
            float randomSeed = Random.value;

            if (randomSeed <= this.rareRate) {
                this.isRare = true; // TODO: Modify Mook based on rare status
            }

            // Mooks have more specific logic regarding skills because they can only have at most 3 specific, 1 general.
            List<ActionBase> actionPool = GameManager.Instance.models.GetPlayerJobActionsInstance(job);
            if (actionPool.Count <= 3) {
                actions = actionPool;
            } else {
                actions = new List<ActionBase>();
                while (actions.Count < 3) {
                    int randIndex = Random.Range(0, actionPool.Count);
                    actions.Add(actionPool[randIndex]);
                    actionPool.RemoveAt(randIndex);
                }
                // Destroy unused actions
                for (int i = 0; i < actionPool.Count; i++) {
                    Object.Destroy(actionPool[i]);
                }
            }


            List<ActionBase> commonMookActionPool = GameManager.Instance.models.GetCommonMookActionPool();

            // Filter common moves if already have
            for (int i = commonMookActionPool.Count-1; i >= 0; i--) {
                if (actionPool.Find(action => action.name == commonMookActionPool[i].name ) != null) {
                    commonMookActionPool.RemoveAt(i);
                } 
            }

            int commonActionIndex = Random.Range(0, commonMookActionPool.Count);
            actions.Add(Object.Instantiate(commonMookActionPool[commonActionIndex]));


        }
    }

    public bool isEnemy() {
        return this.GetType() == typeof(Enemy);
    }


    public bool IsHero() {
        return this.job == Job.HERO;
    }
}
