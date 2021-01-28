using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CopyOnWrite.Caches
{
    class CachingKeySeparationConcurrentDictLazyMeasured: INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        ConcurrentDictionary<string, Lazy<string>> _cacheIpToName = new ConcurrentDictionary<string, Lazy<string>>();

        public CachingKeySeparationConcurrentDictLazyMeasured(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            bool cacheHit = true;
            if (!_cacheIpToName.TryGetValue(ip, out var result))
            {
                lock (_cacheIpToName)
                {
                    if (!_cacheIpToName.TryGetValue(ip, out result))
                    {
                        _cacheIpToName[ip] = result = new Lazy<string>(() => 
                        {
                            cacheHit = false;
                            return _nsLookup.GetNameFromIpSimple(ip);
                        });
                    }
                }
            }
            
            return new Response(result.Value, cacheHit, !cacheHit, 0);
        }
    }
}
