 // _              _                             
 //| | _____  _ __| |_ __ _ _ __  _ __       ____
 //| |/ / _ \| '__| __/ _` | '_ \| '_ \ ____|_  /
 //|   | (_) | |  | || (_| | |_) | |_) |_____/ / 
 //|_|\_\___/|_|   \__\__,_| .__/| .__/     /___|
 //                        |_|   |_|             
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
            this.BackColor = ThemeManager.BackgroundColor;
            this.Padding = new Padding(20);

            // 创建主布局面板
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.BackColor = ThemeManager.BackgroundColor;
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
            infoLabel.Text = "kortapp-z\n版本: 1.2.4\n作者: zs-yg\n一个简单、开源的应用商店\nkortapp-z是完全免费\n基于.NET8和C/C++的软件";
            infoLabel.Font = new Font("Microsoft YaHei", 12);
            infoLabel.AutoSize = false;
            infoLabel.Width = 300;
            infoLabel.Height = 130;  // 增加高度确保文字完整显示
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Anchor = AnchorStyles.None;
            mainLayout.Controls.Add(infoLabel, 0, 1);

            // 调整主布局为3行
            mainLayout.RowCount = 3;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // 在底部添加GitHub链接区域
            TableLayoutPanel githubPanel = new TableLayoutPanel();
            githubPanel.Dock = DockStyle.Bottom;
            githubPanel.BackColor = ThemeManager.BackgroundColor;
            githubPanel.Height = 60;
            githubPanel.ColumnCount = 3;
            githubPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            githubPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            githubPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            githubPanel.RowCount = 1;
            githubPanel.Padding = new Padding(10);
            
            // 添加GitHub图标
            PictureBox githubIcon = new PictureBox();
            try
            {
                githubIcon.Image = Image.FromFile("img/jpg/github.jpg");
            }
            catch (Exception ex)
            {
                Logger.LogError($"无法加载GitHub图标: {ex.Message}");
                githubIcon.Image = SystemIcons.Application.ToBitmap();
            }
            githubIcon.SizeMode = PictureBoxSizeMode.Zoom;
            githubIcon.Width = 30;
            githubIcon.Height = 30;
            githubIcon.Cursor = Cursors.Hand;
            githubIcon.Click += (s, e) => {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/zs-yg/kortapp-z",
                    UseShellExecute = true
                });
            };
            
            // 添加文字说明
            Label githubLabel = new Label();
            githubLabel.Text = "🤗🤗🤗开源地址 🌟 欢迎点star和提交pr 🚀";
            githubLabel.Font = new Font("Microsoft YaHei", 10);
            githubLabel.AutoSize = true;
            githubLabel.Margin = new Padding(10, 0, 0, 0);
            
            // 创建包含图标和文字的面板
            Panel linkPanel = new Panel();
            linkPanel.AutoSize = true;
            linkPanel.BackColor = ThemeManager.BackgroundColor;
            linkPanel.Controls.Add(githubIcon);
            linkPanel.Controls.Add(githubLabel);
            githubIcon.Location = new Point(0, 0);
            githubLabel.Location = new Point(githubIcon.Width + 10, 5);
            
            // 将链接面板添加到中间列
            githubPanel.Controls.Add(linkPanel, 1, 0);
            
            this.Controls.Add(mainLayout);
            this.Controls.Add(githubPanel);
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
