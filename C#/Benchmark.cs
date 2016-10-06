using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QuerySamples
{
    internal class Benchmark
    {
        private static TimeSpan Time(Func<IEnumerable> expr)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            expr.Invoke();

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static TimeSpan Ex(Func<IEnumerable> expr, int repeats = 1)
        {
            var times =
                from n in Enumerable.Range(0, repeats)
                let ticks = Time(expr).Ticks
                orderby ticks
                select ticks;

            var array = times as long[] ?? times.ToArray();
            var median = array[array.Length/2];

//            {
//                foreach (var t in array.Select(t => new TimeSpan(t)).ToArray())
//                {
//                    Console.WriteLine(@"debug.Benchmark.Ex={0} | {1}", t.Ticks, t);
//                }
//            }

            return new TimeSpan(median);
        }
    }
}