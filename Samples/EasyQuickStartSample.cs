using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Network.Core.HTTP;
using Network.Core.HTTP.Serialization.Exceptions;
using Network.Core.HTTP.Types;
using Network.Core.Server;
using Network.Core.Server.Models;
using Network.HTTP.Serialization;
using Network.TCP;
using ThinServer.TCP;
using HttpListener = Network.HTTP.HttpListener;

namespace Main
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Инициализируем tcp listener
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8888);
            ITcpListener tcpListener = new TcpListener(endpoint);

            // Инициализируем http listener
            IHttpSerializer httpSerializer = new HttpSerializer();
            IHttpListener httpListener = new HttpListener(httpSerializer, tcpListener);
            
            // Инициализируем сервер
            ILoggerFactory loggerFactory = LoggerFactory.Create(config => config.AddConsole());
            ILogger logger = loggerFactory.CreateLogger<Network.ThinServer.ThinServer>();
            
            IServer server = new Network.ThinServer.ThinServer(httpListener, logger);
            server.SetHandler(SimpleHandler);

            server.Start();
        }

        public static Task SimpleHandler(IServerHttpRequest request)
        {
            Dictionary<string, string> defaultHeaders = new Dictionary<string, string>();
            defaultHeaders.Add("Connection", "close");

            IHttpObject response = HttpObject.CreateResponse(
                HttpProtocol.Http1_1,
                HttpStatusCode.OK,
                "OK",
                defaultHeaders
            );
            request.Response = response;

            return Task.CompletedTask;
        }
    }
}