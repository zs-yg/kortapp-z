using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using AppStore.Tools.IconExtractor;

namespace AppStore.Tools.IconExtractor
{
    public class IconExtractorForm : Form
    {
        private Button btnBrowse = new Button();
        private Button btnSave = new Button();
        private NumericUpDown numIconIndex = new NumericUpDown();
        private PictureBox picIcon = new PictureBox();
        private Label lblStatus = new Label();
        private TextBox txtFilePath = new TextBox();
        private ComboBox cmbIconSize = new ComboBox();
        private string currentFilePath = string.Empty;

        public IconExtractorForm()
        {
            this.Text = "图标提取器";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // 文件路径文本框
            txtFilePath.Location = new Point(20, 20);
            txtFilePath.Size = new Size(300, 25);
            txtFilePath.ReadOnly = true;
            this.Controls.Add(txtFilePath);

            // 浏览按钮
            btnBrowse.Text = "浏览...";
            btnBrowse.Location = new Point(330, 20);
            btnBrowse.Size = new Size(80, 25);
            btnBrowse.Click += BtnBrowse_Click;
            this.Controls.Add(btnBrowse);

            // 图标索引标签
            Label lblIndex = new Label();
            lblIndex.Text = "图标索引:";
            lblIndex.Location = new Point(20, 60);
            lblIndex.Size = new Size(80, 20);
            this.Controls.Add(lblIndex);

            // 图标索引选择器
            numIconIndex.Location = new Point(100, 60);
            numIconIndex.Size = new Size(80, 20);
            numIconIndex.Minimum = 0;
            numIconIndex.ValueChanged += NumIconIndex_ValueChanged;
            this.Controls.Add(numIconIndex);

            // 图标预览区域
            picIcon.Location = new Point(20, 100);
            picIcon.Size = new Size(256, 256);
            picIcon.SizeMode = PictureBoxSizeMode.Zoom;
            picIcon.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(picIcon);

            // 索引说明
            Label lblIndexHelp = new Label();
            lblIndexHelp.Text = "索引号表示文件中的第几个图标，从0开始";
            lblIndexHelp.Location = new Point(20, 240);
            lblIndexHelp.Size = new Size(300, 20);
            this.Controls.Add(lblIndexHelp);

            // 保存按钮
            btnSave.Text = "保存图标";
            btnSave.Location = new Point(20, 450);
            btnSave.Size = new Size(100, 30);
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // 状态标签
            lblStatus.Location = new Point(20, 490);
            lblStatus.Size = new Size(400, 20);
            lblStatus.Text = "请选择包含图标的文件";
            this.Controls.Add(lblStatus);
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "可执行文件|*.exe;*.dll;*.ocx;*.cpl|所有文件|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = ofd.FileName;
                    txtFilePath.Text = currentFilePath;
                    LoadIconInfo();
                }
            }
        }

        private void LoadIconInfo()
        {
            try
            {
                int iconCount = IconExtractor.GetIconCount(currentFilePath);
                numIconIndex.Maximum = Math.Max(0, iconCount - 1);
                
                // 如果是ICO文件，获取实际包含的尺寸
                if (currentFilePath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                {
                    var sizes = IconExtractor.GetIconDimensions(currentFilePath);
                    if (sizes.Count > 0)
                    {
                        cmbIconSize.Items.Clear();
                        foreach (var size in sizes)
                        {
                            cmbIconSize.Items.Add($"{size}x{size}");
                        }
                        
                        // 默认选择最接近256的尺寸
                        int closest = sizes.OrderBy(s => Math.Abs(s - 256)).First();
                        cmbIconSize.SelectedIndex = sizes.IndexOf(closest);
                    }
                }
                
                lblStatus.Text = $"找到 {iconCount} 个图标";
                ExtractAndDisplayIcon();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"错误: {ex.Message}";
                picIcon.Image = null;
            }
        }

        private void NumIconIndex_ValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                ExtractAndDisplayIcon();
            }
        }

        private void ExtractAndDisplayIcon()
        {
            try
            {
                Icon icon = IconExtractor.ExtractIconFromFile(currentFilePath, (int)numIconIndex.Value);
                picIcon.Image = icon.ToBitmap();
                lblStatus.Text = $"显示原始图标 #{numIconIndex.Value}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"提取图标失败: {ex.Message}";
                picIcon.Image = null;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (picIcon.Image == null)
            {
                MessageBox.Show("没有可保存的图标", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "图标文件|*.ico|位图文件|*.bmp|PNG文件|*.png";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Icon icon = IconExtractor.ExtractIconFromFile(currentFilePath, (int)numIconIndex.Value))
                        {
                            IconExtractor.SaveIconToFile(icon, sfd.FileName);
                            lblStatus.Text = $"已保存原始图标到 {sfd.FileName}";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存图标失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
