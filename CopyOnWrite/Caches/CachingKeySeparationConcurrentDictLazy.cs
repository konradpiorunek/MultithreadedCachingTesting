using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CopyOnWrite.Caches
{
    class CachingKeySeparationConcurrentDictLazy: INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        ConcurrentDictionary<string, Lazy<string>> _cacheIpToName = new ConcurrentDictionary<string, Lazy<string>>();

        public CachingKeySeparationConcurrentDictLazy(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            if (!_cacheIpToName.TryGetValue(ip, out var result))
            {
                lock (_cacheIpToName) // Add a Lazy instance to the dictionary only one at the time
                {
                    if (!_cacheIpToName.TryGetValue(ip, out result))
                    {
                        _cacheIpToName[ip] = result = new Lazy<string>(() => _nsLookup.GetNameFromIpSimple(ip));
                    }
                }
            }
            return new Response(result.Value);
        }
    }
}
