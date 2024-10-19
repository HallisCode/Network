using System.Runtime.Serialization;
using System.Text;
using ThinServer.HTTP.Types;
using HttpMethod = ThinServer.HTTP.Types.HttpMethod;

namespace ThinServer.HTTP
{
    public class HttpSerializer : IHttpSerializer
    {
        public IHttpObject ToObject(byte[] data)
        {
            return _ToObject(data);
        }

        public string ToHttp(IHttpObject httpObject)
        {
            return _ToHttp(httpObject);
        }


        private IHttpObject _ToObject(byte[] data)
        {
            IHttpObject httpObject;

            HttpRaw httpRaw = _UnbundleHttp(Encoding.UTF8.GetString(data));

            HttpStartLine startLine = _ParseStartLine(httpRaw);
            Dictionary<string, string> headers = _ParseHeaders(httpRaw);
            byte[] body = httpRaw.Body;

            bool isRequest = startLine.Argument1.ToLower().StartsWith("http");
            if (isRequest)
            {
                HttpRequestLine httpRequestLine = _ParseRequestLine(startLine);

                httpObject = HttpObject.CreateRequest
                (
                    method: httpRequestLine.Method,
                    url: httpRequestLine.URL,
                    protocol: httpRequestLine.Protocol,
                    headers: headers,
                    body: body
                );
            }
            else
            {
                HttpResponseLine httpResponseLine = _ParseResponseLine(startLine);

                httpObject = HttpObject.CreateResponse
                (
                    protocol: httpResponseLine.Protocol,
                    code: httpResponseLine.Code,
                    message: httpResponseLine.Message,
                    headers: headers,
                    body: body
                );
            }

            return httpObject;
        }

        private string _ToHttp(IHttpObject httpObject)
        {
            StringBuilder httpBuilder = new StringBuilder();

            // Request line
            if (httpObject.IsRequest)
            {
                httpBuilder.AppendLine($"{httpObject.Method.ToString().ToUpper()} " +
                                       $"{httpObject.URL} " +
                                       $"{httpObject.Protocol}");
            }
            else
            {
                httpBuilder.AppendLine($"{httpObject.Protocol} " +
                                       $"{httpObject.Code} " +
                                       $"{httpObject.Message}");
            }

            // Headers
            foreach (KeyValuePair<string, string> head in httpObject.Headers)
            {
                httpBuilder.AppendLine($"{head.Key}: {head.Value}");
            }

            // Разделяем body через перенос пустой строки
            httpBuilder.AppendLine();
            // Body
            httpBuilder.Append(_SerializeToString(httpObject.Body));

            return httpBuilder.ToString();
        }

        /// <summary>
        /// Парсит стартовую строку http, с расчётом что это http request.
        /// </summary>
        /// <param name="startLine"></param>
        /// <returns></returns>
        /// <exception cref="SerializationException"></exception>
        private HttpRequestLine _ParseRequestLine(HttpStartLine startLine)
        {
            HttpMethod method;
            string url;
            HttpProtocol protocol;

            bool isMethod = Enum.TryParse(typeof(HttpMethod), startLine.Argument1, true, out object _method);
            if (isMethod is false)
            {
                throw new SerializationException($"Неизвестный http метод <{startLine.Argument1}>");
            }

            method = (HttpMethod)_method;

            url = startLine.Argument2;

            bool isHttpProtocol = HttpProtocol.TryParse(startLine.Argument3, out HttpProtocol? _protocol);
            if (isHttpProtocol is false)
            {
                throw new SerializationException($"Неизвестный http протокол <{startLine.Argument3}>");
            }

            protocol = _protocol;

            return new HttpRequestLine(method, url, protocol);
        }

        /// <summary>
        /// Парсит стартовую строку http, с расчётом что это http response.
        /// </summary>
        /// <param name="startLine"></param>
        /// <returns></returns>
        /// <exception cref="SerializationException"></exception>
        private HttpResponseLine _ParseResponseLine(HttpStartLine startLine)
        {
            HttpProtocol protocol;
            HttpStatusCode code;
            string message;

            bool isHttpProtocol = HttpProtocol.TryParse(startLine.Argument3, out HttpProtocol? _protocol);
            if (isHttpProtocol is false)
            {
                throw new SerializationException($"Неизвестный http протокол <{startLine.Argument3}>");
            }

            protocol = _protocol;

            bool isNumber = int.TryParse(startLine.Argument2, out int _code);
            if (isNumber is false)
            {
                throw new SerializationException($"Http status code должен быть числом <{startLine.Argument2}>");
            }

            if (_code < 0 || _code > 500)
            {
                throw new SerializationException($"Неизвестный http status code <{_code}>");
            }

            code = _code;

            message = startLine.Argument3;

            return new HttpResponseLine(protocol, code, message);
        }

        /// <summary>
        /// Разбивает http запрос на части : startLine, headers, body.
        /// </summary>
        /// <param name="http">Http в строковом представлении.</param>
        /// <returns></returns>
        private HttpRaw _UnbundleHttp(string http)
        {
            string startLine;
            string? headers = null;
            byte[]? body = null;

            int indexEndStartLine = http.IndexOf(Environment.NewLine);
            if (indexEndStartLine < 0)
            {
                throw new SerializationException($"Невозможно найти конец стартовой строки запроса http.");
            }

            int lengthStartLine = indexEndStartLine - 1;
            startLine = http.Substring(0, lengthStartLine);


            int indexEndHeaders = http.IndexOf(Environment.NewLine + Environment.NewLine);
            if (indexEndHeaders < 0)
            {
                headers = null;
            }

            int lengthHeaders = indexEndHeaders - lengthStartLine - 1;
            headers = http.Substring(indexEndStartLine + 1, lengthHeaders);

            string otherData = http.Substring(indexEndHeaders + 1, http.Length - indexEndHeaders + 1);
            if (string.IsNullOrWhiteSpace(otherData) is false)
            {
                body = _SerializeToBytes(otherData);
            }
            
            return new HttpRaw(startLine, headers, body);
        }

        /// <summary>
        /// Разбивает стартовую строку http на аргументы.
        /// </summary>
        /// <param name="httpRaw"></param>
        /// <returns></returns>
        /// <exception cref="SerializationException"></exception>
        private HttpStartLine _ParseStartLine(HttpRaw httpRaw)
        {
            string[] arguments = httpRaw.StartLine.Split(' ');
            if (arguments.Length != 3)
            {
                throw new SerializationException($"Неккоректная длина строки запроса http {arguments.Length}/3");
            }

            return new HttpStartLine(arguments[0], arguments[1], arguments[2]);
        }

        /// <summary>
        /// Парсит headers и переводит их в словарь.
        /// </summary>
        /// <param name="httpRaw"></param>
        /// <returns>Словарь с заголовками http.</returns>
        private Dictionary<string, string>? _ParseHeaders(HttpRaw httpRaw)
        {
            if (string.IsNullOrWhiteSpace(httpRaw.Headers)) return null;

            string[] headers = httpRaw.Headers.Split(Environment.NewLine, 2)[1].Split(Environment.NewLine);

            Dictionary<string, string> _headers = new Dictionary<string, string>();
            foreach (string head in headers)
            {
                string[] keyValue = head.Split(':', 2);

                if (keyValue.Length != 2) continue;

                _headers.Add(keyValue[0], keyValue[1]);
            }

            return _headers;
        }

        private string _SerializeToString(IEnumerable<byte> data)
        {
            return Encoding.UTF8.GetString(data.ToArray());
        }

        private byte[] _SerializeToBytes(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// Представляет строку запроса http request.
        /// </summary>
        public record HttpRequestLine(HttpMethod Method, string URL, HttpProtocol Protocol);

        /// <summary>
        /// Представляет строку запроса для http response.
        /// </summary>
        public record HttpResponseLine(HttpProtocol Protocol, HttpStatusCode Code, string Message);

        /// <summary>
        /// Представляет сырой запрос http разбитый на секции.
        /// </summary>
        public record HttpRaw(string StartLine, string? Headers, byte[]? Body);

        /// <summary>
        /// Представляет сырую стартовую строку разбитую на аргументы.
        /// </summary>
        public record HttpStartLine(string Argument1, string Argument2, string Argument3);
    }

    public class HttpSerializerException : Exception
    {
        public HttpSerializerException()
        {
        }

        public HttpSerializerException(string message) : base(message)
        {
        }

        public HttpSerializerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}