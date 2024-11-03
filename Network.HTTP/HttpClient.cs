using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Network.Core.HTTP;
using Network.Core.HTTP.Exceptions;
using Network.Core.HTTP.Serialization.Exceptions;
using Network.Core.TCP.TCP;

namespace Network.HTTP
{
    public class HttpClient : IHttpClient
    {
        private IHttpSerializer _httpSerializer;
        private ITcpClient _tcpConnection;
        private bool _disposed;

        public NetworkStream Stream
        {
            get => _tcpConnection.GetStream();
        }

        public int TimeOutMilleSeconds { get; set; } = 16000;
        public int BufferSize { get; set; } = 1024 * 16; // 16KB


        public HttpClient(ITcpClient connection, IHttpSerializer serializer)
        {
            _tcpConnection = connection;
            _httpSerializer = serializer;
        }

        public IHttpObject GetHttp()
        {
            Task<IHttpObject> gettingHttp = GetHttpAsync();

            gettingHttp.Wait();

            return gettingHttp.Result;
        }

        public async Task<IHttpObject> GetHttpAsync(CancellationToken token = default)
        {
            var regexContentLength = new Regex(@"Content-Length:\s*(\d+)", RegexOptions.IgnoreCase);
            const string seperatorCharacter = "\r\n\r\n";

            bool isHasSeperatingLine = false;
            int indexSeperatingLine = -1;

            bool isHasContentLength = false;
            int contentLengthSize = 0;

            byte[] incomingData = new byte[BufferSize];
            int indexNextFree = 0;

            int indexBodyStart = -1;

            while (!token.IsCancellationRequested)
            {
                // Проверяем доступный объем буффера приёма данных
                int availableBytesToWrite = incomingData.Length - (indexNextFree == 0 ? 0 : indexNextFree - 1);
                if (availableBytesToWrite <= 0)
                {
                    throw new BufferOverflowException("Буфер приёма данных переполнен.");
                }

                // Создаём таски на полученние данных и на time-out
                Task<int> receiveTask = Stream.ReadAsync(incomingData, indexNextFree, availableBytesToWrite, token);
                Task delayTask = Task.Delay(TimeOutMilleSeconds, token);

                Task firstFinished = await Task.WhenAny(receiveTask, delayTask);

                if (firstFinished == delayTask)
                {
                    throw new ReceiveTimeOutException("Превышено время ожидания входящих данных.");
                }

                // Сдвигаем индекс
                int receivedBytes = await receiveTask;
                indexNextFree += receivedBytes;

                // Переводим имеющиеся данные в строковое представление
                string requestString = Encoding.UTF8.GetString(incomingData, 0, indexNextFree);

                // Проверяем на наличие разделительной строки
                if (isHasSeperatingLine is false)
                {
                    isHasSeperatingLine = requestString.Contains(seperatorCharacter);
                    indexSeperatingLine = isHasSeperatingLine ? requestString.IndexOf(seperatorCharacter) : -1;
                }

                // Проверяем на наличие тела у запроса
                if (isHasContentLength is false)
                {
                    // Получаем размер body
                    Match matchContentLength = regexContentLength.Match(requestString);
                    if (matchContentLength.Success)
                    {
                        isHasContentLength = true;
                        contentLengthSize = int.Parse(matchContentLength.Groups[1].Value);
                    }
                }

                // Если тело присутствует и найдена разделительная строка, получаем индекс начала тела
                if (indexBodyStart < 0 && isHasSeperatingLine && isHasContentLength)
                {
                    // Вычитаем -1 потому, что indexOf возвращает индекс первого символа из совпадения, таким образом
                    // 1-ый символ уже включен в длину
                    indexBodyStart = indexSeperatingLine + seperatorCharacter.Length - 1;
                }


                // Тело присутствует, все его байты записаны => http запрос полностью принят.
                // indexNextFree -1 потому, что индекс указывает на следующий свободный байт, а мы оперируем 
                // уже считанными данными
                if (indexBodyStart >= 0 && (indexNextFree - 1) - indexBodyStart == contentLengthSize)
                {
                    break;
                }

                // Разделительная строка найдена, тело отсутствует => http запрос полностью принят.
                if (isHasSeperatingLine && isHasContentLength is false)
                {
                    break;
                }
            }

            return _httpSerializer.ToObject(incomingData[0..indexNextFree]);
        }

        public void SendHttp(IHttpObject httpObject)
        {
            byte[] data = Encoding.UTF8.GetBytes(_httpSerializer.ToHttp(httpObject));

            Stream.Write(data);
        }
        
        public async Task SendHttpAsync(IHttpObject httpObject, CancellationToken token = default)
        {
            byte[] data = Encoding.UTF8.GetBytes(_httpSerializer.ToHttp(httpObject));

            await Stream.WriteAsync(data, token);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _tcpConnection.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            Dispose();
        }

        ~HttpClient()
        {
            Dispose(false);
        }
    }
}