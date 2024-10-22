using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThinServer.HTTP;
using ThinServer.TCP;
using HttpListener = System.Net.HttpListener;
using TcpListener = ThinServer.TCP.TcpListener;

namespace Main
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {

        }

        public static async Task Handle(NetworkStream stream)
        {
            byte[] data = new byte[1024 * 16]; // 16KB

            while (true)
            {
                if (stream.DataAvailable is false) continue;


                await stream.ReadAsync(data, 0, data.Length);


                IHttpSerializer httpSerializer = new HttpSerializer();

                IHttpObject httpObject = httpSerializer.ToObject(data);
                Array.Clear(data);

                string http = httpSerializer.ToHttp(httpObject);

                await stream.WriteAsync(Encoding.UTF8.GetBytes(http));
            }
        }
    }
}