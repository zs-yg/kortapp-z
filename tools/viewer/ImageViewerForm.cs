using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using KortAppZ.Tools.Viewer;

namespace KortAppZ.Tools.Viewer
{
    public class ImageViewerForm : Form
    {
        private Button? openButton;
        
        public ImageViewerForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.openButton = new Button();
            // 
            // openButton
            // 
            this.openButton.Text = "打开图片";
            this.openButton.Size = new Size(120, 40);
            this.openButton.Font = new Font("Microsoft YaHei", 10);
            this.openButton.Anchor = AnchorStyles.None;
            this.openButton.Click += new EventHandler(OpenButton_Click);
            
            // 初始位置
            CenterOpenButton();
            
            // 窗体大小变化时重新居中按钮
            this.Resize += (s, e) => CenterOpenButton();
            
            // 
            // ImageViewerForm
            // 
            this.Text = "图片查看器";
            this.ClientSize = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(300, 200);
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.Controls.Add(this.openButton);
        }
        
        private void CenterOpenButton()
        {
            if (openButton != null)
            {
                openButton.Location = new Point(
                    (this.ClientSize.Width - openButton.Width) / 2,
                    (this.ClientSize.Height - openButton.Height) / 2);
            }
        }
        
        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff;*.tif|所有文件|*.*";
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ShowImage(filePath);
            }
        }
        
        private void ShowImage(string filePath)
        {
            try
            {
                if (!ImageFileHandler.IsImageFile(filePath))
                {
                    MessageBox.Show("不支持的图片格式", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                ImageDisplayForm displayForm = new ImageDisplayForm();
                displayForm.LoadImage(filePath);
                displayForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法加载图片: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
