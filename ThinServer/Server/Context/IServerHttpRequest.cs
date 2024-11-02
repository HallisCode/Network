using ThinServer.HTTP;

namespace ThinServer
{
    public interface IServerHttpRequest
    {
        IHttpObject Request { get; }
        IHttpObject? Response { get; set; }
        bool IsResponseSet { get; }
    }
}