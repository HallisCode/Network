using System.Net;
using ThinServer.HTTP.Exceptions;
using ThinServer.TCP;

namespace ThinServer.HTTP
{
    public class HttpListener : IHttpListener
    {
        private IHttpSerializer _httpSerializer;
        private IPEndPoint? _localEndpoint;
        private ITcpListener _tcpServer;

        public IPEndPoint LocalEndpoint
        {
            get => _localEndpoint ?? _tcpServer.LocalEndpoint;
        }

        public bool Active
        {
            get => _tcpServer.Active;
        }

        private bool _disposed;


        public HttpListener(IPEndPoint endpoint, IHttpSerializer serializer)
        {
            _localEndpoint = endpoint;
            _httpSerializer = serializer;

            _InitializeTcpServer();
        }


        public void Start()
        {
            _tcpServer.Start();
        }

        public IHttpClient AcceptConnection()
        {
            _VerifyActiveState();

            return new HttpClient(_tcpServer.AcceptTcpClient(), _httpSerializer);
        }

        public async Task<IHttpClient> AcceptConnectionAsync()
        {
            _VerifyActiveState();

            return new HttpClient(await _tcpServer.AcceptTcpClientAsync(), _httpSerializer);
        }

        public void Stop()
        {
            _tcpServer.Stop();
        }
        
        public async Task StopAsync()
        {
            _tcpServer.StopAsync();
        }

        private void _VerifyActiveState()
        {
            if (Active is false)
            {
                throw new ServerNotActive($"Server isn't active.");
            }
        }

        private void _InitializeTcpServer()
        {
            _tcpServer = new TcpListener(_localEndpoint);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _tcpServer.Dispose();
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

        ~HttpListener()
        {
            this.Dispose(false);
        }
    }
}