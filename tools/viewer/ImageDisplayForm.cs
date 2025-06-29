using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace KortAppZ.Tools.Viewer
{
    public class ImageDisplayForm : Form
    {
        private PictureBox? pictureBox;
        private Image? currentImage;
        
        public ImageDisplayForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.pictureBox = new PictureBox();
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = DockStyle.Fill;
            this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox.BackColor = Color.Black;
            // 
            // ImageDisplayForm
            // 
            this.Text = "图片查看";
            this.ClientSize = new Size(800, 600);
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = true;
            this.ShowIcon = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.Controls.Add(this.pictureBox);
            this.FormClosing += new FormClosingEventHandler(ImageDisplayForm_FormClosing);
        }
        
        public void LoadImage(string filePath)
        {
            try
            {
                if (currentImage != null)
                {
                    currentImage.Dispose();
                    currentImage = null;
                }
                
                currentImage = ImageFileHandler.LoadImage(filePath);
                pictureBox.Image = currentImage;
                this.Text = $"图片查看 - {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法加载图片: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ImageDisplayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (currentImage != null)
            {
                currentImage.Dispose();
                currentImage = null;
            }
        }
    }
}
