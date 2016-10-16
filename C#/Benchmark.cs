using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace QuerySamples
{
    // --------------------------------------------------------------------------------------------------------------

    internal class Benchmark
    {
        public static readonly bool DEBUG = true;

        // ----------------------------------------------------------------------------------------------------------

        internal class Result<T>
        {
            public IEnumerable<T> Value { get; }
            public TimeSpan Time { get; }

            public Result(IEnumerable<T> value, TimeSpan time)
            {
                Value = value;
                Time = time;
            }
        }

        // ----------------------------------------------------------------------------------------------------------

        public static Result<T> Ex<T>(Func<IEnumerable<T>> expr)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = expr.Invoke();
            stopwatch.Stop();

            if (DEBUG)
            {
                Console.WriteLine(@"debug.Benchmark.Ex={0} | {1}",
                    stopwatch.Elapsed.Ticks, stopwatch.Elapsed);
            }

            return new Result<T>(result, stopwatch.Elapsed);
        }

        // ----------------------------------a------------------------------------------------------------------------

        public static Result<T> Ex<T>(Func<IEnumerable<T>> expr, int repeats)
        {
            if (repeats < 3)
            {
                return Ex(expr);
            }

            var times =
                from n in Enumerable.Range(0, repeats)
                let ex = Ex(expr)
                orderby ex.Time.Ticks
                select ex;

            return times.Skip(repeats/2 - 1).First();
        }

        // ----------------------------------------------------------------------------------------------------------

        public static bool Unanimity<TA, TB>(Func<IEnumerable<TA>> expr1, Func<IEnumerable<TB>> expr2)
        {
            if (typeof(TA) != typeof(TB))
            {
                return false;
            }

            var result1 = expr1.Invoke();
            var result2 = expr2.Invoke();

            var list1 = result1 as IList<TA> ?? result1.ToList();
            var list2 = (IList<TA>) (result2 as IList<TB> ?? result2.ToList());

            return
                list1.Count() == list2.Count()
                // && list1.All(value => list2.Contains(value))
                // && list2.All(value => list1.Contains(value))
                && list1.SequenceEqual(list2);
        }

        // ----------------------------------------------------------------------------------------------------------

        public static void DecreasingTest<TSource, TResult>(
            ICollection<TSource> testCollection,
            params Func<IList<TSource>, Func<IEnumerable<TResult>>>[] expr)
        {
            var collSizes = new List<int>();
            var exprTimes = new List<TimeSpan>[expr.Length];
            var exprUnanimities = new List<bool>();

            for (var i = 0; i < expr.Length; i++)
            {
                exprTimes[i] = new List<TimeSpan>();
            }

            // ------------------------------------------------------------

            var test = new List<TSource>(testCollection);

            while (test.Count >= 2)
            {
                var exprResult = new Result<TResult>[expr.Length];

                for (var i = 0; i < expr.Length; i++)
                {
                    exprResult[i] = Ex(expr[i](test), 3);
                    exprTimes[i].Add(exprResult[i].Time);
                }

                var unaminity = true;
                for (var i = 1; i < expr.Length && unaminity; i++)
                {
                    var iCopy = i;
                    unaminity &= Unanimity(
                        () => exprResult[0].Value,
                        () => exprResult[iCopy].Value
                    );
                }
                exprUnanimities.Add(unaminity);

                collSizes.Add(test.Count);
                test = test.Where((c, i) => i%2 == 0).ToList();
            }

            // ------------------------------------------------------------

            Console.WriteLine(@"sizes: {0}", string.Join(", ", collSizes));
            for (var i = 0; i < expr.Length; i++)
            {
                Console.WriteLine(@"expr[{0}]-times: {1}", i, string.Join(", ", exprTimes[i].Select(t => t.Ticks)));
            }
            Console.WriteLine(@"unanimities: {0}", exprUnanimities.All(u => u));
        }
    }
}