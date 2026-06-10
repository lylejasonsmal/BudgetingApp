using System.Diagnostics;

namespace MyFirstApp.Domain.Helpers
{
    public readonly record struct Logger
    {
        private Logger(string message, object[] args)
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] " + string.Format(message, args));
        }

        public static void Log(string message, params object[] args)
            => _ = new Logger(message, args);
    }

}
