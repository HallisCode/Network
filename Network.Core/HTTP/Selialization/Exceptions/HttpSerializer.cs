using System;

namespace Network.Core.HTTP.Exceptions
{
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