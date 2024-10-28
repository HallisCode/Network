using ThinServer.HTTP;

namespace ThinServer
{
    public class ServerHttpRequest : IServerHttpRequest
    {
        // Private properties
        private IHttpObject _response;

        // Public properties
        public bool IsResponseSet { get; protected set; }
        public IHttpObject Request { get; }

        public IHttpObject? Response
        {
            get => _response;
            set
            {
                _response = value;
                IsResponseSet = true;
            }
        }


        public ServerHttpRequest(IHttpObject request)
        {
            Request = request;
        }
    }
}