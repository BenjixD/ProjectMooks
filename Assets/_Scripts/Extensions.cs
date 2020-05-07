
using System;
using System.Collections.Generic;
using System.Linq;



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

}
