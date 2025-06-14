using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AppStore
{
    public class SettingsForm : Form
    {
        private Button btnCleanLogs;

        public SettingsForm()
        {
            this.Text = "设置";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            btnCleanLogs = new Button();
            btnCleanLogs.Text = "清理日志";
            btnCleanLogs.Size = new Size(150, 40);
            btnCleanLogs.Location = new Point(120, 100);
            btnCleanLogs.Font = new Font("Microsoft YaHei", 10);
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
