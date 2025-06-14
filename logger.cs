using System;
using System.IO;
using System.Text;
namespace AppStore
{
    public static class Logger
    {
        private static readonly string LogsDirectory = "logs";
        private static readonly object LockObject = new object();
        static Logger()
        {
            // 确保logs目录存在
            if (!Directory.Exists(LogsDirectory))
            {
                Directory.CreateDirectory(LogsDirectory);
            }
        }
        public static void Log(string message)
        {
            lock (LockObject)
            {
                try
                {
                    string fileName = $"{DateTime.Now:yyyyMMddHHmmss}.log";
                    string filePath = Path.Combine(LogsDirectory, fileName);
                    using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
                    {
                        writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
                    }
                }
                catch (Exception ex)
                {
                    // 日志记录失败时输出到控制台
                    Console.WriteLine($"日志记录失败: {ex.Message}");
                }
            }
        }
        public static void LogError(string message, Exception? ex = null)
        {
            string errorMessage = $"ERROR: {message}";
            if (ex != null)
            {
                errorMessage += $"\nException: {ex}\nStackTrace: {ex.StackTrace}";
            }
            Log(errorMessage);
        }
    }
}
