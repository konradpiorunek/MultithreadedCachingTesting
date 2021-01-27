using System.Collections.Concurrent;
using System.Diagnostics;

namespace CopyOnWrite.Caches
{
    public class CachingSingleLockParallelDictMeasured : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        ConcurrentDictionary<string, string> _cacheIpToName = new ConcurrentDictionary<string, string>();

        public CachingSingleLockParallelDictMeasured(ISimpleNameResolver nsLookup)
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
                    obtained = true;
                    _cacheIpToName[ip] = result = _nsLookup.GetNameFromIpSimple(ip);
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
