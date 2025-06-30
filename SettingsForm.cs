using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

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

            ThemeManager.ThemeChanged += OnThemeChanged;
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
    }
}
