using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CopyOnWrite.Caches
{
    public class CachingKeySeparationLockConcurrentDict : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        private readonly ConcurrentDictionary<string, string> _cacheIpToNameToWrite = new ConcurrentDictionary<string, string>();
        private readonly Dictionary<string, object> _beingDownloaded = new Dictionary<string, object>();
        public CachingKeySeparationLockConcurrentDict(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            if (!_cacheIpToNameToWrite.TryGetValue(ip, out var result))
            {
                object lockObject = null;
                lock (_beingDownloaded)
                {
                    if (!_beingDownloaded.TryGetValue(ip, out lockObject))
                    {
                        _beingDownloaded[ip] = lockObject = new object();
                    }
                }

                lock (lockObject)
                {
                    if (!_cacheIpToNameToWrite.TryGetValue(ip, out result))
                    {
                        _cacheIpToNameToWrite[ip] = result = _nsLookup.GetNameFromIpSimple(ip);
                    }
                }
            }
            return new Response(result);
        }
    }

}
