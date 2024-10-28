using ThinServer.HTTP;

namespace ThinServer
{
    public class ServerHttpRequest : IServerHttpRequest
    {
        // Private properties
        private IHttpObject _request;
        
        // Public properties
        public IHttpObject Request { get; }
        public bool IsResponseSet { get; protected set; }

        public IHttpObject? Response
        {
            get => _request;
            set
            {
                _request = value;
                IsResponseSet = true;
            }
        }


        public ServerHttpRequest(IHttpObject request)
        {
            _request = request;
        }
    }
}