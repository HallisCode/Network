using System.Collections.Immutable;
using System.Collections.ObjectModel;
using ThinServer.HTTP.Types;
using HttpMethod = ThinServer.HTTP.Types.HttpMethod;

namespace ThinServer.HTTP
{
    public class HttpObject : IHttpObject
    {
        public HttpMethod? Method { get; protected set; }
        public string? Message { get; protected set; }
        public string? URL { get; protected set; }
        public HttpStatusCode? Code { get; protected set; }
        public HttpProtocol? Protocol { get; protected set; }
        public bool IsRequest { get; protected set; }

        public ReadOnlyDictionary<string, string> Headers { get; protected set; }
        public ReadOnlyCollection<byte> Body { get; protected set; }

        protected HttpObject
        (
            Dictionary<string, string>? headers = null,
            byte[] body = null
        )
        {
            Headers = headers is null ? null : headers.AsReadOnly();
            Body = body is null ? null : body.AsReadOnly();
        }

        public static HttpObject CreateRequest
        (
            HttpMethod method,
            string url,
            HttpProtocol protocol,
            Dictionary<string, string> headers = null,
            byte[] body = null
        )
        {
            return new HttpObject(headers, body)
            {
                Method = method,
                URL = url,
                Protocol = protocol,

                IsRequest = true,
            };
        }

        public static HttpObject CreateResponse
        (
            HttpProtocol protocol,
            HttpStatusCode code,
            string message,
            Dictionary<string, string> headers = null,
            byte[] body = null
        )
        {
            return new HttpObject(headers, body)
            {
                Protocol = protocol,
                Code = code,
                Message = message,
                IsRequest = false
            };
        }
    }
}