using System;
using System.Drawing;
using System.Windows.Forms;

namespace AppStore
{
    public class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "关于 kortapp-z";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 添加应用图标
            PictureBox logo = new PictureBox();
            try
            {
                logo.Image = Image.FromFile("img/png/kortapp-z.png");
                logo.SizeMode = PictureBoxSizeMode.Zoom;
                logo.Width = 200;
                logo.Height = 200;
                logo.Location = new Point((this.Width - logo.Width) / 2, 30);
                this.Controls.Add(logo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法加载应用图标: {ex.Message}");
            }

            // 添加应用信息
            Label infoLabel = new Label();
            infoLabel.Text = "kortapp-z\n版本: 0.9.5\n一个简单、开源的应用商店\nkortapp-z是完全免费的基于.NET8和C++的软件";
            infoLabel.Font = new Font("Microsoft YaHei", 12);
            infoLabel.AutoSize = false;
            infoLabel.Width = this.ClientSize.Width - 40;
            infoLabel.Height = 100;
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Location = new Point(
                20,
                logo.Bottom + 20
            );
            this.Controls.Add(infoLabel);

            // 添加关闭按钮
            Button closeButton = new Button();
            closeButton.Text = "关闭";
            closeButton.Width = 100;
            closeButton.Height = 40;
            closeButton.Location = new Point(
                (this.Width - closeButton.Width) / 2,
                infoLabel.Bottom + 30
            );
            closeButton.Click += (s, e) => this.Close();
            this.Controls.Add(closeButton);
        }
    }
}
