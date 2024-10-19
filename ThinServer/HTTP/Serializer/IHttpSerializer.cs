namespace ThinServer.HTTP
{
    public interface IHttpSerializer
    {
        IHttpObject ToObject(byte[] data);

        string ToHttp(IHttpObject httpObject);
    }
}