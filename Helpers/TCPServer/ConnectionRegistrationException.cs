using System;

namespace AGenius.UsefulStuff.Helpers.TCPServer
{
    public class ConnectionRegistrationException : Exception
    {
        public ConnectionRegistrationException(string message) : base(message)
        {
        }
    }
}
