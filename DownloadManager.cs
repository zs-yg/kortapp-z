using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppStore
{
    public class DownloadManager
    {
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
            if (process == null) return result;
            
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
            var downloadItem = new DownloadItem
            {
                FileName = fileName,
                Progress = 0,
                Status = "准备下载"
            };
            
            DownloadItems.Add(downloadItem);
            DownloadAdded?.Invoke(downloadItem);

            Task.Run(() => DownloadFile(downloadItem, fileName, url));
        }

        private void DownloadFile(DownloadItem downloadItem, string fileName, string url)
        {
            try
            {
                // 设置下载目录为用户文件夹中的Downloads
                var downloadsDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                    "Downloads");
                Directory.CreateDirectory(downloadsDir);

                // 构建aria2c路径
                var aria2cPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource", "aria2c.exe");
                
                if (!File.Exists(aria2cPath))
                {
                    throw new FileNotFoundException($"找不到aria2c.exe: {aria2cPath}");
                }

                // 设置线程数为16并添加详细日志
                var arguments = $"--out={fileName} --dir=\"{downloadsDir}\" --split=16 --max-connection-per-server=16 {url}";
                Console.WriteLine($"下载目录: {downloadsDir}");

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
                        Console.WriteLine($"输出: {e.Data}");
                        
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
                                Console.WriteLine($"检测到文件总大小: {totalSize} bytes");
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
                        Console.WriteLine($"错误: {e.Data}");
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
                            Console.WriteLine($"文件下载完成: {downloadPath}");
                        }
                        else
                        {
                            Console.WriteLine("警告: 下载完成但文件不存在");
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
                                        $"文件 {downloadItem.FileName} 已成功下载到：\n{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", downloadItem.FileName)}", 
                                        "下载完成", 
                                        MessageBoxButtons.OK, 
                                        MessageBoxIcon.Information);
                                });
                            }
                            else
                            {
                                Console.WriteLine("下载完成提示：无法找到主窗体");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"显示下载完成提示时出错：{ex}");
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

                Console.WriteLine($"启动aria2c: {aria2cPath}");
                Console.WriteLine($"参数: {arguments}");

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
                downloadItem.Status = $"下载错误: {ex.Message}";
                Console.WriteLine($"下载错误: {ex}");
                DownloadCompleted?.Invoke(downloadItem);
            }
        }

        public void CancelDownload(DownloadItem item)
        {
            try
            {
                var process = currentProcess;
                if (process == null || process.HasExited || process.StartInfo == null)
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
                Console.WriteLine($"取消下载时出错: {ex}");
                item.Status = $"取消失败: {ex.Message}";
                DownloadProgressChanged?.Invoke(item);
            }
        }
    }
}
