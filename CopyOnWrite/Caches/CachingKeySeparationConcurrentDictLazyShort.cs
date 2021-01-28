using System;
using System.Collections.Concurrent;

namespace CopyOnWrite.Caches
{
    class CachingKeySeparationConcurrentDictLazyShort: INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        ConcurrentDictionary<string, Lazy<string>> _cacheIpToName = new ConcurrentDictionary<string, Lazy<string>>();

        public CachingKeySeparationConcurrentDictLazyShort(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            var result = _cacheIpToName.GetOrAdd(ip, new Lazy<string>(() => _nsLookup.GetNameFromIpSimple(ip)));
            
            return new Response(result.Value);
        }
    }
}
