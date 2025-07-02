using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Principal;

namespace AppStore
{
    public class SelfStartingManagerToolCard : ToolCard
    {
        public SelfStartingManagerToolCard()
        {
            this.ToolName = "自启动管理";
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "Self_starting_management.png");
                if (File.Exists(iconPath))
                {
                    this.ToolIcon = Image.FromFile(iconPath);
                }
                this.UpdateDisplay();
            }
            catch (Exception ex)
            {
                Logger.LogError("加载自启动管理图标失败", ex);
            }

            // 订阅点击事件
            this.ToolCardClicked += OnSelfStartingManagerClicked;
        }

        private void OnSelfStartingManagerClicked(object sender, EventArgs e)
        {
            if (!IsRunningAsAdmin())
            {
                var result = MessageBox.Show("自启动管理需要管理员权限，是否立即以管理员身份重新运行？", 
                    "权限提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if (result == DialogResult.Yes)
                {
                    RestartAsAdmin();
                }
                return;
            }

            try
            {
                var form = new SelfStartingManagerForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动自启动管理器失败: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("启动自启动管理器失败", ex);
            }
        }

        private bool IsRunningAsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void RestartAsAdmin()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(startInfo);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法以管理员身份重新运行: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogError("以管理员身份重新运行失败", ex);
            }
        }
    }
}