using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CopyOnWrite
{

    public class SystemSimulator
    {
        private readonly List<string> _possibleIps;
        private readonly INameResolver _nameResolver;
        private readonly int _staticRunTimeInMilliseconds;
        private Random _random;

        WorkloadSimulation.WorkloadSimulator _workloadSimulator = new WorkloadSimulation.WorkloadSimulator(12345);

        public SystemSimulator(int seed, List<string> possibleIps, INameResolver nameResolver, int staticRunTimeInMilliseconds)
        {
            _random = new Random(seed);
            _possibleIps = possibleIps;
            _nameResolver = nameResolver;
            _staticRunTimeInMilliseconds = staticRunTimeInMilliseconds;
        }

        public TimeSpan DoQuery(out Response response)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            _workloadSimulator.RunWorkNoLoad(_staticRunTimeInMilliseconds);
            var ip = _possibleIps[_random.Next(_possibleIps.Count)];
            response = _nameResolver.GetNameFromIp(ip);            
            stopWatch.Stop();
            return stopWatch.Elapsed;
        }

    }
}
