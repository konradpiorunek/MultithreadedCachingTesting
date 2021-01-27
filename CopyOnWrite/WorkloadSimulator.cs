using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WorkloadSimulation
{

    public class WorkloadSimulator
    {
        List<string> _comparationSource;
        
        Random _random;
        public WorkloadSimulator(int seed)
        {
            _random = new Random(seed);
            _comparationSource = new TextRandomizer(seed).CreateRandomStrings(100, 100);
        }

        

        public void RunWorkNoLoad(int milliseconds)
        {
            if (milliseconds == 0)
                return;
            Task.Delay(milliseconds).Wait();
        }

        public void RunWorkNoLoadRandom(int maxMilliseconds)
        {
            var milliseconds = _random.Next(maxMilliseconds);
            RunWorkNoLoad(milliseconds);
        }



        private void DoCalculation()
        {
            long totalDiff = 0;
            var toDiff1 = _comparationSource[_random.Next(_comparationSource.Count)];
            var toDiff2 = _comparationSource[_random.Next(_comparationSource.Count)];
            for(var i = 0; i < toDiff1.Length && i < toDiff2.Length;++i)
            {
                var a = toDiff1[i];
                var b = toDiff2[i];
                var diff = Math.Abs(a * a + b * b);
                totalDiff += diff;
            }
        }

        public void RunWorkWithLoad(int milliseconds)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while(stopwatch.ElapsedMilliseconds < milliseconds)
            {
                DoCalculation();
            }
        }
    }
}
