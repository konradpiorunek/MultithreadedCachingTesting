using System.Collections.Generic;
using System.Collections.Immutable;

namespace CopyOnWrite.Caches
{

    public class CachingUnblockedObtainCopyOnWrite : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        private Dictionary<string, string> _cacheIpToNameToWrite = new Dictionary<string, string>();
        private IReadOnlyDictionary<string, string> _cacheIpToNameToRead = new Dictionary<string, string>();

        public CachingUnblockedObtainCopyOnWrite(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            if (!_cacheIpToNameToRead.TryGetValue(ip, out var result))
            {  
                result = _nsLookup.GetNameFromIpSimple(ip);
                lock(_cacheIpToNameToWrite)
                {
                    _cacheIpToNameToWrite[ip] = result;
                    var copy = _cacheIpToNameToWrite.ToImmutableDictionary(); 
                    _cacheIpToNameToRead = copy;
                }
            }
            return new Response(result);
        }
    }
}
