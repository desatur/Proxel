namespace Proxel.Log4Console.Enums
{
    public class SaveLogs
    {
        const string logDirectory = "logs";

        static SaveLogs()
        {
            Initialize();
        }

        private static void Initialize()
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            Log.OnLog += SaveLogToFile;
        }

        private static void SaveLogToFile(LogType type, string formattedMessage, string message, string location)
        {
            string logFileName = $"{DateTime.Now:yyyy-MM-dd}.log";
            string logFilePath = Path.Combine(logDirectory, logFileName);
            File.AppendAllText(logFilePath, formattedMessage + Environment.NewLine);
        }
    }
}
