
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
        if (n >= self.Count) {
            Debug.LogError("ERROR: n is greater than list count!");
            return new List<T>();
        }

        List<T> newList = new List<T>();
        for (int i = 0; i < n; i++) {
            int randomIndex = UnityEngine.Random.Range(0, self.Count);
            newList.Add(self[randomIndex]);
            self.RemoveAt(randomIndex);
        }

        return newList;
    }

}
