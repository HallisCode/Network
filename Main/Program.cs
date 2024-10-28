using System;
using System.Net;
using System.Threading.Tasks;
using Server.Logger;
using ThinServer;
using ThinServer.HTTP;
using ThinServer.HTTP.Types;
using HttpStatusCode = ThinServer.HTTP.Types.HttpStatusCode;


namespace Main
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8080);

            ILogger logger = new Logger();
            IHttpSerializer serializer = new HttpSerializer();

            IServer server = new ThinServer.ThinServer(endpoint, serializer, logger);
            server.SetHandler(SimpleHandler);

            server.Start();
        }

        public static Task SimpleHandler(IServerHttpRequest request)
        {
            IHttpSerializer serializer = new HttpSerializer();
            Console.WriteLine($"\n{serializer.ToHttp(request.Request)}");

            IHttpObject response = HttpObject.CreateResponse(HttpProtocol.Http1_1, HttpStatusCode.OK, "OK");
            request.Response = response;

            return Task.CompletedTask;
        }
    }
}