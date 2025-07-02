using System;
using System.Drawing;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace AppStore
{
    public class QrCodeGeneratorForm : Form
    {
        private TextBox? inputTextBox;
        private PictureBox? qrCodePictureBox;
        private Button? generateButton;
        private Button? saveButton;

        public QrCodeGeneratorForm()
        {
            InitializeComponent();
            this.Text = "二维码生成器";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void InitializeComponent()
        {
            // 输入文本框
            inputTextBox = new TextBox();
            inputTextBox.Multiline = true;
            inputTextBox.ScrollBars = ScrollBars.Vertical;
            inputTextBox.Size = new Size(400, 100);
            inputTextBox.Location = new Point(50, 30);
            inputTextBox.PlaceholderText = "请输入要生成二维码的文本内容...";
            this.Controls.Add(inputTextBox);

            // 生成按钮
            generateButton = new Button();
            generateButton.Text = "生成二维码";
            generateButton.Size = new Size(150, 40);
            generateButton.Location = new Point(50, 150);
            generateButton.Click += GenerateButton_Click;
            this.Controls.Add(generateButton);

            // 二维码显示区域
            qrCodePictureBox = new PictureBox();
            qrCodePictureBox.Size = new Size(300, 300);
            qrCodePictureBox.Location = new Point(100, 220);
            qrCodePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            qrCodePictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(qrCodePictureBox);

            // 保存按钮
            saveButton = new Button();
            saveButton.Text = "保存二维码";
            saveButton.Size = new Size(150, 40);
            saveButton.Location = new Point(300, 150);
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputTextBox?.Text))
            {
                MessageBox.Show("请输入要生成二维码的文本内容", "提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions
                    {
                        Width = 300,
                        Height = 300,
                        Margin = 1
                    }
                };

                var pixelData = writer.Write(inputTextBox.Text);
                var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), 
                    System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                qrCodePictureBox!.Image = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成二维码失败: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (qrCodePictureBox?.Image == null)
            {
                MessageBox.Show("请先生成二维码", "提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PNG 图片|*.png|JPEG 图片|*.jpg|BMP 图片|*.bmp";
                saveDialog.Title = "保存二维码图片";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        qrCodePictureBox!.Image.Save(saveDialog.FileName);
                        MessageBox.Show("二维码保存成功", "成功", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存二维码失败: {ex.Message}", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
