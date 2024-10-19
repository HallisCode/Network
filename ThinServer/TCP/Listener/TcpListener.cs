using System.Net;
using System.Net.Sockets;

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
            get => (_active) ? (IPEndPoint)_serverSocket.LocalEndPoint : _localEndpoint;
        }

        public Socket Server { get; }

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

            InitializeServerSocket();
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

        /// <summary>
        /// Принимает входящее соединение.
        /// </summary>
        /// <returns></returns>
        public Socket AcceptSocket()
        {
            VerifyActiveOnState();

            return _serverSocket.Accept();
        }

        /// <summary>
        /// Принимает входящее соединение.
        /// </summary>
        /// <returns></returns>
        public async Task<Socket> AcceptSocketAsync()
        {
            VerifyActiveOnState();

            return await _serverSocket.AcceptAsync();
        }

        public ITcpClient AcceptTcpClient()
        {
            VerifyActiveOnState();

            return new TcpClient(AcceptSocket());
        }

        public async Task<ITcpClient> AcceptTcpClientAsync()
        {
            VerifyActiveOnState();

            return new TcpClient(await AcceptSocketAsync());
        }

        public void Close()
        {
            Dispose();
        }


        private void VerifyActiveOnState()
        {
            if (_active == false)
            {
                throw new TcpListenerException($"Server isn't active to do given action.");
            }
        }


        private void InitializeServerSocket()
        {
            if (_localEndpoint is null)
                throw new TcpListenerException("LocalEndpoint not defined.");

            _serverSocket = new Socket(_localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }


        ~TcpListener()
        {
            this.Dispose(false);
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

                _active = false;
            }

            _disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }
    }

    public class TcpListenerException : Exception
    {
        public TcpListenerException()
        {
        }

        public TcpListenerException(string message) : base(message)
        {
        }

        public TcpListenerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}