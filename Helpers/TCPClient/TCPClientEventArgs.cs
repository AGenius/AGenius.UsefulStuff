using System;

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
