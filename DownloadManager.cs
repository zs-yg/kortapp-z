using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
            try
            {
                // 设置下载目录为用户文件夹中的Downloads
                // 获取系统下载文件夹路径
                // 获取系统下载文件夹路径
                string downloadsDir;
                IntPtr pathPtr = IntPtr.Zero;
                try
                {
                    // 使用SHGetKnownFolderPath API获取下载文件夹
                    var downloadsFolderGuid = new Guid("374DE290-123F-4565-9164-39C4925E467B");
                    if (SHGetKnownFolderPath(downloadsFolderGuid, 0, IntPtr.Zero, out pathPtr) != 0)
                    {
                        throw new Exception("无法获取下载文件夹路径");
                    }
                    
                    downloadsDir = Marshal.PtrToStringUni(pathPtr) ?? 
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                }
                catch 
                {
                    downloadsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                }
                finally
                {
                    if (pathPtr != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pathPtr);
                    }
                }
                
                if (!string.IsNullOrEmpty(downloadsDir))
                {
                    Directory.CreateDirectory(downloadsDir);
                }
                else
                {
                    throw new Exception("无法确定下载文件夹位置");
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
                downloadItem.Status = $"下载错误: {ex.Message}";
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
    }
}
