using System;

namespace Network.Core.HTTP.Exceptions
{
    public class ReceiveTimeOutException : Exception
    {
        public ReceiveTimeOutException()
        {
        }

        public ReceiveTimeOutException(string message) : base(message)
        {
        }

        public ReceiveTimeOutException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}