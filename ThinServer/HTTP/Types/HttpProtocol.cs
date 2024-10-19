namespace ThinServer.HTTP.Types
{
    public record HttpProtocol(string Version)
    {
        public static HttpProtocol Http0_9 => new("HTTP/0.9");
        public static HttpProtocol Http1_0 => new("HTTP/1.0");
        public static HttpProtocol Http1_1 => new("HTTP/1.1");
        public static HttpProtocol Http2 => new("HTTP/2");
        public static HttpProtocol Http3 => new("HTTP/3");

        public override string ToString() => Version;

        public static bool TryParse(string protocol, out HttpProtocol? _protocol)
        {
            switch (protocol.ToUpper())
            {
                case "HTTP/0.9":
                    _protocol = Http0_9;
                    break;

                case "HTTP/1.0":
                    _protocol = Http1_0;
                    break;

                case "HTTP/1.1":
                    _protocol = Http1_1;
                    break;

                case "HTTP/2":
                    _protocol = Http2;
                    break;

                case "HTTP/3":
                    _protocol = Http3;
                    break;

                default:
                    _protocol = null;
                    return false;
            }

            return true;
        }
    }
}