using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CopyOnWrite
{
    public class TestRunner
    {
        public static List<string> RunTestNTimes(int seed, int numberOfJobs, int threads, int maxLengthOfNsLookup, int staticLengthOfAQueryWork, int numberOfIps, int method, int nTimes)
        {
            var result = new List<string>();
            for(var i = 0; i < nTimes;++i)
            {
                var testText = RunTest(seed, numberOfJobs, threads, maxLengthOfNsLookup, staticLengthOfAQueryWork, numberOfIps, method);
                result.Add(testText);
            }

            return result;
        }
        public static string RunTest(int seed, int numberOfJobs, int threads, int maxLengthOfNsLookup, int staticLengthOfAQueryWork, int numberOfIps, int method)
        {
            var testEnvironment = TestCaseEnvironmentGeneric.Create(seed, maxLengthOfNsLookup, staticLengthOfAQueryWork, numberOfIps, method);
            var resultOfTest = Run(threads, testEnvironment, numberOfJobs);
            return $"Jobs: {numberOfJobs}, Threads: {threads}, OperationMaxT:{maxLengthOfNsLookup}, StaticOpCost:{staticLengthOfAQueryWork}, CachingItems:{numberOfIps} - {resultOfTest}";
        }

        static string Run(int threads, TestCaseEnvironmentGeneric testCaseEnvironment, int numberOfJobs)
        {
            Console.WriteLine($"Starting run for {testCaseEnvironment.CachingStrategyName}");
            var resultsStorage = new List<ResponseContainer>();
            for(var i = 0; i < numberOfJobs; ++i)
            {
                resultsStorage.Add(new ResponseContainer());
            }
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();            
            JobRunner.RunXTimes(testCaseEnvironment.DoUnitOfWork, resultsStorage, threads);
            stopwatch.Stop();

            
            var (variance, standardDeviation, min, max, averageTicks, ticks) = CalculateVariance(resultsStorage);
            var ticksText = "";// string.Join(',', ticks);

            var times = resultsStorage.Select(st => st.RunTime).ToArray();
            var timeOfWork = times.Select(t => t.TotalMilliseconds).Sum();
            var averageMs = timeOfWork / numberOfJobs;
            var cacheHitsCount = resultsStorage.Where(r => r.Result.CacheHit).Count();
            var obtainedValuesCount = resultsStorage.Where(r => r.Result.ObtainedValue).Count();
            var waitingTime = resultsStorage.Select(r => r.Result.MillisecondsWaiting).Sum();
            var badCount = resultsStorage.Where(r => string.IsNullOrWhiteSpace(r.Result.Address)).Count();

            var summary = $"{testCaseEnvironment.CachingStrategyName} run: {stopwatch.ElapsedMilliseconds}ms, worked: {timeOfWork}ms, average job: {averageMs}ms, Cached#:{cacheHitsCount}, Obtains#:{obtainedValuesCount}, Bad#:{badCount}, Waiting[ms]:{waitingTime}, Min:{min}, Max:{max}, AvgTicks:{averageTicks} Variance:{variance}, StdDev:{standardDeviation}\n{ticksText}";
            return summary;
        }

        static (double variance, double standardDeviation, double min, double max, double averageTicks, List<long> ticks) CalculateVariance(List<ResponseContainer> responses)
        {
            var times = responses.Select(r => r.RunTime.Ticks).ToList();
            var totalTicks = times.Sum();
            var averageTicks = totalTicks / times.Count;
            var max = times.Max();
            var min = times.Min();
            long ss = 0;
            for (var i = 0; i < times.Count; ++i)
            {
                var diff = averageTicks - times[i];
                ss += diff * diff;
            }
            var variance = ss / (times.Count - 1);
            var standardDeviation = Math.Sqrt(variance);            
            times.Sort();
            return (variance, standardDeviation, min, max, averageTicks, times);
        }
    }
}
