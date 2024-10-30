using System.Net;

namespace ThinServer
{
    public interface IServer : IDisposable
    {
        /// <summary>
        /// Тайм-аут приёма входящих данных в миллисекундах.
        /// </summary>
        int TimeOutMilleSeconds { get; set; }

        /// <summary>
        /// Работает ли сервер.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Эндпоинт на котормо работает сервер.
        /// </summary>
        IPEndPoint EndPoint { get; }

        /// <summary>
        /// Устанавливает обработчик входящих запросов.
        /// </summary>
        /// <param name="handler"></param>
        void SetHandler(Func<IServerHttpRequest, Task> handler);

        /// <summary>
        /// Запускает работу сервера и ждёт когда он закончит работу.
        /// </summary>
        void Start();

        /// <summary>
        /// Запускает работу сервера
        /// </summary>
        Task StartAsync();
        
        /// <summary>
        /// Завершает работу сервера.
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Завершает работу сервера.
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Завершает работу сервера, освобождая ресурсы.
        /// </summary>
        void Close();
    }
}