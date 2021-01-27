using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CopyOnWrite.Caches
{
    public class CachingUnblockedObtainCopyOnWriteMeasured : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        private Dictionary<string, string> _cacheIpToNameToWrite = new Dictionary<string, string>();
        private IReadOnlyDictionary<string, string> _cacheIpToNameToRead = new Dictionary<string, string>();

        public CachingUnblockedObtainCopyOnWriteMeasured(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            var stopwatch = new Stopwatch();
            var cacheHit = false;
            var obtained = false;
            long waitingTime = 0;
            if (!_cacheIpToNameToRead.TryGetValue(ip, out var result))
            {                
                obtained = true;
                result = _nsLookup.GetNameFromIpSimple(ip);
                stopwatch.Start();
                lock (_cacheIpToNameToWrite)
                {
                    stopwatch.Stop();
                    waitingTime = stopwatch.ElapsedMilliseconds;
                    _cacheIpToNameToWrite[ip] = result;
                    var copy = _cacheIpToNameToWrite.ToImmutableDictionary();
                    _cacheIpToNameToRead = copy;
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
