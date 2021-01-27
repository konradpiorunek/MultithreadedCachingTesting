namespace CopyOnWrite
{

    public interface INameResolver
    {
        Response GetNameFromIp(string ip);        
    }
}
