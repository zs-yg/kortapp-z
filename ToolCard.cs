using System;
using System.Drawing;
using System.Windows.Forms;

namespace AppStore
{
    public class ToolCard : UserControl
    {
        private PictureBox iconBox = new PictureBox();
        private Label nameLabel = new Label();
        
        public string ToolName { get; set; } = string.Empty;
        public Image ToolIcon { get; set; } = SystemIcons.Shield.ToBitmap();

        // 自定义点击事件初始化为空委托
        public event EventHandler ToolCardClicked = delegate {};

        public ToolCard()
        {
            // 启用双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                        ControlStyles.ResizeRedraw, true);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(240, 220);
            this.BackColor = Color.White;
            this.Padding = new Padding(5);
            this.BorderStyle = BorderStyle.FixedSingle;
            
            // 工具图标
            iconBox = new PictureBox();
            iconBox.Size = new Size(80, 80);
            iconBox.Location = new Point((Width - 80) / 2, 15);
            iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(iconBox);

            // 工具名称
            nameLabel = new Label();
            nameLabel.AutoSize = false;
            nameLabel.Size = new Size(Width - 20, 30);
            nameLabel.Location = new Point(10, 100);
            nameLabel.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            nameLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(nameLabel);

            // 打开按钮
            var openButton = new Button();
            openButton.Text = "打开工具";
            openButton.Size = new Size(100, 30);
            openButton.Location = new Point((Width - 100) / 2, 140);
            openButton.BackColor = Color.FromArgb(0, 120, 215);
            openButton.ForeColor = Color.White;
            openButton.FlatStyle = FlatStyle.Flat;
            openButton.FlatAppearance.BorderSize = 0;
            openButton.Cursor = Cursors.Hand;
            // 按钮点击直接触发ToolCardClicked事件
            openButton.Click += (s, e) => {
                ToolCardClicked?.Invoke(this, e);
            };
            this.Controls.Add(openButton);

            // 设置按钮悬停效果
            openButton.BackColor = Color.FromArgb(0, 120, 215);
            openButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 100, 180);
            openButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 80, 160);
        }

        public void UpdateDisplay()
        {
            nameLabel.Text = ToolName;
            iconBox.Image = ToolIcon;
        }
    }
}
