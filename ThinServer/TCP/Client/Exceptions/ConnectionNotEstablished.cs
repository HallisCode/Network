namespace ThinServer.TCP.Exceptions
{
    public class ConnectionNotEstablishedException : Exception
    {
        public ConnectionNotEstablishedException() : base()
        {
        }
        
        public ConnectionNotEstablishedException(string message) : base(message)
        {
        }
        
        public ConnectionNotEstablishedException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}