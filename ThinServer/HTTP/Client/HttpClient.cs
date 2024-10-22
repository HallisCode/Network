using System.Net.Sockets;
using System.Text;
using ThinServer.TCP;

namespace ThinServer.HTTP;

public class HttpClient : IHttpClient
{
    public NetworkStream Stream
    {
        get => _tcpConnection.Stream;
    }

    private IHttpSerializer _httpSerializer;
    private ITcpClient _tcpConnection;
    private bool _disposed;


    public HttpClient(ITcpClient connection, IHttpSerializer serializer)
    {
        _tcpConnection = connection;
        _httpSerializer = serializer;
    }

    public IHttpObject GetHttp()
    {
        byte[] incomingData = new byte[1024 * 16]; //16KB

        int index = 0;
        while (Stream.DataAvailable)
        {
            int readBytes = Stream.Read(incomingData, index, incomingData.Length - index);

            if (readBytes == 0)
            {
                // TODO overhead buffer for reading
            }

            index += readBytes;
        }

        return _httpSerializer.ToObject(incomingData);
    }

    public async Task<IHttpObject> GetHttpAsync()
    {
        byte[] incomingData = new byte[1024 * 16]; //16KB

        int index = 0;
        while (Stream.DataAvailable)
        {
            int readBytes = await Stream.ReadAsync(incomingData, index, incomingData.Length - index);

            if (readBytes == 0)
            {
                // TODO overhead buffer for reading
            }

            index += readBytes;
        }

        return _httpSerializer.ToObject(incomingData);
    }

    public void SendHttp(IHttpObject httpObject)
    {
        byte[] data = Encoding.UTF8.GetBytes(_httpSerializer.ToHttp(httpObject));

        Stream.Write(data);
    }

    public async Task SendHttpAsync(IHttpObject httpObject)
    {
        byte[] data = Encoding.UTF8.GetBytes(_httpSerializer.ToHttp(httpObject));

        await Stream.WriteAsync(data);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _tcpConnection.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Close()
    {
        throw new NotImplementedException();
    }

    ~HttpClient()
    {
        Dispose(false);
    }
}