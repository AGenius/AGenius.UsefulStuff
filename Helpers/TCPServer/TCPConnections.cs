using System.Collections.Concurrent;

namespace AGenius.UsefulStuff.Helpers.TCPServer
{
    public class TCPConnections
    {
        public ConcurrentBag<TCPConnection> OpenConnections;
        private readonly TCPServer thisServer;

        public TCPConnections(TCPServer myServer)
        {
            OpenConnections = new ConcurrentBag<TCPConnection>();
            thisServer = myServer;
        }
    }
}
