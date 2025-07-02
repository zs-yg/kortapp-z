 // _              _                             
 //| | _____  _ __| |_ __ _ _ __  _ __       ____
 //| |/ / _ \| '__| __/ _` | '_ \| '_ \ ____|_  /
 //|   | (_) | |  | || (_| | |_) | |_) |_____/ / 
 //|_|\_\___/|_|   \__\__,_| .__/| .__/     /___|
 //                        |_|   |_|             
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Text.Json;

namespace AppStore
{
    public class SettingsUserControl : UserControl
    {
        private Button btnCleanLogs;
        private Button btnLightTheme;
        private Button btnDarkTheme;

        public SettingsUserControl()
        {
            this.Dock = DockStyle.Fill;
            ThemeManager.ApplyTheme(this);
            
            // 设置顶部内边距
            this.Padding = new Padding(0, 30, 0, 0);
            
            // 主题切换按钮
            btnLightTheme = new Button();
            btnLightTheme.Text = "浅色模式";
            btnLightTheme.Size = new Size(150, 40);
            btnLightTheme.Location = new Point((this.Width - 320) / 2, 50);
            btnLightTheme.Font = new Font("Microsoft YaHei", 10);
            btnLightTheme.Anchor = AnchorStyles.Top;
            btnLightTheme.Click += (s, e) => SwitchTheme(ThemeManager.ThemeMode.Light);
            this.Controls.Add(btnLightTheme);

            btnDarkTheme = new Button();
            btnDarkTheme.Text = "深色模式";
            btnDarkTheme.Size = new Size(150, 40);
            btnDarkTheme.Location = new Point(btnLightTheme.Right + 20, 50);
            btnDarkTheme.Font = new Font("Microsoft YaHei", 10);
            btnDarkTheme.Anchor = AnchorStyles.Top;
            btnDarkTheme.Click += (s, e) => SwitchTheme(ThemeManager.ThemeMode.Dark);
            this.Controls.Add(btnDarkTheme);
            
            // 清理日志按钮
            btnCleanLogs = new Button();
            btnCleanLogs.Text = "清理日志";
            btnCleanLogs.Size = new Size(150, 40);
            btnCleanLogs.Location = new Point((this.Width - 150) / 2, 110);
            btnCleanLogs.Font = new Font("Microsoft YaHei", 10);
            btnCleanLogs.Anchor = AnchorStyles.Top;
            btnCleanLogs.Click += (s, e) => CleanLogs();
            this.Controls.Add(btnCleanLogs);

            // 下载路径设置
            Label lblDownloadPath = new Label();
            lblDownloadPath.Text = "下载路径:";
            lblDownloadPath.AutoSize = true;
            lblDownloadPath.Location = new Point((this.Width - 300) / 2, 170);
            lblDownloadPath.Font = new Font("Microsoft YaHei", 10);
            lblDownloadPath.Anchor = AnchorStyles.Top;
            this.Controls.Add(lblDownloadPath);

            TextBox txtDownloadPath = new TextBox();
            txtDownloadPath.Size = new Size(300, 30);
            txtDownloadPath.Location = new Point((this.Width - 300) / 2, 200);
            txtDownloadPath.Font = new Font("Microsoft YaHei", 10);
            txtDownloadPath.Anchor = AnchorStyles.Top;
            txtDownloadPath.ReadOnly = true;
            this.Controls.Add(txtDownloadPath);

            Button btnBrowse = new Button();
            btnBrowse.Text = "浏览...";
            btnBrowse.Size = new Size(80, 30);
            btnBrowse.Location = new Point(txtDownloadPath.Right + 10, 200);
            btnBrowse.Font = new Font("Microsoft YaHei", 10);
            btnBrowse.Anchor = AnchorStyles.Top;
            btnBrowse.Click += (s, e) => BrowseDownloadPath(txtDownloadPath);
            this.Controls.Add(btnBrowse);

            Button btnSavePath = new Button();
            btnSavePath.Text = "保存路径";
            btnSavePath.Size = new Size(100, 30);
            btnSavePath.Location = new Point((this.Width - 100) / 2, 240);
            btnSavePath.Font = new Font("Microsoft YaHei", 10);
            btnSavePath.Anchor = AnchorStyles.Top;
            btnSavePath.Click += (s, e) => SaveDownloadPath(txtDownloadPath.Text);
            this.Controls.Add(btnSavePath);

            ThemeManager.ThemeChanged += OnThemeChanged;
            LoadDownloadPath(txtDownloadPath);
        }

        private void SwitchTheme(ThemeManager.ThemeMode theme)
        {
            ThemeManager.CurrentTheme = theme;
        }

        private void OnThemeChanged(ThemeManager.ThemeMode theme)
        {
            ThemeManager.ApplyTheme(this);
        }

        private void CleanLogs()
        {
            try
            {
                string logCleanerPath = Path.Combine("resource", "log_cleaner.exe");
                
                if (File.Exists(logCleanerPath))
                {
                    Process.Start(logCleanerPath);
                    MessageBox.Show("日志清理程序已启动", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("日志清理程序未找到", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("清理日志时出错", ex);
                MessageBox.Show($"清理日志时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BrowseDownloadPath(TextBox txtBox)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "选择下载路径";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void SaveDownloadPath(string path)
        {
            try
            {
                // 验证路径
                if (string.IsNullOrWhiteSpace(path))
                {
                    MessageBox.Show("下载路径不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 尝试创建目录（如果不存在）
                try
                {
                    Directory.CreateDirectory(path);
                    
                    // 验证目录是否可写
                    string testFile = Path.Combine(path, "write_test.tmp");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                }
                catch
                {
                    MessageBox.Show($"无法访问路径: {path}\n请确保路径存在且有写入权限", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 保存路径
                string dlPathDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "zsyg", "kortapp-z", ".date", "dl_path");
                
                if (!Directory.Exists(dlPathDir))
                {
                    Directory.CreateDirectory(dlPathDir);
                }

                string jsonPath = Path.Combine(dlPathDir, "download_path.json");
                var jsonData = new { DownloadPath = path };
                string jsonString = JsonSerializer.Serialize(jsonData);
                
                File.WriteAllText(jsonPath, jsonString);
                
                MessageBox.Show($"下载路径已保存到:\n{path}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError("保存下载路径时出错", ex);
                MessageBox.Show($"保存下载路径时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDownloadPath(TextBox txtBox)
        {
            // 默认下载路径为用户文件夹下的Downloads
            string defaultPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                "Downloads");

            try
            {
                string jsonPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "zsyg", "kortapp-z", ".date", "dl_path", "download_path.json");
                
                if (File.Exists(jsonPath))
                {
                    string jsonString = File.ReadAllText(jsonPath);
                    var jsonData = JsonSerializer.Deserialize<JsonElement>(jsonString);
                    string customPath = jsonData.GetProperty("DownloadPath").GetString();
                    
                    // 如果自定义路径有效则显示，否则显示默认路径
                    txtBox.Text = !string.IsNullOrWhiteSpace(customPath) ? customPath : defaultPath;
                }
                else
                {
                    txtBox.Text = defaultPath;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("加载下载路径时出错", ex);
                txtBox.Text = defaultPath;
            }
        }
    }
}
