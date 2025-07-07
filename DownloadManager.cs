using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppStore
{
    public class DownloadManager
    {


        [DllImport("shell32.dll")]
        private static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint dwFlags,
            IntPtr hToken,
            out IntPtr ppszPath);


        private static DownloadManager instance = null!;
        public static DownloadManager Instance => instance ??= new DownloadManager();

        private Process? currentProcess;
        public List<DownloadItem> DownloadItems { get; } = new List<DownloadItem>();
        
        public event Action<DownloadItem> DownloadAdded = delegate { };
        public event Action<DownloadItem> DownloadProgressChanged = delegate { };
        public event Action<DownloadItem> DownloadCompleted = delegate { };

        // 内部类封装进程结果
        private class ProcessResult
        {
            public int ExitCode { get; set; } = -1;
            public bool HasExited { get; set; }
        }

        // 安全获取进程结果
        private ProcessResult GetProcessResult(Process? process)
        {
            var result = new ProcessResult();
            if (process == null || process.StartInfo == null) return result;
            
            try 
            {
                if (!process.HasExited)
                {
                    process.WaitForExit(5000);
                }
                
                result.HasExited = process.HasExited;
                if (result.HasExited)
                {
                    result.ExitCode = process.ExitCode;
                }
            }
            catch 
            {
                // 忽略所有异常，使用默认值
            }
            
            return result;
        }

        public void StartDownload(string fileName, string url)
        {
            // 从URL获取原始文件名用于显示
            var uri = new Uri(url);
            var originalFileName = Path.GetFileName(uri.LocalPath);
            
            var downloadItem = new DownloadItem
            {
                FileName = originalFileName, // 显示原始文件名
                Progress = 0,
                Status = "准备下载"
            };
            
            DownloadItems.Add(downloadItem);
            DownloadAdded?.Invoke(downloadItem);

            Task.Run(() => DownloadFile(downloadItem, fileName, url));
        }

        private void DownloadFile(DownloadItem downloadItem, string fileName, string url)
        {
            string downloadsDir = string.Empty;
            try
            {
                // 获取并验证下载路径
                downloadsDir = GetDownloadPath();
                
                try
                {
                    // 检查路径是否有效
                    if (string.IsNullOrWhiteSpace(downloadsDir))
                    {
                        throw new Exception("下载路径为空");
                    }

                    // 尝试创建目录（如果不存在）
                    Directory.CreateDirectory(downloadsDir);

                    // 验证目录是否可写
                    string testFile = Path.Combine(downloadsDir, "write_test.tmp");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                }
                catch (Exception ex)
                {
                    // 回退到默认下载路径
                    string defaultPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                        "Downloads");
                    
                    Logger.LogError($"下载路径{downloadsDir}不可用，将使用默认路径: {defaultPath}", ex);
                    downloadsDir = defaultPath;
                    Directory.CreateDirectory(downloadsDir);
                }
                


                // 构建aria2c路径
                var aria2cPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource", "aria2c.exe");
                
                if (!File.Exists(aria2cPath))
                {
                    throw new FileNotFoundException($"找不到aria2c.exe: {aria2cPath}");
                }

                // 设置线程数为16并添加详细日志
                // 从URL获取原始文件名
                var uri = new Uri(url);
                var originalFileName = Path.GetFileName(uri.LocalPath);
                var arguments = $"--out=\"{originalFileName}\" --dir=\"{downloadsDir}\" --split=16 --max-connection-per-server=16 {url}";


                currentProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = aria2cPath,
                        Arguments = arguments,
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                // 获取目标文件路径
                string downloadPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads",
                    fileName);
                
                long totalSize = 0;
                
                // 添加进度检测超时机制
                var progressTimer = new System.Timers.Timer(5000); // 5秒无更新视为完成
                progressTimer.Elapsed += (s, e) => {
                    progressTimer.Stop();
                    if (downloadItem.Progress < 100) {
                        downloadItem.Progress = 100;
                        downloadItem.Status = "下载完成 (100%)";
                        DownloadProgressChanged?.Invoke(downloadItem);
                    }
                };
                
                currentProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {

                        
                        // 重置超时计时器
                        progressTimer.Stop();
                        progressTimer.Start();
                        
                        // 解析总大小
                        if (e.Data.Contains("Length:"))
                        {
                            var sizeStr = e.Data.Split(new[]{"Length:"}, StringSplitOptions.RemoveEmptyEntries)[1]
                                .Split('(')[0].Trim();
                            if (long.TryParse(sizeStr, out totalSize))
                            {

                            }
                        }
                        
                        // 解析进度百分比
                        if (e.Data.Contains("%)"))
                        {
                            var start = e.Data.IndexOf("(") + 1;
                            var end = e.Data.IndexOf("%)");
                            if (start > 0 && end > start)
                            {
                                var progressStr = e.Data.Substring(start, end - start);
                                if (int.TryParse(progressStr, out int progress))
                                {
                                    progress = Math.Min(progress, 100);
                                    downloadItem.Progress = progress;
                                    downloadItem.Status = $"下载中({progress}%)";
                                    DownloadProgressChanged?.Invoke(downloadItem);
                                }
                            }
                        }
                    }
                };

                currentProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {

                        downloadItem.Status = $"错误: {e.Data}";
                        DownloadProgressChanged?.Invoke(downloadItem);
                    }
                };

                currentProcess.Exited += (sender, e) =>
                {
                    var process = currentProcess;
                    if (process == null) return;
                    
                    var result = GetProcessResult(process);
                    
                    if (result.ExitCode == 0)
                    {
                        // 最终状态强制更新
                        downloadItem.Progress = 100;
                        downloadItem.Status = "下载完成 (100%)";
                        
                        // 验证文件完整性
                        string downloadPath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            "Downloads",
                            downloadItem.FileName);
                            
                        if (File.Exists(downloadPath))
                        {

                        }
                        else
                        {

                        }
                        
                        // 触发界面更新
                        DownloadProgressChanged?.Invoke(downloadItem);
                        DownloadCompleted?.Invoke(downloadItem);
                        downloadItem.UpdateDisplay();
                        
                        try 
                        {
                            // 双重确保在主线程显示提示
                            if (Application.OpenForms.Count > 0)
                            {
                                var mainForm = Application.OpenForms[0];
                                mainForm.Invoke((MethodInvoker)delegate {
                            MessageBox.Show(mainForm, 
                                $"文件 {downloadItem.FileName} 已成功下载到：\n{Path.Combine(downloadsDir, downloadItem.FileName)}", 
                                "下载完成", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
                                });
                            }
                            else
                            {

                            }
                        }
                        catch 
                        {
                        }
                    }
                    else
                    {
                        downloadItem.Status = $"下载失败 (代码: {result.ExitCode})";
                        MessageBox.Show($"文件 {downloadItem.FileName} 下载失败", "下载失败", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                    DownloadCompleted?.Invoke(downloadItem);
                    
                    try
                    {
                        process?.Dispose();
                    }
                    finally
                    {
                        if (process != null)
                        {
                            currentProcess = null;
                        }
                    }
                    
                    // 强制更新显示
                    downloadItem.UpdateDisplay();
                };



                if (!currentProcess.Start())
                {
                    throw new Exception("进程启动失败");
                }

                currentProcess.BeginOutputReadLine();
                currentProcess.BeginErrorReadLine();
                progressTimer.Start();
            }
                catch (Exception ex)
                {
                    string errorDetails = $"下载错误: {ex.Message}\n";
                    errorDetails += $"目标路径: {downloadsDir}\n";
                    errorDetails += $"URL: {url}";
                    
                    downloadItem.Status = $"下载失败: {ex.Message}";
                    Logger.LogError(errorDetails, ex);
                    
                    MessageBox.Show($"下载失败:\n{errorDetails}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    DownloadCompleted?.Invoke(downloadItem);
                }
        }

        public void CancelDownload(DownloadItem item)
        {
            try
            {
                var process = currentProcess;
                if (process?.StartInfo == null || process.HasExited)
                {
                    item.Status = "已取消";
                    DownloadProgressChanged?.Invoke(item);
                    return;
                }

                process.Kill();
                process.Dispose();
                currentProcess = null;
                
                item.Status = "已取消";
                DownloadProgressChanged?.Invoke(item);
            }
            catch (Exception ex)
            {
                item.Status = $"取消失败: {ex.Message}";
                DownloadProgressChanged?.Invoke(item);
            }
        }

        private string GetDownloadPath()
        {
            string fallbackPath = string.Empty;
            // 1. 优先读取用户设置的下载路径
            try
            {
                string jsonPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "zsyg", "kortapp-z", ".date", "dl_path", "download_path.json");
                
                Logger.Log($"尝试读取下载路径配置文件: {jsonPath}");
                
                if (File.Exists(jsonPath))
                {
                    string jsonString = File.ReadAllText(jsonPath);
                    Logger.Log($"配置文件内容: {jsonString}");
                    
                    var jsonData = JsonSerializer.Deserialize<JsonElement>(jsonString);
                    string customPath = jsonData.GetProperty("DownloadPath").GetString()?.Trim();
                    
                    if (!string.IsNullOrWhiteSpace(customPath))
                    {
                        Logger.Log($"读取到自定义路径: {customPath}");
                        
                        // 处理路径格式
                        customPath = customPath.Replace(@"\\", @"\");
                        try 
                        {
                            // 处理路径中的环境变量和特殊字符
                            customPath = Environment.ExpandEnvironmentVariables(customPath);
                            customPath = Path.GetFullPath(customPath);
                            Logger.Log($"标准化后的路径: {customPath}");
                            
                            // 确保路径以目录分隔符结尾
                            if (!customPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                            {
                                customPath += Path.DirectorySeparatorChar;
                            }

                            // 验证驱动器是否存在
                            string drive = Path.GetPathRoot(customPath);
                            if (!Directory.Exists(drive))
                            {
                                Logger.LogError($"驱动器不存在: {drive}");
                                throw new Exception($"驱动器 {drive} 不存在");
                            }

                            // 验证路径
                            if (!Directory.Exists(customPath))
                            {
                                Logger.Log($"创建目录: {customPath}");
                                Directory.CreateDirectory(customPath);
                            }
                            
                            // 更严格的路径可写性测试
                            string testFile = Path.Combine(customPath, $"write_test_{Guid.NewGuid()}.tmp");
                            Logger.Log($"测试路径可写性: {testFile}");
                            try
                            {
                                File.WriteAllText(testFile, DateTime.Now.ToString());
                                string content = File.ReadAllText(testFile);
                                File.Delete(testFile);
                                Logger.Log($"路径验证成功: {customPath}");
                                return customPath.TrimEnd(Path.DirectorySeparatorChar);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError($"路径不可写: {customPath}", ex);
                                throw new Exception($"路径不可写: {customPath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"路径处理失败: {customPath}", ex);
                            throw;
                        }
                    }
                }
                else
                {
                    Logger.Log("未找到下载路径配置文件");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("读取自定义下载路径失败", ex);
            }

            // 2. 回退到系统默认下载路径
            IntPtr pathPtr = IntPtr.Zero;
            try
            {
                var downloadsFolderGuid = new Guid("374DE290-123F-4565-9164-39C4925E467B");
                if (SHGetKnownFolderPath(downloadsFolderGuid, 0, IntPtr.Zero, out pathPtr) == 0)
                {
                    string? defaultPath = Marshal.PtrToStringUni(pathPtr);
                    if (!string.IsNullOrEmpty(defaultPath))
                    {
                        Directory.CreateDirectory(defaultPath);
                        return defaultPath;
                    }
                    else
                    {
                        Logger.LogWarning("获取到的系统下载路径为空");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("获取系统下载路径失败", ex);
            }
            finally
            {
                if (pathPtr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pathPtr);
                }
            }

            // 3. 最终回退到相对路径 ~/Downloads
            string relativePath = "~/Downloads";
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) ?? 
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) ?? 
                AppDomain.CurrentDomain.BaseDirectory;
            
            if (!string.IsNullOrEmpty(userProfile))
            {
                fallbackPath = relativePath.Replace("~", userProfile);
                if (!string.IsNullOrEmpty(fallbackPath))
                {
                    fallbackPath = Path.GetFullPath(fallbackPath);
                }
            }
            
            try {
                Directory.CreateDirectory(fallbackPath);
                // 测试路径可写性
                string testFile = Path.Combine(fallbackPath, "write_test.tmp");
                if (!string.IsNullOrEmpty(testFile))
                {
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                }
                return fallbackPath;
            }
            catch {
                throw new Exception($"无法使用默认下载路径: {fallbackPath}");
            }
        }
    }
}
