using System.Collections.Generic;
using System.Collections.Immutable;

namespace CopyOnWrite.Caches
{
    public class CachingKeySeparationLockCopyOnWriteRefactoredOVLO : INameResolver //Obtained Value Lock Optimization 
    {
        private IReadOnlyDictionary<string, string> _cacheIpToNameToRead = new Dictionary<string, string>();
        private readonly ISimpleNameResolver _nsLookup;
        private readonly Dictionary<string, string> _cacheIpToNameToWrite = new Dictionary<string, string>();
        private readonly Dictionary<string, object> _beingDownloaded = new Dictionary<string, object>();
        private IReadOnlyDictionary<string, object> _beingDownloadedReadOnly = new Dictionary<string, object>();

        public CachingKeySeparationLockCopyOnWriteRefactoredOVLO(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }

        public Response GetNameFromIp(string ip)
        {
            if (!TryGetCachedValue(ip, out var result))
            {
                lock (GetLockObjectForKey(ip))
                {
                    if (!TryGetCachedValue(ip, out result))
                    {
                        result = ObtainUncachedValue(ip);
                        CreateNewReadingDictionary(ip, result);
                    }
                }
            }
            return new Response(result);
        }


        private void CreateNewReadingDictionary(string ip, string result)
        {
            lock (_cacheIpToNameToWrite)
            {
                _cacheIpToNameToWrite[ip] = result;
                _cacheIpToNameToRead = _cacheIpToNameToWrite.ToImmutableDictionary(); 
            }
        }

        private string ObtainUncachedValue(string ip)
        {
            return _nsLookup.GetNameFromIpSimple(ip);
        }

        
        private object GetLockObjectForKey(string ip)
        {
            if (!_beingDownloadedReadOnly.TryGetValue(ip, out object lockObject)) 
            {
                lock (_beingDownloaded)
                {
                    if (!_beingDownloaded.TryGetValue(ip, out lockObject)) 
                    {
                        _beingDownloaded[ip] = lockObject = new object();
                        _beingDownloadedReadOnly = _beingDownloaded.ToImmutableDictionary(); 
                    }
                }
            }
            return lockObject;
        }

        private bool TryGetCachedValue(string ip, out string result)
        {
            return _cacheIpToNameToRead.TryGetValue(ip, out result);
        }
    }

}
