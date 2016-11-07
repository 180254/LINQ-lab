using System;
using System.Collections.Generic;
using System.Linq;

namespace QuerySamples
{
    public static class LazyEx
    {
        public static IEnumerable<TResult> Lazy<TSource, TResult>(
            this IEnumerable<TSource> data,
            Func<IEnumerable<TSource>, TResult> func
        )
        {
            if (data.Any())
            {
                yield return func.Invoke(data);
            }
        }

        public static IEnumerable<double> BetterMax<T>(
            this IEnumerable<T> data,
            Func<T, double> func
        )
        {
            if (data.Any())
            {
                yield return data.Max(func);
            }
        }
    }
}