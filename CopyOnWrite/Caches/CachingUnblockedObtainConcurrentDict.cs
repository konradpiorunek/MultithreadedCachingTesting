using System.Collections.Concurrent;

namespace CopyOnWrite.Caches
{

    public class CachingUnblockedObtainConcurrentDict : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        ConcurrentDictionary<string, string> _cacheIpToName = new ConcurrentDictionary<string, string>();

        public CachingUnblockedObtainConcurrentDict(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            if (!_cacheIpToName.TryGetValue(ip, out var result))
            {
                _cacheIpToName[ip] = result = _nsLookup.GetNameFromIpSimple(ip);
            }
            return new Response(result);
        }
    }
}
