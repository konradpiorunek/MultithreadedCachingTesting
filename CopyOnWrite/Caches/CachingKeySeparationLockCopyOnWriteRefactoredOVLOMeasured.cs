using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CopyOnWrite.Caches
{
    public class CachingKeySeparationLockCopyOnWriteRefactoredOVLOMeasured : INameResolver 
    {
        private IReadOnlyDictionary<string, string> _cacheIpToNameToRead = new Dictionary<string, string>();
        private readonly ISimpleNameResolver _nsLookup;
        private readonly Dictionary<string, string> _cacheIpToNameToWrite = new Dictionary<string, string>();
        private readonly Dictionary<string, object> _beingDownloaded = new Dictionary<string, object>();
        private IReadOnlyDictionary<string, object> _beingDownloadedReadOnly = new Dictionary<string, object>();

        public CachingKeySeparationLockCopyOnWriteRefactoredOVLOMeasured(ISimpleNameResolver nsLookup)
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

            if (!TryGetCachedValue(ip, out var result))
            {
                if (!_beingDownloadedReadOnly.TryGetValue(ip, out object lockObject)) 
                {
                    stopwatch.Start();
                    lock (_beingDownloaded)
                    {
                        stopwatch.Stop();
                        initialWaitingTime = stopwatch.ElapsedMilliseconds;
                        if (!_beingDownloaded.TryGetValue(ip, out lockObject)) 
                        {
                            _beingDownloaded[ip] = lockObject = new object();
                            _beingDownloadedReadOnly = _beingDownloaded.ToImmutableDictionary(); 
                        }
                    }
                }
                stopwatch.Start();
                lock (lockObject)
                {
                    stopwatch.Stop();
                    sameKeyWaitingTime = stopwatch.ElapsedMilliseconds;
                    if (!TryGetCachedValue(ip, out result))
                    {
                        obtained = true;
                        result = ObtainUncachedValue(ip);
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

   

        private string ObtainUncachedValue(string ip)
        {
            return _nsLookup.GetNameFromIpSimple(ip);
        }



        private bool TryGetCachedValue(string ip, out string result)
        {
            return _cacheIpToNameToRead.TryGetValue(ip, out result);
        }
    }

}
