using System;
using System.Drawing;
using System.Windows.Forms;

namespace AppStore
{
    public class CalculatorToolCard : ToolCard
    {
        public CalculatorToolCard()
        {
            ToolName = "计算器";
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "Calculator.png");
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
            this.ToolCardClicked += OnCalculatorCardClicked;
            UpdateDisplay();
        }

        private void OnCalculatorCardClicked(object sender, EventArgs e)
        {
            var calculatorForm = new CalculatorForm();
            calculatorForm.ShowDialog();
        }
    }
}
