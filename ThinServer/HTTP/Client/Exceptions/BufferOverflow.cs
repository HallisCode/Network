namespace ThinServer.HTTP.Exceptions
{
    public class BufferOverflowException : Exception
    {
        public BufferOverflowException()
        {
        }

        public BufferOverflowException(string message) : base(message)
        {
        }

        public BufferOverflowException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}