using CopyOnWrite.Caches;
using WorkloadSimulation;

namespace CopyOnWrite
{
    public class TestCaseEnvironmentGeneric
    {
        private readonly SystemSimulator _systemSimulator;       

        public string CachingStrategyName { get; }

        public static TestCaseEnvironmentGeneric Create(int seed, int maxLengthOfNsLookup, int staticRunTimeInMilliseconds, int numberOfIps, int method)
        {
            var ips = new TextRandomizer(seed).CreateRandomStrings(length: 14, numberOfIps);
            var names = new TextRandomizer(seed).CreateRandomStrings(length: 32, numberOfIps);
            var nsLookup = new FakeNsLookup(seed, names, ips, maxLengthOfNsLookup);
            var nameResolver = GetNameResolverByMethodNumber(method, nsLookup);
            var systemSimulator = new SystemSimulator(seed, ips, nameResolver, staticRunTimeInMilliseconds);

            return new TestCaseEnvironmentGeneric(systemSimulator, nameResolver.GetType().Name);
        }

        private static INameResolver GetNameResolverByMethodNumber(int method, FakeNsLookup nsLookup)
        {
            switch (method)
            {
                case 0: return new NoCachingDecorator(nsLookup); // Linearly slow
                case 1: return new CachingNoLockDictCanBreak(nsLookup); // Erroneous

                case 2: return new CachingSingleLockNaiveMeasured(nsLookup); // Saturation queued
                case 3: return new CachingSingleLockParallelDict(nsLookup); // Saturation queued
                case 4: return new CachingSingleLockParallelDictMeasured(nsLookup); // Saturation queued
                case 5: return new CachingSingleLockParallelDict2ndCheck(nsLookup); // Saturation queued but reused
                case 6: return new CachingSingleLockParallelDict2ndCheckMeasured(nsLookup); // Saturation queued but reused

                case 7: return new CachingUnblockedObtainConcurrentDict(nsLookup); // Non blocking saturation
                case 8: return new CachingUnblockedObtainConcurrentDictMeasured(nsLookup); // Non blocking saturation
                case 9: return new CachingUnblockedObtainMemoryCache(nsLookup); // Non blocking saturation
                case 10: return new CachingUnblockedObtainMemoryCacheMeasured(nsLookup); // Non blocking saturation
                case 11: return new CachingUnblockedObtainCopyOnWrite(nsLookup); // Non blocking saturation
                case 12: return new CachingUnblockedObtainCopyOnWriteMeasured(nsLookup); // Non blocking saturation
                
                case 13: return new CachingKeySeparationLockConcurrentDict(nsLookup); // Non blocking saturation, blocking value obtain per key
                case 14: return new CachingKeySeparationLockConcurrentDictMeasured(nsLookup); // Non blocking saturation, blocking value obtain per key
                case 15: return new CachingKeySeparationLockCopyOnWrite(nsLookup); // Non blocking saturation, blocking value obtain per key
                case 16: return new CachingKeySeparationLockCopyOnWriteMeasured(nsLookup); // Non blocking saturation, blocking value obtain per key
                case 17: return new CachingKeySeparationLockCopyOnWriteRefactored(nsLookup); // Non blocking saturation, blocking value obtain per key
                case 18: return new CachingKeySeparationLockCopyOnWriteRefactoredOVLO(nsLookup); // Non blocking saturation, blocking value obtain per key
                case 19: return new CachingKeySeparationLockCopyOnWriteRefactoredOVLOMeasured(nsLookup); // Non blocking saturation, blocking value obtain per key
                case 20: return new CachingKeySeparationConcurrentDictLazy(nsLookup);



            }
            return null;
        }

        public TestCaseEnvironmentGeneric(SystemSimulator systemSimulator, string cachingStrategyName)
        {
            _systemSimulator = systemSimulator;
            CachingStrategyName = cachingStrategyName;
        }

        public void DoUnitOfWork(ResponseContainer responseContainer)
        {
            responseContainer.RunTime = _systemSimulator.DoQuery(out var response);
            responseContainer.Result = response;                  
        }
    }
}

