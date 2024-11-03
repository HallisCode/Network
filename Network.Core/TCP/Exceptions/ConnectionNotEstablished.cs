using System;

namespace Network.Core.TCP.Exceptions
{
    public class ConnectionNotEstablishedException : Exception
    {
        public ConnectionNotEstablishedException()
        {
        }

        public ConnectionNotEstablishedException(string message) : base(message)
        {
        }

        public ConnectionNotEstablishedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}