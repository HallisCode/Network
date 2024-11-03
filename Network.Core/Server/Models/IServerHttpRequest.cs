using Network.Core.HTTP;

namespace Network.Core.Server.Models
{
    public interface IServerHttpRequest
    {
        IHttpObject Request { get; }
        IHttpObject? Response { get; set; }
        bool IsResponseSet { get; }
    }
}