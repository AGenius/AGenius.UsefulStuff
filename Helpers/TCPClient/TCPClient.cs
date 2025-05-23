using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace AGenius.UsefulStuff.Helpers.TCPClient
{
    /// <summary>
    /// Simple TCPClient class for communicating to a server over TCP
    /// </summary>
    public class TCPClient
    {
        protected private TcpClient _client;
        /// <summary>
        /// Event to monitor TCP Activity
        /// </summary>
        /// <remarks>Subscribe to this event for updates and access to received messages </remarks>
        public event EventHandler<TCPClientEventArgs> ClientEvent; // Event for notifications

        protected private eTCPClientState _state = eTCPClientState.NotInitialized; // Set initial state

        /// <summary>
        /// State enum
        /// </summary>
        public eTCPClientState State
        {
            get
            {
                return _state;
            }
        }
        /// <summary>
        /// Last Error if any
        /// </summary>
        public string LastError { get; private set; }
        /// <summary>
        /// Connect to the TCP server
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public void Connect(string ipAddress, int port)
        {
            OnEvent(eTCPClientState.Connecting, "Connecting", true);
            try
            {
                _client = new TcpClient();
                _client.Connect(ipAddress, port);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                OnEvent(eTCPClientState.ConnectionFailed, $"Connection to {ipAddress} port {port.ToString()} - Failed", true);
                throw new Exception(ex.Message);
            }
            var endPoint = _client.Client.RemoteEndPoint as System.Net.IPEndPoint;
            var ip4Add = endPoint.Address.ToString().Split(':').Last();

            OnEvent(eTCPClientState.Connected, $"Connection to {ipAddress} ({ip4Add}) port {port.ToString()} - Successful", true);
            _ = ReceiveDataAsync(); // Start a separate task to receive data
        }
        /// <summary>
        /// Send message to the connected server
        /// </summary>
        /// <param name="message"></param>
        public bool SendMessage(string message, bool appendCR = true)
        {
            try
            {
                if (_client.Client.Connected)
                {
                    NetworkStream stream = _client.GetStream();
                    byte[] buffer;
                    if (appendCR)
                    {
                        buffer = Encoding.ASCII.GetBytes(message + '\n');
                    }
                    else
                    {
                        buffer = Encoding.ASCII.GetBytes(message);
                    }

                    stream.Write(buffer, 0, buffer.Length);
                    return true;
                }
                else
                {
                    OnEvent(eTCPClientState.Disconnected, "Connection to host lost", true);
                }
                return false;
            }
            catch (Exception ex)
            {
                LastError += ex.Message;
                OnEvent(eTCPClientState.ErrorOccured, $"Error : {ex.Message}", true);
                return false;
            }
        }
        /// <summary>
        /// Handle incoming data from the server
        /// </summary>
        private async Task ReceiveDataAsync()
        {
            NetworkStream stream = _client.GetStream();
            byte[] buffer = new byte[1024];
            StringBuilder messageBuilder = new StringBuilder();

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                if (bytesRead == 0)
                {
                    OnEvent(eTCPClientState.Disconnected, "Connection to host lost", true);
                    break; // Server or client disconnected, do nothing as no data
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messageBuilder.Append(data);

                if (!stream.DataAvailable)
                {
                    // End of data received
                    string receivedMessage = messageBuilder.ToString();
                    OnEvent(eTCPClientState.MessageReceived, receivedMessage, false);
                    //OnMessageReceived(receivedMessage);
                    messageBuilder.Clear();
                }
            }

            _client.Close();
            OnEvent(eTCPClientState.Disconnected, "Client Disconnected", true);
        }
        /// <summary>
        /// Disconnect the client
        /// </summary>
        public void Disconnect()
        {
            _client?.Close();
            OnEvent(eTCPClientState.Disconnected, "Client Disconnected", true);
        }
        // Raise the activity event
        protected virtual void OnEvent(eTCPClientState state, string message, bool setLocalState = false)
        {
            if (setLocalState)
                _state = state;

            var receivedArgs = new TCPClientEventArgs()
            {
                EventType = state,
                Message = message,
                Client = this,
            };

            ClientEvent?.Invoke(this, receivedArgs);
        }
    }
}
