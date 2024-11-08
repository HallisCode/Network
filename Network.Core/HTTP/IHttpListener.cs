using System;
using System.Net;
using System.Threading.Tasks;
using Network.Core.TCP.Exceptions;

namespace Network.Core.HTTP
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
        /// <exception cref="ServerNotActive"></exception>
        /// <returns></returns>
        IHttpClient AcceptConnection();

        /// <summary>
        /// Принимает входящее соединение
        /// </summary>
        /// <exception cref="ServerNotActive"></exception>
        /// <returns></returns>
        Task<IHttpClient> AcceptConnectionAsync();

        /// <summary>
        /// Прекращает прослушивать входящие http-соединения.
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Прекращает прослушивать входящие http-соединения.
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Прекращает прослушивать входящие http-соединения и освобождает ресурсы.
        /// </summary>
        void Close();
    }
}