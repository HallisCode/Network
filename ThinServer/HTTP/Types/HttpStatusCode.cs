namespace ThinServer.HTTP.Types
{
    public readonly record struct HttpStatusCode
    {
        public readonly short Code;

        public HttpStatusCode(int statusCode)
        {
            if (statusCode > 500 || statusCode < 0)
            {
                throw new HttpStatusCodeException("Status code out of range 0..500.");
            }

            Code = (short)statusCode;
        }

        public override string ToString()
        {
            return Code.ToString();
        }

        public static implicit operator HttpStatusCode(int number)
        {
            return new HttpStatusCode(number);
        }
    }

    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCodeException()
        {
        }

        public HttpStatusCodeException(string message) : base(message)
        {
        }

        public HttpStatusCodeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}