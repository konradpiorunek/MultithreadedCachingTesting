using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace CopyOnWrite.Caches
{
    public class CachingUnblockedObtainMemoryCacheMeasured : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        private readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public CachingUnblockedObtainMemoryCacheMeasured(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }

        public Response GetNameFromIp(string ip)
        {
            var stopwatch = new Stopwatch();
            var cacheHit = false;
            var obtained = false;
            long waitingTimeCheck = 0;
            long waitingTimeSet = 0;
            stopwatch.Start();
            if (!_memoryCache.TryGetValue<string>(ip, out var result))
            {
                stopwatch.Stop();
                waitingTimeCheck = stopwatch.ElapsedMilliseconds;
                result = _nsLookup.GetNameFromIpSimple(ip);
                obtained = true;
                stopwatch.Start();
                _memoryCache.Set(ip, result);
                stopwatch.Stop();
                waitingTimeSet = stopwatch.ElapsedMilliseconds;
            }
            else
            {
                cacheHit = true;
            }
            return new Response(result, cacheHit, obtained, waitingTimeSet + waitingTimeCheck);
        }
    }

}
