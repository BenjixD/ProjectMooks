using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/*
[System.Serializable]
public class ValaueWeightRewardRarityWrapper : ValueWeight<RewardRarityWrapper> {

}


[System.Serializable]
public class RewardRarityWrapper : EnumWrapper<RewardRarity> {

}


[System.Serializable]
public class ValueWeightRewardRarity : RandomPool<RewardRarityWrapper>
{
    public List<ValueWeightRewardRarity> pool2;
    public ValueWeightRewardRarity(List<ValueWeight<RewardRarityWrapper>> pool, List<float> weights) : base(pool, weights)
    {
    }
}
*/

// Wrapper for an enum (because must contain reference type)
// Example usage:     public RandomPool<EnumWrapper<RewardRarity>> pool;
[System.Serializable]
public class EnumWrapper<T> where T : struct {
    public T enumValue;
}


[System.Serializable]
public class ValueWeight<T> where T : class {
    public T value;
    public float weight;
};

[System.Serializable]
public class RandomPool<T> where T : class
{

    public List<ValueWeight<T>> pool;

    private float sum;


    public RandomPool(List<ValueWeight<T>> pool, List<float> weights) {
        if (pool.Count != weights.Count) {
            Debug.LogError("ERROR: invalid weight function");
        }
        this.pool = pool;

        this.sum = 0;
        foreach (var pooledObject in this.pool) {
            this.sum += pooledObject.weight;
        }

        this.NormalizeWeights();
    }

    public void AddValue(ValueWeight<T> val) {
        this.pool.Add(val);
        this.sum += val.weight;
    }

    public void RemoveValue(T val) {
        for (int i = this.pool.Count-1; i >= 0; i--) {
            if (this.pool[i].value == val) {
                this.sum -= this.pool[i].weight;
                this.pool.RemoveAt(i);
            }
        }
    }

    public List<ValueWeight<T>> GetPool() {
        return this.pool;
    }

    private void NormalizeWeights() {
        for (int i = 0; i < this.pool.Count; i++) {
            this.pool[i].weight /= this.sum;

        }

        this.sum = 1;
    }

    public T PickOne() {
        float counter = 0;
        ValueWeight<T> pickedObject = null;
        float randomValue = Random.Range(0, this.sum);

        foreach (ValueWeight<T> pooledObject in this.pool) {
            pickedObject = pooledObject;
            counter += pooledObject.weight;
            if (counter >= randomValue) {
                break;
            }
        }

        if (pickedObject == null) {
            Debug.LogError("ERROR with RandomPool algorithm");
        }

        return pickedObject.value;
    }

    public List<T> PickN(int n) {
        List<T> ret = new List<T>();
        for (int i = 0; i < n; i++) {
            T pickedValue = this.PickOne();
            ret.Add(pickedValue);
            this.RemoveValue(pickedValue);
        }

        return ret;
    }
}
