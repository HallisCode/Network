namespace Network.ThinServer.Logger
{
    public class Logger : ILogger
    {
        public Logger()
        {
        }


        public void LogDebug(string message)
        {
            _LogMessage("Debug information.", message, ConsoleColor.DarkMagenta);
        }

        public void LogInformation(string message)
        {
            _LogMessage("Other information.", message, ConsoleColor.Gray);
        }

        public void LogWarning(string message)
        {
            _LogMessage("Warning information.", message, ConsoleColor.Yellow);
        }

        public void LogError(string message)
        {
            _LogMessage("Error information.", message, ConsoleColor.Red);
        }

        private void _LogMessage(string title, string message, ConsoleColor colorTitle)
        {
            Console.ForegroundColor = colorTitle;

            Console.Write($"\n{title}\t\n");

            Console.ResetColor();

            Console.WriteLine(message);
        }
    }
}