using System.Collections.Immutable;
using System.Collections.ObjectModel;
using ThinServer.HTTP.Types;
using HttpMethod = ThinServer.HTTP.Types.HttpMethod;

namespace ThinServer.HTTP
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

        ReadOnlyCollection<byte>? Body { get; }
    }
}