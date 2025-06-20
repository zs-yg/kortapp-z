using System;
using System.Drawing;
using System.Windows.Forms;

namespace AppStore
{
    public class AboutUserControl : UserControl
    {
        private PictureBox logo = null!;
        private Label infoLabel = null!;

        public AboutUserControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.Padding = new Padding(20);

            // 创建主布局面板
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.Padding = new Padding(0, 20, 0, 20);

            // 初始化并添加应用图标
            logo = new PictureBox();
            try
            {
                logo.Image = Image.FromFile("img/png/kortapp-z.png");
            }
            catch (Exception ex)
            {
                Logger.LogError($"无法加载应用图标: {ex.Message}");
                logo.Image = SystemIcons.Application.ToBitmap();
            }
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            logo.Width = 200;
            logo.Height = 200;
            logo.Anchor = AnchorStyles.None;
            logo.Margin = new Padding(0, 0, 0, 20);
            mainLayout.Controls.Add(logo, 0, 0);

            // 初始化并添加应用信息
            infoLabel = new Label();
            infoLabel.Text = "kortapp-z\n版本: 0.9.8\n一个简单、开源的应用商店\nkortapp-z是完全免费的基于.NET8和C++的软件";
            infoLabel.Font = new Font("Microsoft YaHei", 12);
            infoLabel.AutoSize = false;
            infoLabel.Width = 300;
            infoLabel.Height = 100;
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Anchor = AnchorStyles.None;
            mainLayout.Controls.Add(infoLabel, 0, 1);

            this.Controls.Add(mainLayout);
        }
    }

    // 保留原AboutForm作为容器(可选)
    public class AboutForm : Form
    {
        public AboutForm()
        {
            this.Text = "关于 kortapp-z";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            var aboutControl = new AboutUserControl();
            this.Controls.Add(aboutControl);
        }
    }
}
