using System.Net.Sockets;
using ThinServer.HTTP.Exceptions;

namespace ThinServer.HTTP
{
    public interface IHttpClient : IDisposable
    {
        /// <summary>
        /// Получает текущий поток обмена данными.
        /// </summary>
        /// <returns></returns>
        NetworkStream Stream { get; }

        /// <summary>
        /// Тайм-аут приёма входящих данных в миллисекундах.
        /// </summary>
        int TimeOutMilleSeconds { get; set; }

        /// <summary>
        /// Размер буффера приёма данных в байтах
        /// </summary>
        int BufferSize { get; set; }

        /// <summary>
        /// Получает http запрос.
        /// </summary>
        /// <exception cref="BufferOverflowException">Буфер приёма данных переполнен.</exception>
        /// <exception cref="ReceiveTimeOutException">Превышено время ожидания входящих данных.</exception>
        /// <returns></returns>
        IHttpObject GetHttp();

        /// <summary>
        /// Получает http запрос.
        /// </summary>
        /// <exception cref="BufferOverflowException">Буфер приёма данных переполнен.</exception>
        /// <exception cref="ReceiveTimeOutException">Превышено время ожидания входящих данных.</exception>
        /// <returns></returns>
        Task<IHttpObject> GetHttpAsync(CancellationToken token = default);

        /// <summary>
        /// Отправляет http запрос.
        /// </summary>
        /// <returns></returns>
        void SendHttp(IHttpObject httpObject);

        /// <summary>
        /// Отправляет http запрос.
        /// </summary>
        /// <returns></returns>
        Task SendHttpAsync(IHttpObject httpObject, CancellationToken token = default);

        /// <summary>
        /// Закрывает текущее соединение и свобождает ресурсы.
        /// </summary>
        /// <returns></returns>
        void Close();
    }
}