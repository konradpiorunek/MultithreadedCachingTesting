using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace CopyOnWrite.Caches
{
    public class CachingKeySeparationLockConcurrentDictMeasured : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;
        private readonly ConcurrentDictionary<string, string> _cacheIpToNameToWrite = new ConcurrentDictionary<string, string>();        
        private readonly Dictionary<string, object> _beingDownloaded = new Dictionary<string, object>();
        public CachingKeySeparationLockConcurrentDictMeasured(ISimpleNameResolver nsLookup)
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

            if (!_cacheIpToNameToWrite.TryGetValue(ip, out var result))
            {
                stopwatch.Start();
                object lockObject = null;
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
                    if (!_cacheIpToNameToWrite.TryGetValue(ip, out result))
                    {
                        obtained = true;
                        stopwatch.Start();
                        result = _nsLookup.GetNameFromIpSimple(ip);

                        stopwatch.Start();
                        _cacheIpToNameToWrite[ip] = result;
                        stopwatch.Stop();
                        writeToCacheWaitingTime = stopwatch.ElapsedMilliseconds;
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
