using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using AppStore;

namespace KortAppZ.Tools.Viewer
{
    public class ImageViewerToolCard : ToolCard
    {
        public ImageViewerToolCard()
        {
            this.ToolName = "图片查看";
            
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "ImageCompressor.png");
                if (File.Exists(iconPath))
                {
                    this.ToolIcon = Image.FromFile(iconPath);
                }
                else
                {
                    this.ToolIcon = SystemIcons.Application.ToBitmap();
                }
            }
            catch
            {
                this.ToolIcon = SystemIcons.Application.ToBitmap();
            }
            
            this.ToolCardClicked += (s, e) => {
                try
                {
                    ImageViewerForm viewerForm = new ImageViewerForm();
                    viewerForm.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"打开图片查看器失败: {ex.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            
            this.UpdateDisplay();
        }
    }
}
