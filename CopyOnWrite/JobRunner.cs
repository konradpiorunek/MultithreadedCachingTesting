
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CopyOnWrite
{
    public class JobRunner
    {
        public static void RunXTimes(Action<ResponseContainer> jobToRun, IEnumerable<ResponseContainer> responses, int threads)
        {
            Parallel.ForEach(responses, new ParallelOptions { MaxDegreeOfParallelism = threads }, jobToRun);
        }        
    }
}
