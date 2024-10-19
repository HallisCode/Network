using System.Net;
using System.Net.Sockets;

namespace ThinServer.TCP
{
    /// <summary>
    /// Интерфейс TCP listener.
    /// </summary>
    public interface ITcpListener : IDisposable
    {
        /// <summary>
        /// Эндпойнт который должен прослушиваться.
        /// </summary>
        IPEndPoint LocalEndpoint { get; }

        /// <summary>
        /// Сокет на котором работает сервер.
        /// </summary>
        Socket Server { get; }

        /// <summary>
        /// Слушает ли сервер входящие соединения.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Запуск прослушивания сокета.
        /// </summary>
        void Start();

        /// <summary>
        /// Принимает входящее соединение.
        /// </summary>
        /// <returns></returns>
        Socket AcceptSocket();

        /// <summary>
        /// Принимает входящее соединение.
        /// </summary>
        /// <returns></returns>
        Task<Socket> AcceptSocketAsync();

        /// <summary>
        /// Принимает входящее соединение.
        /// </summary>
        /// <returns></returns>
        ITcpClient AcceptTcpClient();

        /// <summary>
        /// Принимает входящее соединение.
        /// </summary>
        /// <returns></returns>
        Task<ITcpClient> AcceptTcpClientAsync();

        /// <summary>
        /// Прекращение прослушивания сокета.
        /// </summary>
        void Stop();

        /// <summary>
        /// Прекращение прослушивания сокета и освобождение ресурсов.
        /// </summary>
        void Close();
    }
}