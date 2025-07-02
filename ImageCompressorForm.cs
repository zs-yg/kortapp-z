 // _              _                             
 //| | _____  _ __| |_ __ _ _ __  _ __       ____
 //| |/ / _ \| '__| __/ _` | '_ \| '_ \ ____|_  /
 //|   | (_) | |  | || (_| | |_) | |_) |_____/ / 
 //|_|\_\___/|_|   \__\__,_| .__/| .__/     /___|
 //                        |_|   |_|             
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AppStore
{
    public class ImageCompressorForm : Form
    {
        private Button btnSelectInput = new Button();
        private Button btnSelectOutput = new Button();
        private Button btnCompress = new Button();
        private TextBox txtInput = new TextBox();
        private TextBox txtOutput = new TextBox();
        private RadioButton rbLossy = new RadioButton();
        private RadioButton rbLossless = new RadioButton();
        private TrackBar tbQuality = new TrackBar();
        private Label lblQuality = new Label();
        private CheckBox cbKeepExif = new CheckBox();
        private ProgressBar progressBar = new ProgressBar();

        public ImageCompressorForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "图片压缩工具";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 输入文件选择
            btnSelectInput.Text = "选择...";
            btnSelectInput.Location = new Point(400, 20);
            btnSelectInput.Click += (s, e) => SelectFile(txtInput);
            this.Controls.Add(btnSelectInput);

            txtInput.Location = new Point(20, 20);
            txtInput.Size = new Size(370, 20);
            txtInput.ReadOnly = true;
            this.Controls.Add(txtInput);

            Label lblInput = new Label();
            lblInput.Text = "输入文件:";
            lblInput.Location = new Point(20, 0);
            this.Controls.Add(lblInput);

            // 输出文件选择
            btnSelectOutput.Text = "选择...";
            btnSelectOutput.Location = new Point(400, 70);
            btnSelectOutput.Click += (s, e) => SelectFile(txtOutput, true);
            this.Controls.Add(btnSelectOutput);

            txtOutput.Location = new Point(20, 70);
            txtOutput.Size = new Size(370, 20);
            this.Controls.Add(txtOutput);

            Label lblOutput = new Label();
            lblOutput.Text = "输出文件:";
            lblOutput.Location = new Point(20, 50);
            this.Controls.Add(lblOutput);

            // 压缩类型
            rbLossy.Text = "有损压缩 (JPEG)";
            rbLossy.Location = new Point(20, 110);
            rbLossy.Checked = true;
            this.Controls.Add(rbLossy);

            rbLossless.Text = "无损压缩 (PNG)";
            rbLossless.Location = new Point(20, 135);
            this.Controls.Add(rbLossless);

            // 质量设置
            tbQuality.Minimum = 1;
            tbQuality.Maximum = 1000;
            tbQuality.Value = 800;
            tbQuality.Location = new Point(20, 190);
            tbQuality.Size = new Size(300, 50);
            tbQuality.Scroll += (s, e) => lblQuality.Text = $"压缩质量: {tbQuality.Value}";
            this.Controls.Add(tbQuality);

            lblQuality.Text = $"压缩质量: {tbQuality.Value}";
            lblQuality.Location = new Point(20, 170);
            this.Controls.Add(lblQuality);

            // EXIF选项
            cbKeepExif.Text = "保留EXIF信息";
            cbKeepExif.Location = new Point(20, 240);
            this.Controls.Add(cbKeepExif);

            // 压缩按钮
            btnCompress.Text = "开始压缩";
            btnCompress.Location = new Point(20, 280);
            btnCompress.Size = new Size(460, 30);
            btnCompress.Click += BtnCompress_Click;
            this.Controls.Add(btnCompress);

            // 调整窗体大小
            this.Size = new Size(500, 370);
        }

        private void SelectFile(TextBox target, bool isSave = false)
        {
            var dialog = isSave ? new SaveFileDialog() : new OpenFileDialog() as FileDialog;
            dialog.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp|所有文件|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                target.Text = dialog.FileName;
            }
        }

        private void BtnCompress_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text) || !File.Exists(txtInput.Text))
            {
                MessageBox.Show("请选择有效的输入文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtOutput.Text))
            {
                MessageBox.Show("请指定输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnCompress.Enabled = false;

            try
            {
                string toolPath = Path.Combine(Application.StartupPath, "resource", "image_compressor.exe");
                if (!File.Exists(toolPath))
                {
                    MessageBox.Show("图片压缩工具未找到", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string args = $"\"{txtInput.Text}\" \"{txtOutput.Text}\"";
                args += $" -t {(rbLossy.Checked ? "lossy" : "lossless")}";
                args += $" -q {tbQuality.Value}";
                if (cbKeepExif.Checked) args += " -e";

                var process = new Process();
                process.StartInfo.FileName = toolPath;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += (s, ev) => {
                    if (!string.IsNullOrEmpty(ev.Data))
                        Console.WriteLine(ev.Data);
                };

                process.ErrorDataReceived += (s, ev) => {
                    if (!string.IsNullOrEmpty(ev.Data))
                        Console.Error.WriteLine(ev.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    MessageBox.Show("图片压缩完成", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("图片压缩失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"压缩过程中发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCompress.Enabled = true;
                progressBar.Visible = false;
            }
        }
    }
}
