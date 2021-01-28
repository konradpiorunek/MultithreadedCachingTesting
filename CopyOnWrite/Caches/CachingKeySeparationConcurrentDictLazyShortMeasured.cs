using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CopyOnWrite.Caches
{
    class CachingKeySeparationConcurrentDictLazyShortMeasured: INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        ConcurrentDictionary<string, Lazy<string>> _cacheIpToName = new ConcurrentDictionary<string, Lazy<string>>();

        public CachingKeySeparationConcurrentDictLazyShortMeasured(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            bool cacheHit = true;
            var result = _cacheIpToName.GetOrAdd(
                ip, 
                new Lazy<string>(() =>
                { 
                    cacheHit = false;  return _nsLookup.GetNameFromIpSimple(ip);  
                }));

            return new Response(result.Value, cacheHit, !cacheHit, 0);
        }
    }
}
