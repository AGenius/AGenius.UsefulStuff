using System;
using System.Net;

namespace AGenius.UsefulStuff.Helpers.TCPServer
{
    public enum TCPEventCode
    {
        Connected,
        Disconnected,
        DataReceived
    }
   
    public class TCPConnectionEventArgs : EventArgs, IDisposable
    {
        public TCPEventCode EventType { get; set; }
        public string ConnectionId { get; set; }
        public string Message { get; set; }
        public string RemoteData { get; set; }
        public TCPConnection ThisConnection { get; set; }
        public void Dispose()
        {
            ((IDisposable)ThisConnection).Dispose();
        }
    }
}