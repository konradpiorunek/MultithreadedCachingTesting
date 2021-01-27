using System.Collections.Generic;
using WorkloadSimulation;

namespace CopyOnWrite
{
    public class FakeNsLookup : ISimpleNameResolver
    {
        WorkloadSimulator _workloadSimulation;
        private readonly List<string> _possibleKnownAddresses;
        private readonly List<string> _possibleFakeIps;
        private readonly int _maxMillisecondsPerUse;
        private Dictionary<string, string> _ipToAddress = new Dictionary<string, string>();

        public FakeNsLookup(int seed, List<string> possibleKnownAddresses, List<string> possibleFakeIps, int maxMillisecondsPerUse)
        {
            _workloadSimulation = new WorkloadSimulator(seed);
            _possibleKnownAddresses = possibleKnownAddresses;
            _possibleFakeIps = possibleFakeIps;
            _maxMillisecondsPerUse = maxMillisecondsPerUse;
            for (var i = 0; i < _possibleFakeIps.Count && i < _possibleKnownAddresses.Count; ++i)
            {
                _ipToAddress[_possibleFakeIps[i]] = _possibleKnownAddresses[i];
            }
        }

        public string GetNameFromIpSimple(string possibleFakeIp)
        {
            _ipToAddress.TryGetValue(possibleFakeIp, out var result);
            _workloadSimulation.RunWorkNoLoad(_maxMillisecondsPerUse);
            return result;
        }
    }
}

