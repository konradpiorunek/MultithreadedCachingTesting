namespace CopyOnWrite
{
    public struct Response
    {
        public Response(string address) : this(address, false, false, 0)
        {
            ExtendedInformation = false;
        }
        public Response(string address, bool cacheHit, bool obtainedValue, long millisecondsWaiting)
        {            
            Address = address;
            CacheHit = cacheHit;
            ObtainedValue = obtainedValue;
            MillisecondsWaiting = millisecondsWaiting;
            ExtendedInformation = true;
        }

        public string Address { get; }
        public bool CacheHit { get; }
        public bool ObtainedValue { get; }
        public long MillisecondsWaiting { get; }
        public bool ExtendedInformation { get; }
    }
}
