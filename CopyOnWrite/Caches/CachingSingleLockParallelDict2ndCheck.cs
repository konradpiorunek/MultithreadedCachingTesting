using System.Collections.Generic;

namespace CopyOnWrite.Caches
{

    public class CachingSingleLockParallelDict2ndCheck : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        Dictionary<string, string> _cacheIpToName = new Dictionary<string, string>();

        public CachingSingleLockParallelDict2ndCheck(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            if (!_cacheIpToName.TryGetValue(ip, out var result))
            {
                lock (_cacheIpToName)
                {
                    if (!_cacheIpToName.TryGetValue(ip, out result))
                    {
                        _cacheIpToName[ip] = result = _nsLookup.GetNameFromIpSimple(ip);
                    }
                }
            }
            return new Response(result);
        }
    }

}
