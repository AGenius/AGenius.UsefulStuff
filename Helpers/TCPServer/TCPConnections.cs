using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

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
