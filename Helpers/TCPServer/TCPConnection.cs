using System;
using System.Net.Sockets;
using System.Text;

namespace AGenius.UsefulStuff.Helpers.TCPServer
{
    public class TCPConnection : IDisposable
    {
        private TCPServer thisServer;
        private TcpClient thisClient;
        private readonly byte[] buffer;
        private NetworkStream stream;
        public bool isOpen;
        private bool disposed;

        public readonly string Id;
        public string endPoint;
        public string ReceivedResponse { get; set; } = "OK..";
        public string ExitResponse { get; set; } = "BYE..";
        public string WelcomeMessage { get; set; } = "Hello..";
        public bool WaitForCR { get; set; } = true;
        public TCPConnection(TCPServer myServer, bool waitForCR, string receivedResponse, string exitResponse, string welcomeMessage)
        {
            thisServer = myServer;
            buffer = new byte[256];
            ReceivedResponse = receivedResponse;
            ExitResponse = exitResponse;
            WaitForCR = waitForCR;
            WelcomeMessage = welcomeMessage;
            Id = Guid.NewGuid().ToString();
        }
        public string DisplayName
        {
            get
            {
                return endPoint;
            }
        }
        public void Open(TcpClient client)
        {
            thisClient = client;
            isOpen = true;
            try
            {
                thisServer.ActiveConnections.OpenConnections.Add(this);
            }
            catch (Exception)
            {
                isOpen = false;
                throw (new ConnectionRegistrationException("Unable to add connection to list"));
            }

            //endPoint = $"{(thisClient.Client.RemoteEndPoint.};
            endPoint = $"{((System.Net.IPEndPoint)thisClient.Client.RemoteEndPoint).Address}:{((System.Net.IPEndPoint)thisClient.Client.RemoteEndPoint).Port}";
            var acceptedArgs = new TCPConnectionEventArgs()
            {
                EventType = TCPEventCode.Connected,
                ConnectionId = Id,
                RemoteData = endPoint,
                ThisConnection = this
            };

            thisServer.onTCPEvent(acceptedArgs);

            string data = "";
            using (stream = thisClient.GetStream())
            {
                int position;
                Send($"{WelcomeMessage}\r\n");
                while (isOpen)
                {
                    if (clientDisconnected())
                    {
                        Disconnect();
                    }
                    else
                    {
                        try
                        {
                            // Read 1 character at a time
                            while (isOpen && (position = stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                string dataIn = Encoding.UTF8.GetString(buffer, 0, position);
                                data += dataIn;
                                if (dataIn == "\u0003")
                                {
                                    if (!string.IsNullOrEmpty(ExitResponse))
                                    {
                                        Send($"\r\n{ExitResponse}");
                                    }
                                    // ctrl+c detected
                                    Disconnect();
                                }
                                if ((WaitForCR && dataIn.Contains("\n")) || !WaitForCR)
                                {
                                    // Raise the data received event and pass the data 
                                    var receivedArgs = new TCPConnectionEventArgs()
                                    {
                                        EventType = TCPEventCode.DataReceived,
                                        Message = data.Replace(Environment.NewLine, ""),
                                        ConnectionId = Id,
                                        ThisConnection = this,
                                        RemoteData = endPoint
                                    };

                                    thisServer.onTCPEvent(receivedArgs);
                                    data = "";// Empty buffer
                                              // Send response 
                                    if (!string.IsNullOrEmpty(ReceivedResponse))
                                    {
                                        Send($"\r\n{ReceivedResponse}");
                                    }
                                }
                                //if (dataIn.Contains("\n")) // wait until a cr received
                                //{

                                //}
                                if (!isOpen) { break; }
                            }
                        }
                        catch (Exception ex)
                        {
                            //if (ex is not IOException)
                            //{
                            //    throw new Exception(ex.Message);
                            //}
                        }
                    }
                }
            }
        }
        public void Disconnect()
        {
            // Raise the Disconnected event
            var receivedArgs = new TCPConnectionEventArgs()
            {
                EventType = TCPEventCode.Disconnected,
                ConnectionId = Id,
                ThisConnection = this,
                RemoteData= endPoint
            };
            Disconnected();
            thisServer.onTCPEvent(receivedArgs);

        }
        public void Send(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        void Disconnected()
        {
            Dispose(false);
            isOpen = false;
            // Remove the client from the collection
            thisServer.ActiveConnections.OpenConnections.TryTake(out TCPConnection removedChannel);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                stream.Close();
                thisClient.Close();
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool clientDisconnected()
        {
            return (thisClient.Client.Available == 0 && thisClient.Client.Poll(1, SelectMode.SelectRead));
        }
    }
}