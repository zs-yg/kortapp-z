using System;
using System.Drawing;
using System.Windows.Forms;

namespace AppStore
{
    public class AppCard : UserControl
    {
        private PictureBox iconBox;
        private Label nameLabel;
        private Button downloadBtn;

        public string AppName { get; set; } = string.Empty;
        public Image AppIcon { get; set; } = SystemIcons.Application.ToBitmap();
        public string DownloadUrl { get; set; } = string.Empty;

        public AppCard()
        {
            iconBox = new PictureBox();
            nameLabel = new Label();
            downloadBtn = new Button();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(220, 180);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Padding = new Padding(5);

            // 应用图标
            iconBox = new PictureBox();
            iconBox.Size = new Size(64, 64);
            iconBox.Location = new Point(10, 10);
            iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(iconBox);

            // 应用名称
            nameLabel = new Label();
            nameLabel.AutoSize = false;
            nameLabel.Size = new Size(140, 60);
            nameLabel.Location = new Point(80, 15);
            nameLabel.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            nameLabel.TextAlign = ContentAlignment.TopLeft;
            this.Controls.Add(nameLabel);

            // 下载按钮
            downloadBtn = new Button();
            downloadBtn.Text = "下载";
            downloadBtn.Size = new Size(80, 30);
            downloadBtn.Location = new Point(70, 120);
            downloadBtn.BackColor = Color.FromArgb(0, 120, 215);
            downloadBtn.ForeColor = Color.White;
            downloadBtn.FlatStyle = FlatStyle.Flat;
            downloadBtn.FlatAppearance.BorderSize = 0;
            downloadBtn.Click += DownloadBtn_Click;
            this.Controls.Add(downloadBtn);
        }

        public void UpdateDisplay()
        {
            nameLabel.Text = AppName;
            iconBox.Image = AppIcon;
        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            if (sender == null || e == null) return;
            if (!string.IsNullOrEmpty(DownloadUrl))
            {
                try
                {
                    string fileName = $"{AppName.Replace(" ", "_")}.exe";
                    DownloadManager.Instance.StartDownload(fileName, DownloadUrl);
                    MessageBox.Show($"已开始下载: {AppName}", "下载中", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"下载失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
