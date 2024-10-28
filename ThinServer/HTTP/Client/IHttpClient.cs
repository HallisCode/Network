using System.Net.Sockets;

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
        /// Получает http запрос.
        /// </summary>
        /// <returns></returns>
        IHttpObject GetHttp();

        /// <summary>
        /// Получает http запрос.
        /// </summary>
        /// <returns></returns>
        Task<IHttpObject> GetHttpAsync(CancellationToken token);

        /// <summary>
        /// Отправляет http запрос.
        /// </summary>
        /// <returns></returns>
        void SendHttp(IHttpObject httpObject);

        /// <summary>
        /// Отправляет http запрос.
        /// </summary>
        /// <returns></returns>
        Task SendHttpAsync(IHttpObject httpObject);

        /// <summary>
        /// Закрывает текущее соединение и свобождает ресурсы.
        /// </summary>
        /// <returns></returns>
        void Close();
    }
}