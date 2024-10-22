using System.Net;

namespace ThinServer.HTTP
{
    /// <summary>
    /// Интерфейс IHttpListener 
    /// </summary>
    public interface IHttpListener : IDisposable
    {
        /// <summary>
        /// Эндпойнт который должен прослушиваться.
        /// </summary>
        IPEndPoint? LocalEndpoint { get; }

        /// <summary>
        /// Слушает ли listener входящие соединения.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Запускает прослушивание входящих http-соединений.
        /// </summary>
        void Start();

        /// <summary>
        /// Принимает входящее соединение
        /// </summary>
        /// <returns></returns>
        IHttpClient AcceptConnection();

        /// <summary>
        /// Принимает входящее соединение
        /// </summary>
        /// <returns></returns>
        Task<IHttpClient> AcceptConnectionAsync();

        /// <summary>
        /// Прекращает прослушивать входящие http-соединения.
        /// </summary>
        void Stop();

        /// <summary>
        /// Прекращает прослушивать входящие http-соединения и освобождает ресурсы.
        /// </summary>
        void Close();
    }
}