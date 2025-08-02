using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace AppStore
{
    public class WebSiteCards : UserControl
    {
        private PictureBox iconBox;
        private Label nameLabel;
        private Label descriptionLabel;
        private Button visitBtn;
        private ToolTip toolTip;
        private Color borderColor = SystemColors.ControlDark;

        public string WebSiteName { get; set; } = string.Empty;
        private Image _webSiteIcon = SystemIcons.Application.ToBitmap();
        public Image WebSiteIcon 
        { 
            get { return _webSiteIcon; }
            set 
            {
                try
                {
                    if (value != null)
                    {
                        _webSiteIcon = value;
                    }
                    else
                    {
                        _webSiteIcon = SystemIcons.Application.ToBitmap();
                    }
                }
                catch
                {
                    _webSiteIcon = SystemIcons.Application.ToBitmap();
                }
                UpdateDisplay();
            }
        }

        public string WebSiteIconPath
        {
            set
            {
                try
                {
                    string path = value;
                    if (!Path.IsPathRooted(path))
                    {
                        path = Path.Combine(Application.StartupPath, path);
                    }
                    
                    if (File.Exists(path))
                    {
                        _webSiteIcon = Image.FromFile(path);
                    }
                    else
                    {
                        _webSiteIcon = SystemIcons.Application.ToBitmap();
                    }
                }
                catch
                {
                    _webSiteIcon = SystemIcons.Application.ToBitmap();
                }
                UpdateDisplay();
            }
        }
        public string WebSiteUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public WebSiteCards()
        {
            iconBox = new PictureBox() { SizeMode = PictureBoxSizeMode.StretchImage };
            nameLabel = new Label() { Text = string.Empty };
            descriptionLabel = new Label() { Text = string.Empty };
            visitBtn = new Button() { Text = "访问" };
            toolTip = new ToolTip();
            
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 150); // 更宽的卡片以适应横向布局
            this.BackColor = Color.White;
            this.Padding = new Padding(10);

            // 网站名称标签 - 顶部
            nameLabel.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;
            nameLabel.Location = new Point(10, 10);
            nameLabel.Size = new Size(Width - 20, 20);
            this.Controls.Add(nameLabel);

            // 网站图标 - 左侧
            iconBox.Size = new Size(80, 80);
            iconBox.Location = new Point(10, 40);
            this.Controls.Add(iconBox);

            // 网站描述 - 右侧
            descriptionLabel.Font = new Font("Microsoft YaHei", 9);
            descriptionLabel.TextAlign = ContentAlignment.TopLeft;
            descriptionLabel.AutoSize = false;
            descriptionLabel.Size = new Size(Width - 110, 80);
            descriptionLabel.Location = new Point(100, 40);
            descriptionLabel.MaximumSize = new Size(Width - 110, 0); // 允许自动换行
            this.Controls.Add(descriptionLabel);

            // 访问按钮 - 右下角
            visitBtn.Size = new Size(80, 30);
            visitBtn.Location = new Point(Width - 90, Height - 40);
            visitBtn.Click += VisitBtn_Click;
            this.Controls.Add(visitBtn);

            // 工具提示
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;
            toolTip.SetToolTip(visitBtn, "在浏览器中打开网站");

            UpdateDisplay();
            UpdateLabelTheme();
            ThemeManager.ThemeChanged += (theme) => UpdateLabelTheme();
        }

        private void UpdateLabelTheme()
        {
            if (ThemeManager.CurrentTheme == ThemeManager.ThemeMode.Dark)
            {
                this.BackColor = Color.FromArgb(45, 45, 48);
                nameLabel.ForeColor = Color.White;
                descriptionLabel.ForeColor = Color.White;
            }
            else
            {
                this.BackColor = Color.White;
                nameLabel.ForeColor = Color.Black;
                descriptionLabel.ForeColor = Color.Black;
            }
        }

        public void UpdateDisplay()
        {
            nameLabel.Text = WebSiteName;
            iconBox.Image = WebSiteIcon;
            descriptionLabel.Text = Description;
        }

        private void VisitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(WebSiteUrl))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = WebSiteUrl,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"打开网站失败: {ex.Message}");
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // 绘制圆角边框
            using (var pen = new Pen(borderColor, 1))
            {
                int radius = 10;
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                e.Graphics.DrawPath(pen, GetRoundedRectPath(rect, radius));
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
