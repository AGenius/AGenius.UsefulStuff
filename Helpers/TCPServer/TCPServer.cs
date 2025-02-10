using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff.Helpers.TCPServer
{
    /// <summary>This Class is to allow the listening on a TCP port for incoming data</summary>
    public class TCPServer
    {
        #region Private vars
        private int? _port;
        private bool _running;

        public string ReceivedResponse { get; set; } = "OK..\r\n";
        public string ExitResponse { get; set; } = "BYE..\r\n";
        public string WelcomeMessage { get; set; } = "Hello..";
        public bool WaitForCR { get; set; } = true;


        private TcpListener _listener;

        #endregion  
        /// <summary>
        /// Is server listening
        /// </summary>
        public bool isRunning
        {
            get
            {
                return _running;
            }
            set
            {
                _running = value;
            }
        }
        public event EventHandler<TCPConnectionEventArgs> TCPEvent;

        public TCPConnections ActiveConnections;
        public int? PortNumber => _port;

        public TCPServer(int Port)
        {
            _port = Port;
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port.Value);
        }
        public TCPServer()
        {
            if (!_port.HasValue)
            {
                throw (new ConnectionRegistrationException("Listening port not specified"));
            }
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port.Value);
        }

        public async void StartListening()
        {
            try
            {
                if (_running)
                {
                    return;
                }

                _running = true;
                _listener.Start();
                ActiveConnections = new TCPConnections(this);
                while (_running)
                {
                    try
                    {
                        var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(true);
                        _ = Task.Run(() => new TCPConnection(this, WaitForCR, ReceivedResponse, ExitResponse, WelcomeMessage).Open(client));
                    }
                    catch (Exception)
                    {
                        // Dont care                         
                    }
                }
            }
            catch (SocketException)
            {
                throw;
            }
            catch (ConnectionRegistrationException)
            {
                throw;
            }
        }

        public void StopListening()
        {
            _listener.Stop();
            _running = false;
        }

        public void onTCPEvent(TCPConnectionEventArgs e)
        {
            TCPEvent?.Invoke(this, e);
        }
    }
}
