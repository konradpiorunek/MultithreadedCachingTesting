using System.Collections.Generic;

namespace CopyOnWrite.Caches
{

    public class CachingNoLockDictCanBreak : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        Dictionary<string, string> _cacheIpToName = new Dictionary<string, string>();

        public CachingNoLockDictCanBreak(ISimpleNameResolver nsLookup)
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
