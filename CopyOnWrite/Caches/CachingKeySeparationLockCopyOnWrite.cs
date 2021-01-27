using System.Collections.Generic;
using System.Collections.Immutable;

namespace CopyOnWrite.Caches
{
    public class CachingKeySeparationLockCopyOnWrite : INameResolver
    {
        private IReadOnlyDictionary<string, string> _cacheIpToNameToRead = new Dictionary<string, string>();
        private readonly ISimpleNameResolver _nsLookup;
        private readonly Dictionary<string, string> _cacheIpToNameToWrite = new Dictionary<string, string>();        
        private readonly Dictionary<string, object> _beingDownloaded = new Dictionary<string, object>();
        public CachingKeySeparationLockCopyOnWrite(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            if (!_cacheIpToNameToRead.TryGetValue(ip, out var result))
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
                    if(!_cacheIpToNameToRead.TryGetValue(ip, out result))
                    {
                        result = _nsLookup.GetNameFromIpSimple(ip);
                        lock (_cacheIpToNameToWrite)
                        {
                            _cacheIpToNameToWrite[ip] = result;                            
                            _cacheIpToNameToRead = _cacheIpToNameToWrite.ToImmutableDictionary();
                        }
                    }                        
                }
                
            }
            return new Response(result);
        }
    }

}
