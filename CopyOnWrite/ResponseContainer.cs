using System;

namespace CopyOnWrite
{
    public class ResponseContainer
    {
        public TimeSpan RunTime { get; set; }
        public Response Result { get; set; }
    }
}
