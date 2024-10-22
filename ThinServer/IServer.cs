namespace ThinServer
{
    public interface IServer
    {
        void SetHandler();
        
        void Start();

        void StartAsync();

        void Stop();

        void StopAsync();
    }
}