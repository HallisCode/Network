using System.Net;
using System.Net.Sockets;

namespace ThinServer.TCP
{
    public class TcpClient : ITcpClient
    {
        // Private properties
        private IPEndPoint? _localEndpoint;
        private IPEndPoint? _RemoteEndpoint;
        private Socket? _client;
        private bool _disposed;


        // Public properties
        public bool Connected
        {
            get => _client != null && _client.Connected;
        }

        public int Available
        {
            get => _client.Available;
        }

        public IPEndPoint? LocalEndpoint
        {
            get => _localEndpoint ?? _client.LocalEndPoint as IPEndPoint;
        }

        public IPEndPoint? RemoteEndPoint
        {
            get => _RemoteEndpoint ?? _client.RemoteEndPoint as IPEndPoint;
        }

        public int ReceiveBufferSize
        {
            get => _client.ReceiveBufferSize;
            set { _client.ReceiveBufferSize = value; }
        }

        public int SendBufferSize
        {
            get => _client.SendBufferSize;
            set { _client.SendBufferSize = value; }
        }


        public TcpClient(Socket socket)
        {
            _client = socket;
        }

        public TcpClient(IPEndPoint endPoint)
        {
            _localEndpoint = endPoint;

            InitializeClientSocket();
        }


        public void Connect(string hostname, int port)
        {
            _client.Connect(hostname, port);
        }

        public async Task ConnectAsync(string hostname, int port)
        {
            await _client.ConnectAsync(hostname, port);
        }

        public void Stop()
        {
            _client.Disconnect(true);
        }

        public int Receive(byte[] buffer)
        {
            _VerifyActiveConnected();

            return _client.Receive(buffer);
        }

        public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags = SocketFlags.None)
        {
            _VerifyActiveConnected();

            return _client.Receive(buffer, offset, size, socketFlags);
        }

        public async Task<int> ReceiveAsync(byte[] buffer)
        {
            _VerifyActiveConnected();

            return await _client.ReceiveAsync(buffer);
        }

        public NetworkStream GetStream()
        {
            _VerifyActiveConnected();

            return new NetworkStream(this._client);
        }

        private void _VerifyActiveConnected()
        {
            if (Connected is false)
            {
                throw new Exception("The connection was not established");
            }
        }

        private void InitializeClientSocket()
        {
            if (_localEndpoint is null)
            {
                throw new TcpClientException("LocalEndpoint not defined.");
            }

            _client = new Socket(_localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _client.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            _client.Close();
        }

        ~TcpClient()
        {
            this.Dispose(false);
        }
    }


    public class TcpClientException : Exception
    {
        public TcpClientException()
        {
        }

        public TcpClientException(string message) : base(message)
        {
        }

        public TcpClientException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}