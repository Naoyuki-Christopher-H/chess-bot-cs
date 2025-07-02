using System;
using System.IO;

namespace chess_bot_cs.Utilities
{
    public static class FileLogger
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chessbot.log");

        public static void Log(string message)
        {
            try
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
            }
            catch
            {
                // Silently fail if logging isn't possible
            }
        }

        public static void LogError(string message, Exception ex)
        {
            Log($"ERROR: {message} - {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }
}