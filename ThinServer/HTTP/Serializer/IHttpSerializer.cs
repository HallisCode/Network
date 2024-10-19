namespace ThinServer.HTTP
{
    /// <summary>
    /// Сериализует http запросы.
    /// </summary>
    public interface IHttpSerializer
    {
        /// <summary>
        /// В объект <see cref="IHttpObject"/>
        /// </summary>
        /// <param name="data">Http запрос в байтах.</param>
        /// <returns></returns>
        IHttpObject ToObject(byte[] data);
        
        /// <summary>
        /// В http запрос в строковм представлении.
        /// </summary>
        /// <param name="httpObject"></param>
        /// <returns></returns>
        string ToHttp(IHttpObject httpObject);
    }
}