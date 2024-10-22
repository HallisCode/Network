using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThinServer.HTTP;
using ThinServer.HTTP.Types;
using ThinServer.TCP;
using HttpListener = System.Net.HttpListener;
using HttpStatusCode = ThinServer.HTTP.Types.HttpStatusCode;
using TcpListener = ThinServer.TCP.TcpListener;

namespace Main
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8080);


            IHttpListener listener = new ThinServer.HTTP.HttpListener(endpoint, new HttpSerializer());

            listener.Start();

            while (true)
            {
                IHttpClient connection = listener.AcceptConnection();


                IHttpObject request = connection.GetHttp();

                IHttpObject response = HttpObject.CreateResponse(
                    HttpProtocol.Http1_1,
                    new HttpStatusCode(200),
                    "OK"
                );

                connection.SendHttp(response);

                connection.Close();
            }
        }
    }
}