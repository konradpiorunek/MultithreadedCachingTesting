using System.Collections.Generic;
using System.Diagnostics;

namespace CopyOnWrite.Caches
{
    public class CachingSingleLockParallelDict2ndCheckMeasured : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;        
        Dictionary<string, string> _cacheIpToName = new Dictionary<string, string>();


        public CachingSingleLockParallelDict2ndCheckMeasured(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            var stopwatch = new Stopwatch();
            var cacheHit = false;
            var obtained = false;
            long waitingTime = 0;
            if (!_cacheIpToName.TryGetValue(ip, out var result))
            {
                stopwatch.Start();
                lock (_cacheIpToName)
                {
                    stopwatch.Stop();
                    waitingTime = stopwatch.ElapsedMilliseconds;
                    if (!_cacheIpToName.TryGetValue(ip, out result))
                    {                        
                        obtained = true;                        
                        _cacheIpToName[ip] = result = _nsLookup.GetNameFromIpSimple(ip); 
                    }
                    else
                    {
                        cacheHit = true;
                    }
                }
            }
            else
            {
                cacheHit = true;
            }
            return new Response(result, cacheHit, obtained, waitingTime);
        }
    }

}
