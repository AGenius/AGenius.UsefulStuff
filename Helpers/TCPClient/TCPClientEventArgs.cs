using AGenius.UsefulStuff.Helpers.TCPServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff.Helpers.TCPClient
{
    /// <summary>
    /// Potential states for a TCP Client connection 
    /// </summary>
    /// <remarks>Not all currently implemented</remarks>
    public enum eTCPClientState
    {
        NotInitialized,
        NotReady,
        Idle,
        Connecting,
        Connected,
        Disconnected,
        ConnectionFailed,
        ErrorOccured,
        MessageReceived
    }

    public class TCPClientEventArgs : EventArgs
    {
        public eTCPClientState EventType { get; set; }
        public string Message { get; set; }
        public TCPClient Client { get; set; }

    }
}
