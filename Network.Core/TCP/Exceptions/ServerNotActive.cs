using System;

namespace Network.Core.TCP.Exceptions
{
    public class ServerNotActive : Exception
    {
        public ServerNotActive()
        {
        }

        public ServerNotActive(string message) : base(message)
        {
        }

        public ServerNotActive(string message, Exception inner) : base(message, inner)
        {
        }
    }
}