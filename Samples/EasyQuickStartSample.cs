using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Network.Core.HTTP;
using Network.Core.HTTP.Serialization.Exceptions;
using Network.Core.HTTP.Types;
using Network.Core.Server;
using Network.Core.Server.Models;
using Network.HTTP.Serialization;
using Network.TCP;
using Network.ThinServer.Logger;
using ThinServer.TCP;
using HttpListener = Network.HTTP.HttpListener;

namespace Main
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8080);

            ILogger logger = new Logger();
            IHttpSerializer httpSerializer = new HttpSerializer();

            // Инициализируем tcp listener
            ITcpListener tcpListener = new TcpListener(endpoint);

            // Инициализируем http listener
            IHttpListener httpListener = new HttpListener(httpSerializer, tcpListener);

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