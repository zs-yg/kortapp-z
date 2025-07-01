using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace AppStore
{
    public class AppCard : UserControl
    {
        private PictureBox iconBox;
        private Label nameLabel;
        private Panel namePanel;
        private Button downloadBtn;
        private Color borderColor = SystemColors.ControlDark;
        private static readonly ConcurrentDictionary<string, System.Drawing.Drawing2D.GraphicsPath> PathCache = 
            new ConcurrentDictionary<string, System.Drawing.Drawing2D.GraphicsPath>();

        public string AppName { get; set; } = string.Empty;
        public Image AppIcon { get; set; } = SystemIcons.Application.ToBitmap();
        public string DownloadUrl { get; set; } = string.Empty;
        public bool ShowDownloadButton { get; set; } = true;

        public AppCard()
        {
            // 确保关键对象不为null
            iconBox = new PictureBox() { SizeMode = PictureBoxSizeMode.StretchImage };
            nameLabel = new Label() { Text = string.Empty };
            namePanel = new Panel();
            downloadBtn = new Button() { Text = "下载" };
            
            // 确保DownloadManager已初始化
            var _ = DownloadManager.Instance;
            
            InitializeComponent();
        }

        private static readonly ConcurrentDictionary<string, System.Drawing.Drawing2D.GraphicsPath> BorderCache = 
            new ConcurrentDictionary<string, System.Drawing.Drawing2D.GraphicsPath>();

        private void InitializeComponent()
        {
            this.Size = new Size(240, 200);
            this.BackColor = Color.White;
            this.Padding = new Padding(10);
            
            // 异步初始化卡片路径和边框
            Task.Run(() => {
                InitializeCardPath();
                InitializeBorder();
            });

            // 应用图标
            iconBox = new PictureBox();
            iconBox.Size = new Size(80, 80);
            iconBox.Location = new Point((Width - 80) / 2, 15);
            iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(iconBox);

            // 应用名称 - 使用Panel包裹Label实现边框颜色
            // namePanel已在构造函数中初始化
            namePanel.Size = new Size(Width - 20, 40);
            namePanel.Location = new Point(10, 100);
            namePanel.Paint += (sender, e) => {
                ControlPaint.DrawBorder(e.Graphics, namePanel.ClientRectangle, 
                    borderColor, ButtonBorderStyle.Solid);
            };
            
            nameLabel = new Label();
            nameLabel.Dock = DockStyle.Fill;
            nameLabel.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            nameLabel.TextAlign = ContentAlignment.MiddleCenter;
            
            namePanel.Controls.Add(nameLabel);
            
            // 初始主题设置
            UpdateLabelTheme();
            
            // 订阅主题变化事件
            ThemeManager.ThemeChanged += (theme) => UpdateLabelTheme();
            
            this.Controls.Add(namePanel);

            // 下载按钮
            downloadBtn = new Button();
            downloadBtn.Text = "下载";
            downloadBtn.Size = new Size(100, 32);
            downloadBtn.Location = new Point((Width - 100) / 2, 150);
            downloadBtn.BackColor = Color.FromArgb(0, 120, 215);
            downloadBtn.ForeColor = Color.White;
            downloadBtn.FlatStyle = FlatStyle.Flat;
            downloadBtn.FlatAppearance.BorderSize = 0;
            downloadBtn.Cursor = Cursors.Hand;
            downloadBtn.Font = new Font("Microsoft YaHei", 9);
            
            // 按钮悬停效果
            downloadBtn.MouseEnter += (s, e) => {
                downloadBtn.BackColor = Color.FromArgb(0, 150, 255);
            };
            
            downloadBtn.MouseLeave += (s, e) => {
                downloadBtn.BackColor = Color.FromArgb(0, 120, 215);
            };
            
            downloadBtn.Click += DownloadBtn_Click;
            this.Controls.Add(downloadBtn);
            downloadBtn.Visible = ShowDownloadButton;
        }

        private void UpdateLabelTheme()
        {
            if (ThemeManager.CurrentTheme == ThemeManager.ThemeMode.Dark)
            {
                nameLabel.BackColor = Color.Black;
                nameLabel.ForeColor = Color.White;
                namePanel.BackColor = Color.Black;
                borderColor = Color.White;
            }
            else
            {
                nameLabel.BackColor = Color.White;
                nameLabel.ForeColor = Color.Black;
                namePanel.BackColor = Color.White;
                borderColor = SystemColors.ControlDark;
            }
            namePanel.Invalidate(); // 触发重绘
        }

        /// <summary>
        /// 初始化卡片边框路径
        /// 使用C++程序计算高性能边框路径并缓存结果
        /// </summary>
        private void InitializeBorder()
        {
            // 使用卡片尺寸作为缓存键
            string cacheKey = $"{Width}_{Height}_10";
            
            // 检查缓存中是否已有路径
            if (!BorderCache.TryGetValue(cacheKey, out var borderPath))
            {
                // 创建临时文件存储路径数据
                string tempFile = Path.GetTempFileName();
                try 
                {
                    // 配置C++程序启动参数
                    ProcessStartInfo startInfo = new ProcessStartInfo 
                    {
                        FileName = Path.Combine(Application.StartupPath, "resource", "border_renderer.exe"),
                        Arguments = $"{Width} {Height} 10 \"{tempFile}\"", // 传递宽高和圆角半径
                        UseShellExecute = false, // 不显示命令行窗口
                        CreateNoWindow = true    // 静默运行
                    };

                    // 启动C++程序计算路径
                    using (var process = Process.Start(startInfo)) 
                    {
                        process.WaitForExit();
                        
                        // 检查计算结果
                        if (process.ExitCode == 0 && File.Exists(tempFile)) 
                        {
                            // 读取C++程序生成的路径点
                            var lines = File.ReadAllLines(tempFile);
                            PointF[] points = lines.Select(line => {
                                var parts = line.Split(','); // 解析坐标点
                                return new PointF(float.Parse(parts[0]), float.Parse(parts[1]));
                            }).ToArray();
                            
                            // 创建GraphicsPath对象
                            borderPath = new System.Drawing.Drawing2D.GraphicsPath();
                            borderPath.AddLines(points); // 添加路径点
                            
                            // 缓存路径对象
                            BorderCache.TryAdd(cacheKey, borderPath);
                        }
                    }
                }
                finally 
                {
                    // 确保临时文件被删除
                    if (File.Exists(tempFile)) 
                    {
                        File.Delete(tempFile);
                    }
                }
            }
        }

        // 边框和阴影效果
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // 绘制背景
            using (var brush = new SolidBrush(this.BackColor)) {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            string cacheKey = $"{Width}_{Height}_10";
            if (BorderCache.TryGetValue(cacheKey, out var borderPath))
            {
                try 
                {
                    // 绘制阴影
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0))) {
                        e.Graphics.FillPath(shadowBrush, borderPath);
                    }

                    // 绘制边框（调整为蓝色系）
                    using (var pen = new Pen(Color.FromArgb(70, 130, 200), 4)) {
                        e.Graphics.DrawPath(pen, borderPath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"边框绘制错误: {ex.Message}");
                    // 回退到简单矩形边框
                    using (var pen = new Pen(Color.FromArgb(100, 150, 220), 2)) {
                        e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width-1, Height-1));
                    }
                }
            }
            else
            {
                // 缓存未命中时绘制默认边框
                using (var pen = new Pen(Color.FromArgb(100, 150, 220), 2)) {
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width-1, Height-1));
                }
            }
        }

        /// <summary>
        /// 初始化卡片形状路径
        /// 使用C++程序计算圆角矩形路径并缓存结果
        /// </summary>
        private void InitializeCardPath()
        {
            int radius = 10; // 圆角半径
            string cacheKey = $"{Width}_{Height}_{radius}"; // 缓存键
            
            // 检查缓存
            if (!PathCache.TryGetValue(cacheKey, out var path)) 
            {
                string tempFile = Path.GetTempFileName(); // 临时文件路径
                
                try {
                    // 配置C++程序启动参数
                    ProcessStartInfo startInfo = new ProcessStartInfo {
                        FileName = Path.Combine(Application.StartupPath, "resource", "card_calculator.exe"),
                        Arguments = $"{Width} {Height} {radius} {tempFile}", // 传递宽高半径和输出文件
                        UseShellExecute = false, // 不显示命令行窗口
                        CreateNoWindow = true    // 静默运行
                    };

                    // 启动C++程序计算路径
                    using (var process = Process.Start(startInfo)) {
                        process.WaitForExit();
                        
                        // 检查计算结果
                        if (process.ExitCode == 0 && File.Exists(tempFile)) {
                            // 读取生成的路径点
                            var lines = File.ReadAllLines(tempFile);
                            PointF[] points = lines.Select(line => {
                                var parts = line.Split(','); // 解析坐标
                                return new PointF(float.Parse(parts[0]), float.Parse(parts[1]));
                            }).ToArray();
                            
                            // 创建并缓存路径对象
                            path = new System.Drawing.Drawing2D.GraphicsPath();
                            path.AddLines(points);
                            PathCache.TryAdd(cacheKey, path);
                        }
                    }
                } catch {
                    // C++程序失败时使用C#回退方案
                    path = CalculatePathFallback(Width, Height, radius);
                } finally {
                    // 确保删除临时文件
                    if (File.Exists(tempFile)) {
                        File.Delete(tempFile);
                    }
                }
            }

            // 应用计算好的路径 - 更严格的null检查和异常处理
            try 
            {
                var safePath = path ?? CalculatePathFallback(Width, Height, 10);
                // 更严格的null检查，包括路径和控件状态
                if (safePath != null && 
                    safePath.PointCount > 0 && 
                    this.IsHandleCreated &&
                    !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate {
                        try 
                        {
                            // 委托内部再次验证safePath
                            if (safePath != null && safePath.PointCount > 0)
                            {
                                var validPath = safePath; // 确保非null
                                using (var region = new Region(validPath))
                                {
                                    this.Region = region;
                                    this.Refresh();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"创建Region失败: {ex.Message}");
                            this.Region = null;
                            this.Refresh();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"初始化卡片路径失败: {ex.Message}");
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath CalculatePathFallback(int width, int height, int radius)
        {
            try 
            {
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                path.AddArc(width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
                path.AddArc(width - radius * 2, height - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(0, height - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseFigure();
                return path;
            }
            catch
            {
                // 绝对回退方案 - 返回最小有效路径
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddRectangle(new Rectangle(0, 0, width, height));
                return path;
            }
        }

        public void UpdateDisplay()
        {
            nameLabel.Text = AppName;
            iconBox.Image = AppIcon;
        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // 更严格的null检查
                // 更严格的null检查，包括DownloadManager.Instance和其方法
                // 全面的null和状态检查
                if (sender == null || e == null || 
                    string.IsNullOrWhiteSpace(DownloadUrl) || 
                    string.IsNullOrWhiteSpace(AppName) ||
                    !this.IsHandleCreated ||
                    this.IsDisposed ||
                    DownloadManager.Instance == null ||
                    DownloadManager.Instance.DownloadItems == null ||
                    DownloadManager.Instance.StartDownload == null)
                {
                    return;
                }

                string safeAppName = AppName ?? "未知应用";
                string fileName = $"{safeAppName.Replace(" ", "_")}.exe";
                
                DownloadManager.Instance.StartDownload(fileName, DownloadUrl);
                
                string message = $"已开始下载: {safeAppName}";
                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show(this, message, "下载中", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"下载按钮点击处理失败: {ex.Message}");
                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show(this, "下载处理发生错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }
    }
}
