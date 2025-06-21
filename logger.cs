using System;
using System.IO;
using System.Text;

namespace AppStore
{
    public static class Logger
    {
        private static readonly string LogsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "zsyg", "kortapp-z", ".logs");
        private static readonly object LockObject = new object();

        static Logger()
        {
            try
            {
                // 确保logs目录存在
                if (!Directory.Exists(LogsDirectory))
                {
                    Directory.CreateDirectory(LogsDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"无法创建日志目录: {LogsDirectory}, 错误: {ex.Message}");
                throw;
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
