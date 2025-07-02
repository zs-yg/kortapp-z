using System;
using System.Drawing;
using System.Windows.Forms;
using AppStore;

namespace AppStore
{
    public class CalculatorForm : Form
    {
        private TextBox display = new TextBox();
        private double firstNumber = 0;
        private string operation = "";
        
        public CalculatorForm()
        {
            this.Text = "计算器";
            this.Size = new Size(300, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            CreateControls();
        }

        private void CreateControls()
        {
            // 显示框
            display = new TextBox();
            display.ReadOnly = true;
            display.TextAlign = HorizontalAlignment.Right;
            display.Font = new Font("Microsoft YaHei", 14);
            display.Size = new Size(260, 40);
            display.Location = new Point(20, 20);
            this.Controls.Add(display);

            // 按钮布局
            string[] buttonLabels = {
                "7", "8", "9", "/",
                "4", "5", "6", "*", 
                "1", "2", "3", "-",
                "0", ".", "=", "+"
            };

            for (int i = 0; i < buttonLabels.Length; i++)
            {
                var btn = new Button();
                btn.Text = buttonLabels[i];
                btn.Size = new Size(60, 50);
                btn.Location = new Point(20 + (i % 4) * 65, 70 + (i / 4) * 55);
                btn.Font = new Font("Microsoft YaHei", 12);
                btn.Click += ButtonClickHandler;
                this.Controls.Add(btn);
            }

            // 清除按钮
            var clearBtn = new Button();
            clearBtn.Text = "C";
            clearBtn.Size = new Size(260, 40);
            clearBtn.Location = new Point(20, 300);
            clearBtn.Font = new Font("Microsoft YaHei", 12);
            clearBtn.Click += (s, e) => {
                display.Text = "";
                firstNumber = 0;
                operation = "";
            };
            this.Controls.Add(clearBtn);
        }

        private void ButtonClickHandler(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            switch (button.Text)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                    if (!string.IsNullOrEmpty(display.Text))
                    {
                        firstNumber = double.Parse(display.Text);
                        operation = button.Text;
                        display.Text = "";
                    }
                    break;
                    
                case "=":
                    if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(display.Text))
                    {
                        double secondNumber = double.Parse(display.Text);
                        double result = 0;
                        
                        try
                        {
                            result = operation switch
                            {
                                "+" => AppStore.Calculator.Add(firstNumber, secondNumber),
                                "-" => AppStore.Calculator.Subtract(firstNumber, secondNumber),
                                "*" => AppStore.Calculator.Multiply(firstNumber, secondNumber),
                                "/" => AppStore.Calculator.Divide(firstNumber, secondNumber),
                                _ => 0
                            };
                            
                            display.Text = result.ToString();
                        }
                        catch (DivideByZeroException)
                        {
                            MessageBox.Show("除数不能为零", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        operation = "";
                    }
                    break;
                    
                default:
                    display.Text += button.Text;
                    break;
            }
        }
    }
}
