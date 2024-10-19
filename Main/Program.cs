using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThinServer.HTTP;
using ThinServer.TCP;
using TcpListener = ThinServer.TCP.TcpListener;

namespace Main
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            //
            // IPEndPoint endPoint = new IPEndPoint(ipAddress, 8080);
            //
            // TimeSpan awaitNewData = new TimeSpan(0, 0, 0, 0, 256);
            //
            //
            // TcpListener listener = new TcpListener(endPoint);
            // listener.Start();
            //
            // while (true)
            // {
            //     ITcpClient client = await listener.AcceptTcpClientAsync();
            //     NetworkStream stream = client.GetStream();
            //
            //     Task.Run(async () =>
            //     {
            //         try
            //         {
            //             await Handle(stream);
            //         }
            //         catch (Exception e)
            //         {
            //             Console.WriteLine(e);
            //         }
            //         finally
            //         {
            //             stream.Close();
            //             client.Close();
            //         }
            //     });
            // }

            string text = "";

            IHttpSerializer httpSerializer = new HttpSerializer();

            httpSerializer.ToObject(Encoding.UTF8.GetBytes(text));
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