using Proxel.Log4Console.Enums;

namespace Proxel.Log4Console
{
    public static class Log
    {
#if DEBUG
        const bool DebugEnabled = true;
#else
        const bool DebugEnabled = false;
#endif

        public delegate void LogEventHandler(LogType type, string formattedMessage, string message, string location);
        public static event LogEventHandler OnLog;

        public static void Error(string message, string location = null)
        {
            HandleLog(LogType.Error, message, location);
        }

        public static void Info(string message, string location = null)
        {
            HandleLog(LogType.Info, message, location);
        }

        public static void Warn(string message, string location = null)
        {
            HandleLog(LogType.Warning, message, location);
        }

        public static void Debug(string message, string location = null)
        {
            HandleLog(LogType.Debug, message, location);
        }

        private static void HandleLog(LogType type, string message, string location)
        {
            if (type == LogType.Debug && !DebugEnabled) return;
            SetConsoleColor(type);
            string formattedMessage = $"[{DateTime.Now}] [{type}]";
            if (!string.IsNullOrEmpty(location))
            {
                formattedMessage += $" {location} >>";
            }
            formattedMessage += $" {message}";
            Console.WriteLine(formattedMessage);
            Console.ResetColor();
            OnLog?.Invoke(type, formattedMessage, message, location);
        }

        private static void SetConsoleColor(LogType type)
        {
            Console.ForegroundColor = type switch
            {
                LogType.Error => ConsoleColor.Red,
                LogType.Info => ConsoleColor.White,
                LogType.Warning => ConsoleColor.Yellow,
                LogType.Debug => ConsoleColor.DarkMagenta,
            };
        }
    }
}
