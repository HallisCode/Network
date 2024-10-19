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

            HttpRaw httpRaw = new HttpRaw(_SerializeToString(data));

            HttpStartLine startLine = _GetStartLine(httpRaw);
            Dictionary<string, string> headers = _GetHeaders(httpRaw);
            byte[] body = _GetBody(httpRaw);

            bool isRequest;
            if (startLine.Argument1.ToLower().StartsWith("http"))
            {
                isRequest = false;
                HttpResponseLine httpResponseLine = _GetResponseLine(startLine);

                httpObject = HttpObject.CreateResponse
                (
                    protocol: httpResponseLine.Protocol,
                    code: httpResponseLine.Code,
                    message: httpResponseLine.Message,
                    headers: headers,
                    body: body
                );
            }
            else
            {
                isRequest = true;
                HttpRequestLine httpRequestLine = _GetRequestLine(startLine);

                httpObject = HttpObject.CreateRequest
                (
                    method: httpRequestLine.Method,
                    url: httpRequestLine.URL,
                    protocol: httpRequestLine.Protocol,
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

        private HttpRequestLine _GetRequestLine(HttpStartLine startLine)
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

        private HttpResponseLine _GetResponseLine(HttpStartLine startLine)
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

        private HttpStartLine _GetStartLine(HttpRaw httpRaw)
        {
            string startLine = httpRaw.Http.Split(Environment.NewLine, 2)[0];

            return new HttpStartLine(startLine);
        }

        private Dictionary<string, string> _GetHeaders(HttpRaw httpRaw)
        {
            string headPart = httpRaw.Http.Split(Environment.NewLine + Environment.NewLine, 2)[0];
            string[] headers = headPart.Split(Environment.NewLine, 2)[1].Split(Environment.NewLine);

            Dictionary<string, string> _headers = new Dictionary<string, string>();

            foreach (string head in headers)
            {
                string[] keyValue = head.Split(':', 2);

                if (keyValue.Length != 2) continue;

                _headers.Add(keyValue[0], keyValue[1]);
            }

            return _headers;
        }

        private byte[] _GetBody(HttpRaw httpRaw)
        {
            string body = httpRaw.Http.Split(Environment.NewLine + Environment.NewLine, 2)[1];

            return _SerializeToBytes(body);
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
        /// Представляет сырой запрос http в строковом формате.
        /// </summary>
        public record HttpRaw(string Http);

        /// <summary>
        /// Представляет строку запроса http request.
        /// </summary>
        public record HttpRequestLine(HttpMethod Method, string URL, HttpProtocol Protocol);

        /// <summary>
        /// Представляет строку запроса для http response.
        /// </summary>
        public record HttpResponseLine(HttpProtocol Protocol, HttpStatusCode Code, string Message);

        /// <summary>
        /// Представляет сырую стартовую строку разбитую на аргументы.
        /// </summary>
        public record HttpStartLine
        {
            public string Argument1 { get; init; }
            public string Argument2 { get; init; }
            public string Argument3 { get; init; }

            public HttpStartLine(string line)
            {
                string[] lineArguments = line.Split(' ');

                if (lineArguments.Length != 3)
                {
                    throw new HttpSerializerException(
                        $"Неккоректное количество элементов в строке http запроса {lineArguments.Length}."
                    );
                }

                Argument1 = lineArguments[0];
                Argument2 = lineArguments[1];
                Argument3 = lineArguments[2];
            }
        }
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