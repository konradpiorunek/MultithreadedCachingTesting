namespace CopyOnWrite.Caches
{
    public class NoCachingDecorator : INameResolver
    {
        private readonly ISimpleNameResolver _nsLookup;

        public NoCachingDecorator(ISimpleNameResolver nsLookup)
        {
            _nsLookup = nsLookup;
        }

        public Response GetNameFromIp(string ip)
        {
            return new Response(_nsLookup.GetNameFromIpSimple(ip));
        }
    }
}
