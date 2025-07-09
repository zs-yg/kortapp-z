using System;
using System.IO;
using System.Text;

namespace AppStore
{
    public static class PathHelper
    {
        /// <summary>
        /// 清理可执行文件路径，移除参数和引号
        /// </summary>
        public static string CleanExecutablePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            // 处理带引号的路径
            path = path.Trim().Trim('"');

            // 找到第一个空格或参数符号
            int paramIndex = path.IndexOfAny(new[] { ' ', '-', '/' });
            if (paramIndex > 0)
            {
                // 检查空格前的部分是否是有效路径
                string potentialPath = path.Substring(0, paramIndex).Trim();
                if (File.Exists(potentialPath))
                {
                    return potentialPath;
                }

                // 尝试提取带空格的路径
                if (path.Contains("\""))
                {
                    int startQuote = path.IndexOf('"');
                    int endQuote = path.IndexOf('"', startQuote + 1);
                    if (endQuote > startQuote)
                    {
                        potentialPath = path.Substring(startQuote + 1, endQuote - startQuote - 1);
                        if (File.Exists(potentialPath))
                        {
                            return potentialPath;
                        }
                    }
                }
            }

            // 如果路径包含.exe，尝试提取到.exe结尾
            int exeIndex = path.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
            if (exeIndex > 0)
            {
                string potentialPath = path.Substring(0, exeIndex + 4);
                if (File.Exists(potentialPath))
                {
                    return potentialPath;
                }
            }

            // 最后尝试直接返回路径
            return path;
        }

        /// <summary>
        /// 验证路径是否指向可执行文件
        /// </summary>
        public static bool IsValidExecutablePath(string path)
        {
            try
            {
                string cleanPath = CleanExecutablePath(path);
                return File.Exists(cleanPath) && 
                       cleanPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
