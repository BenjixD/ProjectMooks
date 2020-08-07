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

    public ValueWeight(){}
    public ValueWeight(T value, float weight) {
        this.value = value;
        this.weight = weight;
    }
};

[System.Serializable]
public class RandomPool<T> where T : class
{

    public List<ValueWeight<T>> pool;

    private float sum;


    public RandomPool(List<ValueWeight<T>> pool) {
        this.pool = pool;
        this.NormalizeWeights();
    }

    public RandomPool<T> Clone(RandomPool<T> other) {
        RandomPool<T> newPool = new RandomPool<T>(new List<ValueWeight<T>>(other.pool));
        return newPool;
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

    public List<ValueWeight<T>> GetPoolWeights() {
        return this.pool;
    }

    public void NormalizeWeights() {
        this.sum = 0;
        foreach (var pooledObject in this.pool) {
            this.sum += pooledObject.weight;
        }

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

    public List<T> PickN(int n, bool allowDuplicates = true) {
        List<T> ret = new List<T>();
        RandomPool<T> clone = this.Clone(this);

        for (int i = 0; i < n; i++) {
            T pickedValue = clone.PickOne();
            ret.Add(pickedValue);
            if (!allowDuplicates) {
                clone.RemoveValue(pickedValue);
            }
        }

        return ret;
    }


    public void ForAll(System.Action<ValueWeight<T>> action) {
        foreach (ValueWeight<T> valueWeight in this.pool ) {
            action(valueWeight);
        }
        
        // this.NormalizeWeights();
    }

    public void PrintWeights() {
        for (int i = 0; i < this.pool.Count; i++) {
            Debug.Log(i + " " +this.pool[i].weight);
        }
    }

    public int Count() {
        return this.pool.Count;
    }
}
