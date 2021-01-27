using System;
using System.Collections.Generic;
using System.IO;

namespace CopyOnWrite
{
    class Program
    {
        static string OutputFileName = "output.txt";
        static void Main(string[] args)
        {
            if (args.Length > 0)
                OutputFileName = args[0];
            const int seed = 123;

            // todo: Adjust this to your liking which test you want to run. Rebuild and run release version.

            SimpleCaseComparison(seed);
            //NumberOfThreadsComparison(seed);
            //LengthOfObtainComparison(seed);
            //StaticTimeLengthComparison(seed);
            //NumberOfRequestsComparison(seed);
            //NumberOfIpsToObtainComparison(seed);
            //CompareDeviation(seed);
        }

        private static void PrintResults(List<string> results)
        {
            File.AppendAllLines(OutputFileName, results);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }

        private static void NumberOfIpsToObtainComparison(int seed)
        {
            Console.WriteLine("NumberOfIpsToObtainComparison");
            var methods = new[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            var numbersOfIps = new[] { 10, 100, 1000, 10000, 100000 };
            foreach (var numberOfIps in numbersOfIps)
                foreach (var method in methods)
                {
                    var result = TestRunner.RunTestNTimes(
                        seed: seed,
                        numberOfJobs: 100000,
                        threads: 32,
                        maxLengthOfNsLookup: 200,
                        staticLengthOfAQueryWork: 0,
                        numberOfIps: numberOfIps,
                        method: method,
                        nTimes: 5);
                    PrintResults(result);
                }
        }
        private static void StaticTimeLengthComparison(int seed)
        {
            Console.WriteLine("StaticTimeLengthComparison");
            var methods = new[]  { 7, 9, 11, 13, 15, 18 };
            var staticCostsOfResults = new[] { 0, 1, 2, 4, 8, 16, 32 };
            foreach (var method in methods)
            {
                foreach (var staticCostOfResults in staticCostsOfResults)
                {
                    var result = TestRunner.RunTestNTimes(
                        seed: seed,
                        numberOfJobs: 10000,
                        threads: 64,
                        maxLengthOfNsLookup: 200,
                        staticLengthOfAQueryWork: staticCostOfResults,
                        numberOfIps: 100,
                        method: method,
                        nTimes: 5);
                    PrintResults(result);
                }
            }
        }

        private static void NumberOfRequestsComparison(int seed)
        {
            Console.WriteLine("StaticTimeLengthComparison");
            var methods = new[] { 18, 7, 9, 11, 13, 15, 18 };
            var numbersOfJobs = new[] { 100000, 1000000, 10000000, 100000000 };
            foreach (var numberOfJobs in numbersOfJobs)
                foreach (var method in methods)
                {
                    var result = TestRunner.RunTestNTimes(
                        seed: seed,
                        numberOfJobs: numberOfJobs,
                        threads: 64,
                        maxLengthOfNsLookup: 200,
                        staticLengthOfAQueryWork: 0,
                        numberOfIps: 200,
                        method: method,
                        nTimes: 5);
                    PrintResults(result);
                }
        }
        private static void LengthOfObtainComparison(int seed)
        {
            Console.WriteLine("LengthOfObtainComparison");
            var methods = new[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            var lengthsOfObtain = new[] { 500, 1000, 2000, 4000 };
            foreach (var lengthOfObtain in lengthsOfObtain)
                foreach (var method in methods)
                {
                    var result = TestRunner.RunTestNTimes(
                        seed: seed,
                        numberOfJobs: 1000,
                        threads: 32,
                        maxLengthOfNsLookup: lengthOfObtain,
                        staticLengthOfAQueryWork: 0,
                        numberOfIps: 300,
                        method: method,
                        nTimes: 5);
                    PrintResults(result);
                }
        }
        private static void NumberOfThreadsComparison(int seed)
        {
            Console.WriteLine("NumberOfThreadsComparison");
            var methods = new[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            var numbersOfThreads = new[] { 1, 4, 8, 16, 32, 64, 128, 256 };
            foreach (var numberOfThreads in numbersOfThreads)
                foreach (var method in methods)
                {
                    var result = TestRunner.RunTestNTimes(
                        seed: seed,
                        numberOfJobs: 1000,
                        threads: numberOfThreads,
                        maxLengthOfNsLookup: 200,
                        staticLengthOfAQueryWork: 0,
                        numberOfIps: 50,
                        method: method,
                        nTimes: 5);
                    PrintResults(result);
                }
        }
        private static void SimpleCaseComparison(int seed)
        {
            Console.WriteLine("SimpleCaseComparison");
            var result = TestRunner.RunTestNTimes(
                seed: seed,
                numberOfJobs: 10000,
                threads: 32,
                maxLengthOfNsLookup: 100,
                staticLengthOfAQueryWork: 0,
                numberOfIps: 50,
                method: 0,
                nTimes: 1);
            PrintResults(result);

            var methods = new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

            foreach (var method in methods)
            {
                result = TestRunner.RunTestNTimes(
                    seed: seed,
                    numberOfJobs: 10000,
                    threads: 32,
                    maxLengthOfNsLookup: 100,
                    staticLengthOfAQueryWork: 0,
                    numberOfIps: 50,
                    method: method,
                    nTimes: 5);
                PrintResults(result);
            }
        }

        private static void CompareDeviation(int seed)
        {
            Console.WriteLine("SimpleCaseComparison");
            var methods = new[] { 19, 2, 4, 6, 8, 10, 12, 14, 15, 16, 19 };            

            for (var n = 0; n < 3; n++)
                foreach (var method in methods)
                {
                    var result = TestRunner.RunTestNTimes(
                        seed: seed,
                        numberOfJobs: 10000,
                        threads: 64,
                        maxLengthOfNsLookup: 200,
                        staticLengthOfAQueryWork: 0,
                        numberOfIps: 100,
                        method: method,
                        nTimes: 1);
                    PrintResults(result);
                }
        }
    }
}
