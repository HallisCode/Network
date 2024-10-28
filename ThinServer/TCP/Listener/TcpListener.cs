using System.Net;
using System.Net.Sockets;
using ThinServer.TCP.Exceptions;

namespace ThinServer.TCP
{
    public class TcpListener : ITcpListener
    {
        // Private properties
        private Socket? _serverSocket;
        private IPEndPoint? _localEndpoint;
        private bool _active;
        private bool _disposed;

        // Public properties
        public IPEndPoint LocalEndpoint
        {
            get => _localEndpoint ?? (IPEndPoint)_serverSocket.LocalEndPoint;
        }

        public bool Active
        {
            get => _active;
        }


        /// <summary>
        /// Инициализирует нового слушателя TCP.
        /// </summary>
        /// <param name="endPoint">Адресс который будет прослушиваться.</param>
        public TcpListener(IPEndPoint endPoint)
        {
            _localEndpoint = endPoint;

            _InitializeServerSocket();
        }

        /// <summary>
        /// Запускает прослушивание.
        /// </summary>
        public void Start()
        {
            _serverSocket.Bind(_localEndpoint);

            _serverSocket.Listen();

            _active = true;
        }

        /// <summary>
        /// Остановливает прослушивание.
        /// </summary>
        public void Stop()
        {
            _serverSocket.Disconnect(true);

            _active = false;
        }

        public ITcpClient AcceptTcpClient()
        {
            _VerifyActiveState();

            return new TcpClient(_serverSocket.Accept());
        }

        public async Task<ITcpClient> AcceptTcpClientAsync()
        {
            _VerifyActiveState();

            return new TcpClient(await _serverSocket.AcceptAsync());
        }

        private void _VerifyActiveState()
        {
            if (_active is false)
            {
                throw new ServerNotActive($"Server isn't active.");
            }
        }

        private void _InitializeServerSocket()
        {
            _serverSocket = new Socket(_localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _serverSocket.Dispose();
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
            Dispose();
        }

        ~TcpListener()
        {
            this.Dispose(false);
        }
    }
}