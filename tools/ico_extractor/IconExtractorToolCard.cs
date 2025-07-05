using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace AppStore.Tools.IconExtractor
{
    public class IconExtractorToolCard : ToolCard
    {
        public IconExtractorToolCard()
        {
            ToolName = "图标提取器";
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "QRcode.png");
                if (File.Exists(iconPath))
                {
                    ToolIcon = Image.FromFile(iconPath);
                }
                else
                {
                    ToolIcon = SystemIcons.Application.ToBitmap();
                }
            }
            catch
            {
                ToolIcon = SystemIcons.Application.ToBitmap();
            }
            this.ToolCardClicked += OnIconExtractorCardClicked;
            UpdateDisplay();
        }

        private void OnIconExtractorCardClicked(object sender, EventArgs e)
        {
            var iconExtractorForm = new IconExtractorForm();
            iconExtractorForm.ShowDialog();
        }
    }
}
