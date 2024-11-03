using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using Network.Core.HTTP.Types;

namespace Network.Core.HTTP
{
    /// <summary>
    /// Представляет HTTP ( запрос или ответ ).
    /// </summary>
    public interface IHttpObject
    {
        HttpMethod? Method { get; }

        string? Message { get; }

        string? URL { get; }

        HttpStatusCode? Code { get; }


        HttpProtocol? Protocol { get; }

        bool IsRequest { get; }

        ReadOnlyDictionary<string, string>? Headers { get; }

        ReadOnlyCollection<byte> Body { get; }
    }
}