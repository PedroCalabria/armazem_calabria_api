using System.Collections;
using System.Net;

namespace ArmazemCalabria.CrossCutting
{
    public interface ISourceInfo
    {
        Hashtable Data { get; set; }

        IPAddress IP { get; set; }
    }

    public class SourceInfo : ISourceInfo
    {
        public Hashtable Data { get; set; }
        public IPAddress IP { get; set; }
    }
}