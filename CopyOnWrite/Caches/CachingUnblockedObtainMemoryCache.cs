using Microsoft.Extensions.Caching.Memory;

namespace CopyOnWrite.Caches
{
    public class CachingUnblockedObtainMemoryCache : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        private readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public CachingUnblockedObtainMemoryCache(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }

        public Response GetNameFromIp(string ip)
        {
            if (!_memoryCache.TryGetValue<string>(ip, out var result))
            {                    
                result = _nsLookup.GetNameFromIpSimple(ip);
                _memoryCache.Set(ip, result);
            }
            return new Response(result);
        }        
    }

}
