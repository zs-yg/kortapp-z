using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AppStore
{
    public class DownloadItem : UserControl
    {
        private Label nameLabel;
        private ProgressBar progressBar;
        private Label statusLabel;
        private Button cancelBtn;

        public string FileName { get; set; } = string.Empty;
        public int Progress { get; set; }
        public string Status { get; set; } = string.Empty;
        public Process? DownloadProcess { get; set; }

        public DownloadItem()
        {
            nameLabel = new Label();
            progressBar = new ProgressBar();
            statusLabel = new Label();
            cancelBtn = new Button();
            
            InitializeComponent();
            
            // 监听主题变化
            ThemeManager.ThemeChanged += (theme) => {
                this.Invoke((MethodInvoker)delegate {
                    ApplyTheme();
                });
            };
        }

        private void ApplyTheme()
        {
            this.BackColor = ThemeManager.CurrentTheme == ThemeManager.ThemeMode.Light 
                ? Color.White 
                : Color.Black;
            this.ForeColor = ThemeManager.CurrentTheme == ThemeManager.ThemeMode.Light 
                ? Color.Black 
                : Color.White;
            
            cancelBtn.BackColor = ThemeManager.CurrentTheme == ThemeManager.ThemeMode.Light 
                ? SystemColors.Control 
                : Color.FromArgb(70, 70, 70);
            cancelBtn.ForeColor = ThemeManager.TextColor;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 60);
            this.BackColor = ThemeManager.CurrentTheme == ThemeManager.ThemeMode.Light 
                ? Color.White 
                : Color.Black;
            this.BorderStyle = BorderStyle.None; // 禁用默认边框
            this.ForeColor = ThemeManager.CurrentTheme == ThemeManager.ThemeMode.Light 
                ? Color.Black 
                : Color.White;
            this.Paint += DownloadItem_Paint; // 添加自定义绘制

            // 文件名标签
            nameLabel = new Label();
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(10, 10);
            nameLabel.Font = new Font("Microsoft YaHei", 9, FontStyle.Bold);
            this.Controls.Add(nameLabel);

            // 进度条
            progressBar = new ProgressBar();
            progressBar.Size = new Size(200, 20);
            progressBar.Location = new Point(10, 30);
            this.Controls.Add(progressBar);

            // 状态标签
            statusLabel = new Label();
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(220, 30);
            statusLabel.Font = new Font("Microsoft YaHei", 8);
            this.Controls.Add(statusLabel);

            // 取消按钮
            cancelBtn = new Button();
            cancelBtn.Text = "取消";
            cancelBtn.Size = new Size(60, 25);
            cancelBtn.Location = new Point(320, 30);
            cancelBtn.BackColor = ThemeManager.CurrentTheme == ThemeManager.ThemeMode.Light 
                ? SystemColors.Control 
                : Color.FromArgb(70, 70, 70);
            cancelBtn.ForeColor = ThemeManager.TextColor;
            cancelBtn.FlatStyle = FlatStyle.Flat;
            cancelBtn.FlatAppearance.BorderSize = 0;
            cancelBtn.Click += CancelBtn_Click;
            this.Controls.Add(cancelBtn);
        }

        public void UpdateDisplay()
        {
            nameLabel.Text = FileName;
            progressBar.Value = Progress;
            statusLabel.Text = Status;
            this.Invalidate(); // 触发重绘
        }

        private void DownloadItem_Paint(object sender, PaintEventArgs e)
        {
            // 自定义边框绘制
            using (var pen = new Pen(ThemeManager.BorderColor, 1))
            {
                e.Graphics.DrawRectangle(pen, 
                    new Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            if (sender == null || e == null) return;
            if (InvokeRequired)
            {
                Invoke(new EventHandler(CancelBtn_Click), sender, e);
                return;
            }

            try
            {
                // 1. 先取消下载
                DownloadManager.Instance.CancelDownload(this);
                
                // 2. 更新状态为已取消
                Status = "已取消";
                UpdateDisplay();
                
                // 3. 延迟100ms后移除控件，确保UI更新完成
                var timer = new System.Windows.Forms.Timer { Interval = 100 };
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    if (this.Parent != null)
                    {
                        this.Parent.Controls.Remove(this);
                        this.Dispose();
                    }
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                Status = $"取消失败: {ex.Message}";
                UpdateDisplay();
            }
        }
    }
}
