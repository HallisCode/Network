using System.Net;
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
            return new HttpClient(_tcpServer.AcceptTcpClient(), _httpSerializer);
        }

        public async Task<IHttpClient> AcceptConnectionAsync()
        {
            return new HttpClient(await _tcpServer.AcceptTcpClientAsync(), _httpSerializer);
        }

        public void Stop()
        {
            _tcpServer.Stop();
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