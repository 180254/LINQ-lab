using System;
using System.Collections.Generic;
using System.Linq;

namespace QuerySamples
{
    public static class BetterMaxEx
    {
        public static IEnumerable<double> BetterMax<T>(this IEnumerable<T> data, Func<T, double> func)
        {
            // casting to (double?): avoid exception if data is empty
            var max = data.Max(x => (double?) func(x));
            if (max != null)
            {
                yield return (double) max;
            }
        }
    }
}