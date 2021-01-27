using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CopyOnWrite.Caches
{
    public class CachingKeySeparationLockCopyOnWriteMeasured : INameResolver
    {
        private IReadOnlyDictionary<string, string> _cacheIpToNameToRead = new Dictionary<string, string>();
        private readonly ISimpleNameResolver _nsLookup;
        private readonly Dictionary<string, string> _cacheIpToNameToWrite = new Dictionary<string, string>();        
        private readonly Dictionary<string, object> _beingDownloaded = new Dictionary<string, object>();
        public CachingKeySeparationLockCopyOnWriteMeasured(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }
        public Response GetNameFromIp(string ip)
        {
            var stopwatch = new Stopwatch();
            var cacheHit = false;
            var obtained = false;
            long initialWaitingTime = 0;
            long sameKeyWaitingTime = 0;
            long writeToCacheWaitingTime = 0;

            if (!_cacheIpToNameToRead.TryGetValue(ip, out var result))
            {
                object lockObject = null;
                stopwatch.Start();
                lock (_beingDownloaded)
                {
                    stopwatch.Stop();
                    initialWaitingTime = stopwatch.ElapsedMilliseconds;
                    if (!_beingDownloaded.TryGetValue(ip, out lockObject))
                    {   
                        _beingDownloaded[ip] = lockObject = new object();
                    }
                }
                
                stopwatch.Start();
                lock (lockObject)
                {
                    stopwatch.Stop();
                    sameKeyWaitingTime = stopwatch.ElapsedMilliseconds;
                    if (!_cacheIpToNameToRead.TryGetValue(ip, out result))
                    {
                        obtained = true;
                        result = _nsLookup.GetNameFromIpSimple(ip);
                        stopwatch.Start();
                        lock (_cacheIpToNameToWrite)
                        {
                            stopwatch.Stop();
                            writeToCacheWaitingTime = stopwatch.ElapsedMilliseconds;
                            _cacheIpToNameToWrite[ip] = result;
                            _cacheIpToNameToRead = _cacheIpToNameToWrite.ToImmutableDictionary();
                        }
                    }
                    else
                    {
                        cacheHit = true;
                    }                    
                }
            }
            else
            {
                cacheHit = true;
            }

            return new Response(result, cacheHit, obtained, initialWaitingTime + sameKeyWaitingTime + writeToCacheWaitingTime);
        }
    }


}
