using System.Collections.Generic;
using System.Diagnostics;

namespace CopyOnWrite.Caches
{
    public class CachingSingleLockNaiveMeasured : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;       
        Dictionary<string, string> _cacheIpToName = new Dictionary<string, string>();

        public CachingSingleLockNaiveMeasured(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            var stopwatch = new Stopwatch();
            var cacheHit = false;
            var obtained = false;
            stopwatch.Start();
            lock (_cacheIpToName)
            {
                stopwatch.Stop();
                var waitingTime = stopwatch.ElapsedMilliseconds;
                if (!_cacheIpToName.TryGetValue(ip, out var result))
                {
                    obtained = true;
                    _cacheIpToName[ip] = result = _nsLookup.GetNameFromIpSimple(ip);
                }           
                else
                {
                    cacheHit = true;
                }

                return new Response(result, cacheHit, obtained, waitingTime);
            }
        }
    }
}
