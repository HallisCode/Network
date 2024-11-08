using System.Net;
using Microsoft.Extensions.Logging;
using Network.Core.HTTP;
using Network.Core.HTTP.Types;
using Network.Core.Server;
using Network.Core.Server.Models;
using Network.HTTP.Serialization;
using Network.ThinServer.Models;

namespace Network.ThinServer
{
    public class ThinServer : IServer
    {
        // DI elements
        private ILogger<ThinServer> _logger;

        // Logic of statements and settings
        private bool _disposed;
        private Func<IServerHttpRequest, Task> _handler;

        private Dictionary<string, string> _defaultHeaders = new Dictionary<string, string>()
        {
            { "Connection", "close" }
        }; // default headers

        // Logic of listening
        private IHttpListener _listener;
        private Task _mainLoop;
        private IList<Task> _runningHandlers = new List<Task>();
        private CancellationTokenSource _tokenServer = new CancellationTokenSource();

        // Public properties
        public int TimeOutMilleSeconds { get; set; } = 16000;

        public bool Active
        {
            get => _listener.Active;
        }

        public IPEndPoint EndPoint
        {
            get => _listener.LocalEndpoint;
        }


        public ThinServer(IHttpListener listener, ILogger<ThinServer> logger)
        {
            _listener = listener;
            _logger = logger;
        }

        public void SetHandler(Func<IServerHttpRequest, Task> handler)
        {
            _handler = handler;
        }

        public void Start()
        {
            _StartAsync();

            Task.WaitAny(_mainLoop);
        }

        public async Task StartAsync()
        {
            await _StartAsync();
        }

        public void Stop()
        {
            _tokenServer.Cancel();

            Task.WaitAll(_runningHandlers.ToArray());

            _listener.Stop();
        }

        public async Task StopAsync()
        {
            _tokenServer.Cancel();

            await Task.WhenAll(_runningHandlers);

            await _listener.StopAsync();
        }

        private Task _StartAsync()
        {
            _listener.Start();

            _InformStartUpInformation();

            return _RunMainLoop();
        }

        private Task _RunMainLoop()
        {
            if (_listener.Active is false) return Task.CompletedTask;

            _mainLoop = Task.Run(async () =>
            {
                // Бесконечно прослушиваем входящие запросы
                while (!_tokenServer.IsCancellationRequested)
                {
                    IHttpClient connection = await _listener.AcceptConnectionAsync();

                    // Запускаем обработчик текущего соединения.
                    Task handler = _RunConnectionHandler(connection, _tokenServer.Token);
                }

                return Task.CompletedTask;
            });

            return _mainLoop;
        }

        private async Task _RunConnectionHandler(IHttpClient connection, CancellationToken token)
        {
            // Запускаем оработку соединения
            Task handler = Task.Run(async () =>
            {
                bool isRequiredCloseConnection = false;

                try
                {
                    while (!token.IsCancellationRequested && !isRequiredCloseConnection)
                    {
                        IHttpObject request = await connection.GetHttpAsync(token);
                        IServerHttpRequest serverHttpRequest = new ServerHttpRequest(request);


                        // Вызываем определенный извне обработчик
                        if (_handler is not null)
                        {
                            await _handler?.Invoke(serverHttpRequest);
                        }


                        IHttpObject response = null;
                        if (serverHttpRequest.Response is not null)
                        {
                            response = serverHttpRequest.Response;
                        }
                        else
                        {
                            response = HttpObject.CreateResponse(
                                protocol: HttpProtocol.Http1_1,
                                code: HttpStatusCode.NoContent,
                                message: "OK",
                                headers: _defaultHeaders
                            );
                        }

                        bool isRequestHasConnectionHeader = _TryGetValueHeader(
                            request, "Connection", out string? requestConnectionStatus
                        );
                        bool isResponseHasConnectionHeader = _TryGetValueHeader(
                            response, "Connection", out string? responseConnectionStatus
                        );

                        if ((isRequestHasConnectionHeader && requestConnectionStatus.ToLower() == "close") ||
                            (isResponseHasConnectionHeader && responseConnectionStatus.ToLower() == "close"))
                        {
                            isRequiredCloseConnection = true;
                        }

                        // Отсылаем ответ
                        await connection.SendHttpAsync(response);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.ToString());

                    IHttpObject response = HttpObject.CreateResponse(
                        protocol: HttpProtocol.Http1_1,
                        code: HttpStatusCode.InternalServerError,
                        message: "Internal Server Error",
                        headers: _defaultHeaders
                    );

                    await connection.SendHttpAsync(response);

                    return Task.FromException(exception);
                }
                finally
                {
                    connection.Close();
                }


                return Task.CompletedTask;
            });

            // Добавляем обработку запроса в список запущенных обработок
            _runningHandlers.Add(handler);

            // Удаляем обработку запроса из запущенных обработок
            await handler;
            _runningHandlers.Remove(handler);

            // Закрываем теукщее соединение
            connection.Close();
        }

        private bool _TryGetValueHeader(IHttpObject httpObject, string headerName, out string? value)
        {
            value = null;

            if (httpObject.Headers is null) return false;


            string? _headerName = httpObject.Headers.Keys.FirstOrDefault(
                key => key.ToLower() == headerName.ToLower(), null
            );

            if (_headerName is not null)
            {
                value = httpObject.Headers[_headerName];

                return true;
            }

            return false;
        }

        private void _InformStartUpInformation()
        {
            if (_listener.Active is false) return;

            _logger.LogInformation("The server has been started." +
                                   $"\nHe's listening to the address: {EndPoint}"
            );
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _listener.Close();
                _tokenServer.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            Dispose();
        }

        ~ThinServer()
        {
            Dispose(false);
        }
    }
}