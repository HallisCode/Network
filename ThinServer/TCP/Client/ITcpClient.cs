using System.Net;
using System.Net.Sockets;

namespace ThinServer.TCP
{
    public interface ITcpClient : IDisposable
    {
        /// <summary>
        /// Установлено ли соединение с другим ендпоинтом.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Сколько байт еще доступно на чтение.
        /// </summary>
        int Available { get; }

        /// <summary>
        /// Сокет на котором работает  клиент.
        /// </summary>
        Socket? Client { get; }

        /// <summary>
        /// Еднпоинт на котором работает сокет.
        /// </summary>
        IPEndPoint LocalEndpoint { get; }

        /// <summary>
        /// Ендпоинт с которым должно быть соединение.
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Размер буфера в байтах на входящие данные.
        /// </summary>
        int ReceiveBufferSize { get; set; }

        /// <summary>
        /// Размер буфера в байтах на исходящие данные.
        /// </summary>
        int SendBufferSize { get; set; }


        /// <summary>
        /// Подключиться к сокету.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        void Connect(string hostname, int port);

        /// <summary>
        /// Подключиться к сокету.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        Task ConnectAsync(string hostname, int port);

        /// <summary>
        /// Получить поток текущего соединения.
        /// </summary>
        /// <returns></returns>
        NetworkStream GetStream();

        /// <summary>
        /// Принимает входящие данные.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>Количество принятых байтов.</returns>
        int Receive(byte[] buffer);

        /// <summary>
        /// Принимает входящие данные.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset">Место в буфере для записи данных.</param>
        /// <param name="size">Количество байт на получение.</param>
        /// <param name="socketFlags"></param>
        /// <returns>Количество принятых байтов.</returns>
        public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags);

        /// <summary>
        /// Принимает входящие данные.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>Количество принятых байтов.</returns>
        Task<int> ReceiveAsync(byte[] buffer);

        /// <summary>
        /// Закрывает текущее соединение.
        /// </summary>
        void Stop();

        /// <summary>
        /// Закрывает текущее соединение и освобождает ресурсы.
        /// </summary>
        void Close();
    }
}