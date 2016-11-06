using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace QuerySamples
{
    // --------------------------------------------------------------------------------------------------------------

    internal class Benchmark
    {
        /* Ex param: print info about each iteration */
        public static bool ExDebug = false;

        /* ExMulti param: how many repeats do? */
        public static int EmRepeats = 1;

        /* ExMulti param: do decreasing (true) or only base collection size (false)? */
        public static bool EmDecreasing = false;

        /* ExMulti param: which method should be described? 0=None, 1=First, 2=Each */
        public static int EmDescMode = 1;

        // ----------------------------------------------------------------------------------------------------------

        internal class Result<T>
        {
            public IList<T> Value { get; }
            public TimeSpan Time { get; }

            public Result(IList<T> value, TimeSpan time)
            {
                Value = value;
                Time = time;
            }

            public void Describe()
            {
                Console.WriteLine(@"time={0}", Time.TotalMilliseconds);
                ObjectDumper.Write(Value);
                Console.WriteLine();
            }
        }

        // ----------------------------------------------------------------------------------------------------------

        public static Result<T> Ex<T>(Func<IEnumerable<T>> expr)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = expr.Invoke().ToList();
            stopwatch.Stop();

            if (ExDebug)
            {
                Console.WriteLine(@"debug.Benchmark.Ex={0}", stopwatch.Elapsed.TotalMilliseconds);
            }

            return new Result<T>(result, stopwatch.Elapsed);
        }

        // ----------------------------------a------------------------------------------------------------------------

        public static Result<T> Ex<T>(Func<IEnumerable<T>> expr, int repeats)
        {
            if (ExDebug)
            {
                Console.WriteLine(@"debug.Benchmark.Ex");
            }

            var times =
                from n in Enumerable.Range(0, repeats)
                let ex = Ex(expr)
                orderby ex.Time.Ticks
                select ex;

            return times.Skip(Math.Max(0, repeats/2 - 1)).First();
        }

        // ----------------------------------------------------------------------------------------------------------

        public static bool Unanimity<TA, TB>(IEnumerable<TA> expr1, IEnumerable<TB> expr2)
        {
            if (typeof(TA) != typeof(TB))
            {
                return false;
            }

            var list1 = expr1 as IList<TA> ?? expr1.ToList();
            var list2 = (IList<TA>) (expr2 as IList<TB> ?? expr2.ToList());

            return
                list1.Count() == list2.Count()
                && list1.All(value => list2.Contains(value))
                && list2.All(value => list1.Contains(value));
            // && list1.SequenceEqual(list2);
        }

        // ----------------------------------------------------------------------------------------------------------

        public static void ExMulti<TSource, TResult>(
            ICollection<TSource> testCollection,
            params Func<IList<TSource>, Func<IEnumerable<TResult>>>[] expr)
        {
            var collectionSizes = new List<int>();
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
                    exprResult[i] = Ex(expr[i](test), EmRepeats);
                    exprTimes[i].Add(exprResult[i].Time);

                    if ((EmDescMode == 1 && i == 0 && test.Count == testCollection.Count)
                        || (EmDescMode == 2 && test.Count == testCollection.Count))
                    {
                        exprResult[i].Describe();
                    }
                }

                var unaminity = true;
                for (var i = 1; i < expr.Length && unaminity; i++)
                {
                    unaminity &= Unanimity(
                        exprResult[0].Value,
                        exprResult[i].Value
                    );
                }

                exprUnanimities.Add(unaminity);
                collectionSizes.Add(test.Count);

                test = EmDecreasing
                    ? test.Where((c, i) => i%2 == 0).ToList()
                    : new List<TSource>();
            }

            // ------------------------------------------------------------

            Console.WriteLine(@"sizes: {0}", string.Join(", ", collectionSizes));

            for (var i = 0; i < expr.Length; i++)
            {
                Console.WriteLine(@"expr[{0}]-times: {1}", i,
                    string.Join(", ", exprTimes[i].Select(t => t.TotalMilliseconds)));
            }

            Console.WriteLine(@"unanimities: {0}", exprUnanimities.All(u => u));
        }
    }
}