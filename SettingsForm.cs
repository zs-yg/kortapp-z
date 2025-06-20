using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AppStore
{
    public class SettingsUserControl : UserControl
    {
        private Button btnCleanLogs;

        public SettingsUserControl()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            
            // 设置顶部内边距
            this.Padding = new Padding(0, 30, 0, 0);
            
            btnCleanLogs = new Button();
            btnCleanLogs.Text = "清理日志";
            btnCleanLogs.Size = new Size(150, 40);
            btnCleanLogs.Location = new Point((this.Width - 150) / 2, 50); // 调整Y坐标为50靠近顶部
            btnCleanLogs.Font = new Font("Microsoft YaHei", 10);
            btnCleanLogs.Anchor = AnchorStyles.Top; // 添加顶部锚点
            btnCleanLogs.Click += (s, e) => CleanLogs();
            this.Controls.Add(btnCleanLogs);
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
