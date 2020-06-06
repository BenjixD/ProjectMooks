
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class Extension
{
    // List extensions
    public static List<R> Map<T, R>(this IEnumerable<T> self, Func<T, R> selector) {
        return self.Select(selector).ToList();
    }

    public static T Reduce<T>(this IEnumerable<T> self, Func<T, T, T> func) {
        return self.Aggregate(func);
    }

    public static List<T> Filter<T>(this IEnumerable<T> self, Func<T, bool> predicate) {
        return self.Where(predicate).ToList();
    }

    public static List<T> CreateNRandom<T>(this List<T> self, int n) {
        if (self.Count == 0) {
            Debug.LogError("ERROR: No elements in list");
        }

        List<T> refernece = new List<T>(self);

        int ogCount = refernece.Count;
        bool repeats = n >= ogCount;

        List<T> newList = new List<T>();
        for (int i = 0; i < n; i++) {
            int randomIndex;
            
            if (!repeats) {
                randomIndex = UnityEngine.Random.Range(0, refernece.Count);
            } else {
                randomIndex = UnityEngine.Random.Range(0, ogCount);
            }

            newList.Add(refernece[randomIndex]);
            if (!repeats) {
                refernece.RemoveAt(randomIndex);
            }
        }

        return newList;
    }

}
