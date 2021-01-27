using System.Collections.Concurrent;

namespace CopyOnWrite.Caches
{
    public class CachingSingleLockParallelDict : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        ConcurrentDictionary<string, string> _cacheIpToName = new ConcurrentDictionary<string, string>();

        public CachingSingleLockParallelDict(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            if (!_cacheIpToName.TryGetValue(ip, out var result))
            {
                lock (_cacheIpToName)
                {
                    _cacheIpToName[ip] = result = _nsLookup.GetNameFromIpSimple(ip);
                }
            }
            return new Response(result);
        }
    }
}
