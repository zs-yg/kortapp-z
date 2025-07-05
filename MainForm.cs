 // _              _                             
 //| | _____  _ __| |_ __ _ _ __  _ __       ____
 //| |/ / _ \| '__| __/ _` | '_ \| '_ \ ____|_  /
 //|   | (_) | |  | || (_| | |_) | |_) |_____/ / 
 //|_|\_\___/|_|   \__\__,_| .__/| .__/     /___|
 //                        |_|   |_|             
#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using AppStore;

namespace AppStore
{
    /// <summary>
    /// 主窗体类，负责应用程序的主界面显示和交互
    /// </summary>
    public class MainForm : Form
    {
        private static readonly string CacheDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "zsyg", "kortapp-z", ".cache");

        private class AppPositionCache
        {
            public string AppName { get; set; } = string.Empty;
            public int X { get; set; }
            public int Y { get; set; }
            public DateTime LastUpdated { get; set; } = DateTime.Now;
        }

        private static string GetPositionCacheFilePath(string appName)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(appName));
            var fileName = BitConverter.ToString(hash).Replace("-", "").ToLower() + ".json";
            return Path.Combine(CacheDir, fileName);
        }

        private static void EnsureCacheDirectory()
        {
            try 
            {
                if (!Directory.Exists(CacheDir))
                {
                    Directory.CreateDirectory(CacheDir);
                    Logger.Log($"已创建缓存目录: {CacheDir}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"创建缓存目录失败: {CacheDir}", ex);
                throw;
            }
        }

        private static bool TryReadPositionCache(string appName, out AppPositionCache? cacheData)
        {
            cacheData = null;
            var cacheFile = GetPositionCacheFilePath(appName);
            
            if (!File.Exists(cacheFile)) 
                return false;

            try
            {
                var json = File.ReadAllText(cacheFile);
                cacheData = JsonSerializer.Deserialize<AppPositionCache>(json);
                return cacheData != null;
            }
            catch
            {
                return false;
            }
        }

        private static void WritePositionCache(string appName, Point position)
        {
            try
            {
                EnsureCacheDirectory();
                var cacheFile = GetPositionCacheFilePath(appName);
                
                var cacheData = new AppPositionCache
                {
                    AppName = appName,
                    X = position.X,
                    Y = position.Y,
                    LastUpdated = DateTime.Now
                };

                var json = JsonSerializer.Serialize(cacheData);
                File.WriteAllText(cacheFile, json);
                Logger.Log($"已保存位置缓存: {appName} ({position.X}, {position.Y})");
            }
            catch (Exception ex)
            {
                Logger.LogError($"保存位置缓存失败: {appName}", ex);
                throw;
            }
        }

        private static bool IsPositionCacheValid(AppPositionCache cacheData)
        {
            // 缓存有效期设为7天
            return (DateTime.Now - cacheData.LastUpdated).TotalDays <= 7;
        }

        private static void SaveAllCardPositions(FlowLayoutPanel flowPanel)
        {
            foreach (Control control in flowPanel.Controls)
            {
                if (control is AppCard card)
                {
                    WritePositionCache(card.AppName, control.Location);
                }
            }
        }

        // 软件下载按钮
        private Button btnApps = null!;
        // 下载进度按钮
        private Button btnDownloads = null!;
        // 设置按钮
        private Button btnSettings = null!;
        // 关于按钮
        private Button btnAbout = null!;
        // 内容显示面板
        private Panel contentPanel = null!;

        /// <summary>
        /// 初始化窗体组件
        /// </summary>
        private void InitializeComponent()
        {
            // 设置窗体基本属性
            // 窗体基本设置
            this.Text = "kortapp-z";
            this.Size = new Size(1430, 1050); // 增加窗体高度
            this.MinimumSize = new Size(600, 600); // 设置最小尺寸
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = new Icon("img/ico/icon.ico"); // 设置窗体图标

            // 注册主题变更事件
            ThemeManager.ThemeChanged += (theme) => 
            {
                this.Invoke((MethodInvoker)delegate {
                    AnimateThemeChange();
                });
            };

            // 现代化顶部导航栏
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Height = 70;
            buttonPanel.BackColor = ThemeManager.ControlBackgroundColor;
            buttonPanel.Padding = new Padding(10, 15, 10, 0);
            buttonPanel.AutoScroll = true;
            buttonPanel.AutoSize = true;
            buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            
            // 导航按钮样式
            Action<Button> styleButton = (Button btn) => {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = ThemeManager.ControlBackgroundColor;
                btn.ForeColor = ThemeManager.TextColor;
                btn.Font = new Font("Microsoft YaHei", 10, FontStyle.Regular);
                btn.Size = new Size(120, 40);
                btn.Cursor = Cursors.Hand;
                btn.FlatAppearance.MouseOverBackColor = ThemeManager.ButtonHoverColor;
                btn.FlatAppearance.MouseDownBackColor = ThemeManager.ButtonActiveColor;
                
                // 悬停效果
                btn.MouseEnter += (s, e) => {
                    btn.ForeColor = Color.FromArgb(0, 120, 215);
                    btn.Font = new Font(btn.Font, FontStyle.Bold);
                };
                
                btn.MouseLeave += (s, e) => {
                    btn.ForeColor = ThemeManager.TextColor;
                    btn.Font = new Font(btn.Font, FontStyle.Regular);
                };
            };

            // 软件下载按钮
            btnApps = new Button();
            btnApps.Text = "软件下载";
            btnApps.Location = new Point(30, 0);
            styleButton(btnApps);
            btnApps.Click += (s, e) => {
                Logger.Log("用户点击了'软件下载'按钮");
                ShowAppsView();
            };
            buttonPanel.Controls.Add(btnApps);
            
            // 下载进度按钮
            btnDownloads = new Button();
            btnDownloads.Text = "下载进度";
            btnDownloads.Location = new Point(170, 0);
            styleButton(btnDownloads);
            btnDownloads.Click += (s, e) => {
                Logger.Log("用户点击了'下载进度'按钮");
                ShowDownloadsView();
            };
            buttonPanel.Controls.Add(btnDownloads);

            // 设置按钮
            btnSettings = new Button();
            btnSettings.Text = "设置";
            btnSettings.Location = new Point(310, 0);
            styleButton(btnSettings);
            btnSettings.Click += (s, e) => {
                Logger.Log("用户点击了'设置'按钮");
                ShowSettingsView();
            };
            buttonPanel.Controls.Add(btnSettings);
            
            // 关于按钮
            btnAbout = new Button();
            btnAbout.Text = "关于";
            btnAbout.Location = new Point(450, 0);
            styleButton(btnAbout);
            btnAbout.Click += (s, e) => {
                Logger.Log("用户点击了'关于'按钮");
                ShowAboutView();
            };
            buttonPanel.Controls.Add(btnAbout);

            // 内置工具按钮
            var btnTools = new Button();
            btnTools.Text = "内置工具";
            btnTools.Location = new Point(590, 0);
            styleButton(btnTools);
            btnTools.Click += (s, e) => {
                Logger.Log("用户点击了'内置工具'按钮");
                ShowToolsView();
            };
            buttonPanel.Controls.Add(btnTools);
            
            // 现代化内容区域
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = ThemeManager.BackgroundColor;
            contentPanel.Padding = new Padding(20);
            contentPanel.AutoScroll = true;
            this.Controls.Add(contentPanel);

            // 添加分隔线
            Panel separator = new Panel();
            separator.Dock = DockStyle.Top;
            separator.Height = 1;
            separator.BackColor = ThemeManager.CurrentTheme == ThemeManager.ThemeMode.Light 
                ? Color.FromArgb(230, 230, 230) 
                : Color.FromArgb(60, 60, 60);
            contentPanel.Controls.Add(separator);

            this.Controls.Add(buttonPanel);
            this.BackColor = ThemeManager.BackgroundColor;

            // 默认显示软件下载视图
            ShowAppsView();
        }

        /// <summary>
        /// 显示设置窗口
        /// </summary>
        private void ShowSettingsView()
        {
            contentPanel.Controls.Clear();
            var settingsControl = new SettingsUserControl();
            contentPanel.Controls.Add(settingsControl);
        }

        private void ShowToolsView()
        {
            try
            {
                contentPanel.Controls.Clear();
                
                var flowPanel = new FlowLayoutPanel();
                flowPanel.Dock = DockStyle.Fill;
                flowPanel.AutoScroll = true;
                flowPanel.WrapContents = true;
                flowPanel.FlowDirection = FlowDirection.LeftToRight;
                flowPanel.Padding = new Padding(15, 50, 15, 15);
                flowPanel.Margin = new Padding(0);
                flowPanel.AutoSize = true;
                flowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                flowPanel.AutoScrollMinSize = new Size(0, 3350);
                contentPanel.Controls.Add(flowPanel);

            // 系统清理卡片
            var cleanerCard = new ToolCard();
            cleanerCard.ToolName = "系统清理";
            
            try 
            {
                cleanerCard.ToolIcon = Image.FromFile("img/resource/png/system_cleaner.png");
            }
            catch
            {
                cleanerCard.ToolIcon = SystemIcons.Shield.ToBitmap();
            }
            
            cleanerCard.UpdateDisplay();
            cleanerCard.ToolCardClicked += (s, e) => {
                    try {
                        string toolPath = Path.Combine(Application.StartupPath, "resource", "system_cleaner.exe");
                        if (File.Exists(toolPath)) {
                            Process.Start(toolPath);
                        } else {
                            MessageBox.Show("系统清理工具未找到，请确保已正确安装", "错误", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } catch (Exception ex) {
                        MessageBox.Show($"启动清理工具失败: {ex.Message}", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                flowPanel.Controls.Add(cleanerCard);

            // 二维码生成卡片
            var qrCard = new ToolCard();
            qrCard.ToolName = "二维码生成";
            
            try 
            {
                qrCard.ToolIcon = Image.FromFile("img/resource/png/QRcode.png");
            }
            catch
            {
                qrCard.ToolIcon = SystemIcons.Application.ToBitmap();
            }
            
            qrCard.UpdateDisplay();
            qrCard.ToolCardClicked += (s, e) => {
                try {
                    var qrForm = new QrCodeGeneratorForm();
                    qrForm.ShowDialog();
                } catch (Exception ex) {
                    MessageBox.Show($"启动二维码生成工具失败: {ex.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            flowPanel.Controls.Add(qrCard);

            // 图片压缩卡片
            var imageCompressorCard = new ToolCard();
            imageCompressorCard.ToolName = "图片压缩";
            
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png","ImageCompressor.png");
                if (File.Exists(iconPath))
                {
                    imageCompressorCard.ToolIcon = Image.FromFile(iconPath);
                }
                else
                {
                    imageCompressorCard.ToolIcon = SystemIcons.Application.ToBitmap();
                }
            }
            catch
            {
                imageCompressorCard.ToolIcon = SystemIcons.Application.ToBitmap();
            }
            
            imageCompressorCard.UpdateDisplay();
            imageCompressorCard.ToolCardClicked += (s, e) => {
                try {
                    string toolPath = Path.Combine(Application.StartupPath, "resource", "image_compressor.exe");
                    if (File.Exists(toolPath)) {
                        var form = new ImageCompressorForm();
                        form.ShowDialog();
                    } else {
                        MessageBox.Show("图片压缩工具未找到，请确保已正确安装", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"启动图片压缩工具失败: {ex.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            flowPanel.Controls.Add(imageCompressorCard);

            // 内存锻炼器卡片
            var memoryTrainerCard = new ToolCard();
            memoryTrainerCard.ToolName = "内存锻炼器";
            
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "memory_trainer.png");
                if (File.Exists(iconPath))
                {
                    memoryTrainerCard.ToolIcon = Image.FromFile(iconPath);
                }
                else
                {
                    memoryTrainerCard.ToolIcon = SystemIcons.Shield.ToBitmap();
                }
            }
            catch
            {
                memoryTrainerCard.ToolIcon = SystemIcons.Shield.ToBitmap();
            }
            
            memoryTrainerCard.UpdateDisplay();
            memoryTrainerCard.ToolCardClicked += (s, e) => {
                try {
                    string toolPath = Path.Combine(Application.StartupPath, "resource", "memory_trainer.exe");
                    if (File.Exists(toolPath)) {
                        Process.Start(toolPath);
                    } else {
                        MessageBox.Show("内存锻炼器工具未找到，请确保已正确安装", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"启动内存锻炼器失败: {ex.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            flowPanel.Controls.Add(memoryTrainerCard);

            // 系统信息查看器卡片
            var systemInfoCard = new ToolCard();
            systemInfoCard.ToolName = "系统信息查看器";
            
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "system_info.png");
                if (File.Exists(iconPath))
                {
                    systemInfoCard.ToolIcon = Image.FromFile(iconPath);
                }
                else
                {
                    systemInfoCard.ToolIcon = SystemIcons.Shield.ToBitmap();
                }
            }
            catch
            {
                systemInfoCard.ToolIcon = SystemIcons.Shield.ToBitmap();
            }
            
            systemInfoCard.UpdateDisplay();
            systemInfoCard.ToolCardClicked += (s, e) => {
                try {
                    string toolPath = Path.Combine(Application.StartupPath, "resource", "system_info.exe");
                    if (File.Exists(toolPath)) {
                        Process.Start(toolPath);
                    } else {
                        MessageBox.Show("系统信息查看器工具未找到，请确保已正确安装", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"启动系统信息查看器失败: {ex.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            flowPanel.Controls.Add(systemInfoCard);

            // 计算器工具卡片
            var calculatorCard = new CalculatorToolCard();
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "Calculator.png");
                if (File.Exists(iconPath))
                {
                    calculatorCard.ToolIcon = Image.FromFile(iconPath);
                }
                calculatorCard.UpdateDisplay();
            }
            catch (Exception ex)
            {
                Logger.LogError("加载计算器图标失败", ex);
            }
            flowPanel.Controls.Add(calculatorCard);

            // 图片查看工具卡片
            var imageViewerCard = new KortAppZ.Tools.Viewer.ImageViewerToolCard();
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "ImageCompressor.png");
                if (File.Exists(iconPath))
                {
                    imageViewerCard.ToolIcon = Image.FromFile(iconPath);
                }
                imageViewerCard.UpdateDisplay();
            }
            catch (Exception ex)
            {
                Logger.LogError("加载图片查看器图标失败", ex);
            }
            flowPanel.Controls.Add(imageViewerCard);

            // 密码生成器工具卡片
            var passwordGeneratorCard = new ToolCard();
            passwordGeneratorCard.ToolName = "密码生成器";
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "password_generator.png");
                if (File.Exists(iconPath))
                {
                    passwordGeneratorCard.ToolIcon = Image.FromFile(iconPath);
                }
                else
                {
                    passwordGeneratorCard.ToolIcon = SystemIcons.Shield.ToBitmap();
                }
            }
            catch
            {
                passwordGeneratorCard.ToolIcon = SystemIcons.Shield.ToBitmap();
            }
            passwordGeneratorCard.ToolCardClicked += (s, e) => {
                try {
                    string toolPath = Path.Combine(Application.StartupPath, "resource", "password_generator.exe");
                    if (File.Exists(toolPath)) {
                        Process.Start(toolPath);
                    } else {
                        MessageBox.Show("密码生成器工具未找到，请确保已正确安装", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"启动密码生成器失败: {ex.Message}", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            passwordGeneratorCard.UpdateDisplay();
            flowPanel.Controls.Add(passwordGeneratorCard);

            // 自启动管理工具卡片
            var selfStartingManagerCard = new SelfStartingManagerToolCard();
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "Self_starting_management.png");
                if (File.Exists(iconPath))
                {
                    selfStartingManagerCard.ToolIcon = Image.FromFile(iconPath);
                }
                selfStartingManagerCard.UpdateDisplay();
            }
            catch (Exception ex)
            {
                Logger.LogError("加载自启动管理工具图标失败", ex);
            }
            flowPanel.Controls.Add(selfStartingManagerCard);

            // 图标提取器工具卡片
            var iconExtractorCard = new AppStore.Tools.IconExtractor.IconExtractorToolCard();
            try 
            {
                string iconPath = Path.Combine(Application.StartupPath, "img", "resource", "png", "ico_extractor.png");
                if (File.Exists(iconPath))
                {
                    iconExtractorCard.ToolIcon = Image.FromFile(iconPath);
                }
                iconExtractorCard.UpdateDisplay();
            }
            catch (Exception ex)
            {
                Logger.LogError("加载图标提取器图标失败", ex);
            }
            flowPanel.Controls.Add(iconExtractorCard);

            }
            catch (Exception ex)
            {
                Logger.LogError("显示内置工具视图时出错", ex);
                MessageBox.Show("加载内置工具时发生错误，请重试", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowAppsView(); // 回退到默认视图
            }
        }

        private void ShowAboutView()
        {
            contentPanel.Controls.Clear();
            var aboutControl = new AboutUserControl();
            contentPanel.Controls.Add(aboutControl);
        }

        /// <summary>
        /// 创建应用卡片
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="downloadUrl">下载URL</param>
        /// <param name="iconPath">图标路径</param>
        /// <returns>创建好的应用卡片</returns>
        /// <summary>
        /// 创建应用卡片控件
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="downloadUrl">应用下载地址</param>
        /// <param name="iconPath">应用图标路径</param>
        /// <returns>创建完成的应用卡片对象</returns>
        private AppCard CreateAppCard(string appName, string downloadUrl, string iconPath)
        {
            // 创建新的应用卡片实例
            AppCard card = new AppCard();
            card.AppName = appName;
            card.DownloadUrl = downloadUrl;
            
            try
            {
                // 尝试从指定路径加载应用图标
                card.AppIcon = Image.FromFile(iconPath);
                Logger.Log($"成功创建应用卡片: {appName}, 图标路径: {iconPath}");
            }
            catch (Exception ex)
            {
                // 图标加载失败时使用系统默认图标
                card.AppIcon = SystemIcons.Application.ToBitmap();
                Logger.LogError($"创建应用卡片时加载图标失败: {appName}, 使用默认图标", ex);
            }
            
            // 更新卡片UI显示
            card.UpdateDisplay(); 
            
            // 返回创建完成的应用卡片
            return card;
        }

        private async void ShowAppsView()
        {
            contentPanel.Controls.Clear();

            // 创建应用卡片面板
            FlowLayoutPanel flowPanel = new FlowLayoutPanel();
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.AutoScroll = true;
            flowPanel.Padding = new Padding(15, 15, 15, 15);
            flowPanel.WrapContents = false;
            flowPanel.Margin = new Padding(0);
            flowPanel.AutoSize = true;
            flowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowPanel.AutoScrollMinSize = new Size(0, 5000);

            // 创建搜索框
            TextBox searchBox = new TextBox();
            searchBox.Width = 300;
            searchBox.Height = 32;
            searchBox.Font = new Font("Microsoft YaHei", 10);
            searchBox.PlaceholderText = "搜索应用...";
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            searchBox.BackColor = Color.White;
            searchBox.ForeColor = Color.FromArgb(64, 64, 64);
            searchBox.Location = new Point((contentPanel.Width - searchBox.Width) / 2, 20);

            // 响应窗体大小变化
            contentPanel.Resize += (s, e) => {
                searchBox.Location = new Point((contentPanel.Width - searchBox.Width) / 2, 20);
            };

            // 搜索框事件
            searchBox.TextChanged += (s, e) => {
                AppSearch.SearchApps(flowPanel, searchBox.Text);
            };

            contentPanel.Controls.Add(searchBox);
            contentPanel.Controls.Add(flowPanel);
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.AutoScroll = true;
            flowPanel.Padding = new Padding(15, 60, 15, 15);
            flowPanel.WrapContents = false;
            flowPanel.Margin = new Padding(0);
            flowPanel.AutoSize = true;
            flowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowPanel.AutoScrollMinSize = new Size(0, 5000);
            contentPanel.Controls.Add(flowPanel);

            // 添加窗体关闭事件处理
            this.FormClosing += (sender, e) => {
                SaveAllCardPositions(flowPanel);
            };

            // 确保控件已创建
            await Task.Delay(100);
            
            try
            {
                // 异步添加卡片
                await Task.Run(() => {
                    if (flowPanel.IsHandleCreated)
                    {
                        flowPanel.Invoke((MethodInvoker)delegate {
                            AddAllAppCards(flowPanel);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("渲染应用卡片时出错", ex);
            }
        }

        private void AddAllAppCards(FlowLayoutPanel flowPanel)
        {
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.AutoScroll = true;
            flowPanel.Padding = new Padding(15, 50, 15, 15);
            flowPanel.WrapContents = true;
            flowPanel.Margin = new Padding(0);
            flowPanel.AutoSize = true;
            flowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowPanel.AutoScrollMinSize = new Size(0, 3550);
            contentPanel.Controls.Add(flowPanel);

            // 添加所有应用卡片并恢复位置
            

            flowPanel.Controls.Add(CreateAppCard(
                "XDM",
                "https://ghproxy.net/https://github.com/subhra74/xdm/releases/download/7.2.11/xdm-setup.msi",
                "img/png/XDM.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "FDM",
                "https://files2.freedownloadmanager.org/6/latest/fdm_x64_setup.exe",
                "img/png/FDM.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "ABDM",
                "https://ghproxy.net/https://github.com/amir1376/ab-download-manager/releases/download/v1.6.4/ABDownloadManager_1.6.4_windows_x64.exe",
                "img/png/ABDM.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "NDM",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.7/NeatDM_setup.exe",
                "img/jpg/NDM.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "youtube-dl",
                "https://ghproxy.net/https://github.com/ytdl-org/youtube-dl/releases/download/2021.12.17/youtube-dl.exe",
                ""));

            flowPanel.Controls.Add(CreateAppCard(
                "python3.8",
                "https://www.python.org/ftp/python/3.8.0/python-3.8.0-amd64.exe",
                "img/png/python.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Oracle Java8",
                "https://sdlc-esd.oracle.com/ESD6/JSCDL/jdk/8u451-b10/8a1589aa0fe24566b4337beee47c2d29/jre-8u451-windows-x64.exe?GroupName=JSC&FilePath=/ESD6/JSCDL/jdk/8u451-b10/8a1589aa0fe24566b4337beee47c2d29/jre-8u451-windows-x64.exe&BHost=javadl.sun.com&File=jre-8u451-windows-x64.exe&AuthParam=1750252610_4d0f61835e3392b8f0158398fd5ebd90&ext=.exe",
                "img/png/java.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Rust",
                "https://static.rust-lang.org/rustup/dist/x86_64-pc-windows-msvc/rustup-init.exe",
                "img/png/rust.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Ruby",
                "https://mirror.sjtu.edu.cn/github-release/oneclick/rubyinstaller2/releases/download/RubyInstaller-3.4.4-2/rubyinstaller-devkit-3.4.4-2-x64.exe",
                "img/png/ruby.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "D",
                "https://downloads.dlang.org/releases/2.x/2.111.0/dmd-2.111.0.exe",
                "img/jpg/D.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Go",
                "https://golang.google.cn/dl/go1.24.4.windows-amd64.msi",
                "img/png/Go.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Node.js",
                "https://nodejs.org/dist/v22.16.0/node-v22.16.0-x64.msi",
                "img/png/nodejs.png"));
                
            flowPanel.Controls.Add(CreateAppCard(
                "mingw-64",
                "https://ghproxy.net/https://github.com/niXman/mingw-builds-binaries/releases/download/15.1.0-rt_v12-rev0/x86_64-15.1.0-release-posix-seh-ucrt-rt_v12-rev0.7z",
                "img/png/mingw-64.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Msys2",
                "https://ghproxy.net/https://github.com/msys2/msys2-installer/releases/download/2025-02-21/msys2-x86_64-20250221.exe",
                "img/png/MSYS2.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "OpenJDK by Azul JDKs",
                "https://cdn.azul.com/zulu/bin/zulu21.42.19-ca-jdk21.0.7-win_x64.msi",
                "img/png/Azul_JDKs.png"));

            flowPanel.Controls.Add(CreateAppCard(
                ".NET SDK 8.0",
                "https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/sdk-8.0.411-windows-x64-installer",
                "img/png/.NET.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "ASP.NET Core 运行时 8.0",
                "https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-aspnetcore-8.0.17-windows-x64-installer",
                "img/png/.NET.png"));

            flowPanel.Controls.Add(CreateAppCard(
                ".NET 桌面运行时 8.0",
                "https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-desktop-8.0.17-windows-x64-installer",
                "img/png/.NET.png"));

            flowPanel.Controls.Add(CreateAppCard(
                ".NET 运行时 8.0",
                "https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-8.0.17-windows-x64-installer",
                "img/png/.NET.png"));
            
            flowPanel.Controls.Add(CreateAppCard(
                "openlist",
                "https://ghproxy.net/https://github.com/OpenListTeam/OpenList/releases/download/beta/openlist-windows-amd64.zip",
                "img/png/openlist.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "SpaceSniffer",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.8/SpaceSniffer.exe",
                "img/png/SpaceSniffer.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "OpenSpeedy",
                "https://ghproxy.net/https://github.com/game1024/OpenSpeedy/releases/download/v1.7.1/OpenSpeedy-v1.7.1.zip",
                "img/png/openspeedy.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Final2x",
                "https://ghproxy.net/https://github.com/Tohrusky/Final2x/releases/download/2024-12-14/Final2x-windows-x64-setup.exe",
                "img/png/Final2x.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Pixpin",
                "https://download.pixpin.cn/PixPin_2.0.0.3.exe",
                "img/png/pixpin.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "QuickLook",
                "https://ghproxy.net/https://github.com/QL-Win/QuickLook/releases/download/4.0.2/QuickLook-4.0.2.exe",
                "img/png/quicklook.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "VSCode",
                "https://vscode.download.prss.microsoft.com/dbazure/download/stable/dfaf44141ea9deb3b4096f7cd6d24e00c147a4b1/VSCodeSetup-x64-1.101.0.exe",
                "img/png/vscode.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "vs community 2022",
                "https://visualstudio.microsoft.com/zh-hans/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false",
                "img/jpg/vs.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "VSCodium",
                "https://visualstudio.microsoft.com/zh-hans/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false",
                "img/png/codium_cnl.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "7-Zip",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/466446150/1645817e-3677-4207-93ff-e62de7e147be?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=releaseassetproduction%2F20250613%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250613T035936Z&X-Amz-Expires=300&X-Amz-Signature=5e02d5fc34f45bd8308029c9fc78052007e9475ce0e32775619921cb8f3b83ea&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3D7z2409-x64.exe&response-content-type=application%2Foctet-stream",
                "img/png/7ziplogo.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "7-Zip Lite",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.1/7-Zip.7z",
                "img/png/7ziplogo.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "peazip",
                "https://ghproxy.net/https://github.com/peazip/PeaZip/releases/download/10.4.0/peazip-10.4.0.WIN64.exe",
                "img/jpg/peazip.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "nanazip",
                "https://ghproxy.net/https://github.com/M2Team/NanaZip/releases/download/5.0.1263.0/NanaZip_5.0.1263.0_DebugSymbols.zip",
                "img/png/nanazip.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "PCL2",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.9/Plain.Craft.Launcher.2.exe",
                "img/jpg/pcl2.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "GreenShot",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/36756917/239aedb0-7d29-11e7-9f9c-d36ec4466ade?X-Amz-Algorithm=AWS4-HMAC-SSHA256&X-Amz-Credential=releaseassetproduction%2F20250613%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250613T041723Z&X-Amz-Expires=300&X-Amz-Signature=be1ef88a68bbc7065af5111809d11de881022933b44f6d961eb6bd6e6b7e60a8&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3DGreenshot-INSTALLER-1.2.10.6-RELEASE.exe&response-content-type=application%2Foctet-stream",
                "img/png/greenshot-logo.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "DWMBlurGlass",
                "https://ghproxy.net/https://github.com/Maplespe/DWMBlurGlass/releases/download/2.3.1r/DWMBlurGlass_2.3.1_x64.zip",
                "img/png/DWMBlurGlass.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Umi-OCR",
                "https://ghproxy.net/https://github.com/hiroi-sora/Umi-OCR/releases/download/v2.1.5/Umi-OCR_Paddle_v2.1.5.7z.exe",
                "img/png/Umi-OCR.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "frp",
                "https://ghproxy.net/https://github.com/fatedier/frp/releases/download/v0.62.1/frp_0.62.1_windows_amd64.zip",
                ""));

            flowPanel.Controls.Add(CreateAppCard(
                "VLC Media Player", 
                "https://mirrors.ustc.edu.cn/videolan-ftp/vlc/3.0.21/win64/vlc-3.0.21-win64.exe",
                "img/jpg/VLC.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "OBS Studio",
                "https://cdn-fastly.obsproject.com/downloads/OBS-Studio-31.0.3-Windows-Installer.exe",
                "img/png/OBS.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Everything",
                "https://www.voidtools.com/Everything-1.4.1.1027.x64-Setup.exe",
                "img/jpg/everything.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Everything Lite",
                "https://www.voidtools.com/Everything-1.4.1.1027.x64.Lite-Setup.exe",
                "img/jpg/everything.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Pinta",
                "https://ghproxy.net/https://github.com/PintaProject/Pinta/releases/download/3.0.1/pinta-3.0.1.zip",
                "img/png/pinta.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "ShareX",
                "https://ghproxy.net/https://github.com/ShareX/ShareX/releases/download/v17.1.0/ShareX-17.1.0-setup.exe",
                "img/png/ShareX.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "LosslessCut",
                "https://ghproxy.net/https://github.com/mifi/lossless-cut/releases/download/v3.64.0/LosslessCut-win-x64.7z",
                "img/png/LosslessCut.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Edge",
                "https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/cb21e6b5-3f63-4df2-bec3-a2015b80dc56/MicrosoftEdgeEnterpriseX64.msi",
                "img/jpg/edge.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Min",
                "https://ghproxy.net/https://github.com/minbrowser/min/releases/download/v1.35.0/min-1.35.0-setup.exe",
                "img/jpg/Min.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Brave",
                "https://ghproxy.net/https://github.com/brave/brave-browser/releases/download/v1.79.126/BraveBrowserSetup.exe",
                "img/png/brave.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Firefox",
                "https://download-ssl.firefox.com.cn/releases-sha2/full/116.0/zh-CN/Firefox-full-latest-win64.exe",
                "img/jpg/firefox.jpg"));
	
	        //这应该是为数不多的国产软件了
            flowPanel.Controls.Add(CreateAppCard(
                "星愿浏览器",
                "https://d1.twinkstar.com/win/Twinkstar_v10.7.1000.2505_Release.exe",
                "img/jpg/Twinkstar.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Mem Reduct",
                "https://memreduct.org/files/memreduct-3.5.2-setup.exe",
                "img/png/mem reduct.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "LibreOffice",
                "https://mirrors.cloud.tencent.com/libreoffice/libreoffice/stable/24.8.6/win/x86_64/LibreOffice_24.8.6_Win_x86-64.msi",
                "img/png/LibreOffice.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "CherryStudio",
                "https://file-cdn.gitcode.com/5007375/releases/untagger_fa019f33ee3b413db46d9329625a2fdf/Cherry-Studio-1.4.2-x64-setup.signed.exe?auth_key=1749794532-8fe6a6851ae34764bb94ea340cd34724-0-84f8c3bb7ca7abe033b03fc07bac78b97e1c9b2863dedb89e5b89ea236205bc0",
                "img/png/CherryStudio.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "GeekUninstaller",
                "https://geekuninstaller.com/geek.zip",
                "img/png/geek.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "aria2",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.2/aria2c.7z",
                ""));

            flowPanel.Controls.Add(CreateAppCard(
                "Git",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.2/Git-2.49.0-64-bit.exe.7z",
                "img/png/git.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "BleachBit",
                "https://download.bleachbit.org/BleachBit-5.0.0-setup.exe",
                "img/png/BleachBit.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "WinDirStat",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/55435293/ec421a5f-c893-4eb3-a75f-53791d7290dd?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=releaseassetproduction%2F20250613%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250613T063808Z&X-Amz-Expires=300&X-Amz-Signature=c21542e5c607a37dfa9e49d3fd9098b8717eaaaf04782d7f8d3a73ef9501c1a9&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3DWinDirStat-x64.msi&response-content-type=application%2Foctet-stream",
                "img/png/WinDirStat.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "HandBrake",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/41215835/5cfe9f3c-b233-4ec1-a3db-84a374cbdd7d?X-Amz-Algorithm=AWS4-HMAC-SSHA256&X-Amz-Credential=releaseassetproduction%2F20250613%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250613T064143Z&X-Amz-Expires=300&X-Amz-Signature=896229c1c4668f0560f6c9ac38fbd22b04f15241717357e7c4d83ed97c65cf0d&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3DHandBrake-1.9.2-x86_64-Win_GUI.exe&response-content-type=application%2Foctet-stream",
                "img/png/HandBrake.png"));
            
            flowPanel.Controls.Add(CreateAppCard(
                "Catime",
                "https://ghproxy.net/https://github.com/vladelaina/Catime/releases/download/v1.1.1/catime_1.1.1.exe",
                "img/png/catime_resize.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "fluxy",
                "https://ghproxy.net/https://github.com/alley-rs/fluxy/releases/download/v0.1.17/fluxy_0.1.17_x64-setup.exe",
                "img/png/fluxy.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "vnote",
                "https://ghproxy.net/https://github.com/vnotex/vnote/releases/download/v3.19.2/VNote-3.19.2-win64.zip",
                "img/png/vnote.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "notepad--",
                "https://www.ghproxy.cn/https://github.com/cxasm/notepad--/releases/download/notepad-v3.3/Notepad--v3.3-plugin-Installer.exe",
                "img/png/notepad--.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "PowerToys",
                "https://ghproxy.net/https://github.com/microsoft/PowerToys/releases/download/v0.91.1/PowerToysSetup-0.91.1-x64.exe",
                "img/png/PowerToys.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "terminal",
                "https://ghproxy.net/https://github.com/microsoft/terminal/releases/download/v1.22.11141.0/Microsoft.WindowsTerminal_1.22.11141.0_x64.zip",
                "img/png/terminal.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "github_cli",
                "https://ghproxy.cn/https://github.com/cli/cli/releases/download/v2.74.2/gh_2.74.2_windows_arm64.msi",
                "img/png/github_cli.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "ReactOS",
                "https://ghproxy.cn/https://github.com/reactos/reactos/releases/download/0.4.15-release/ReactOS-0.4.15-release-1-gdbb43bbaeb2-x86-iso.zip",
                "img/png/ReactOS.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Ubuntu桌面发行版",
                "https://releases.ubuntu.com/24.04/ubuntu-24.04.2-desktop-amd64.iso",
                "img/png/Ubuntu.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "typescript",
                "https://ghproxy.net/https://github.com/microsoft/TypeScript/releases/download/v5.8.3/typescript-5.8.3.tgz",
                ""));

            flowPanel.Controls.Add(CreateAppCard(
                "Gimp",
                "https://mirror.nju.edu.cn/gimp/gimp/v3.0/windows/gimp-3.0.4-setup.exe",
                "img/jpg/Gimp.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Shotcut",
                "https://sourceforge.net/projects/shotcut/files/v25.05.11/shotcut-win64-250511.exe/download",
                "img/png/Shotcut.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Audacity",
                "https://muse-cdn.com/Audacity_Installer_via_MuseHub.exe",
                "img/jpg/Audacity.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "KeePass",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.3/KeePass-2.58-Setup.exe",
                "img/png/KeePass.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Thunderbird",
                "https://download-installer.cdn.mozilla.net/pub/thunderbird/releases/139.0.2/win64/zh-CN/Thunderbird%20Setup%20139.0.2.exe",
                "img/png/Thunderbird.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Dism++",
                "https://ghproxy.net/https://github.com/Chuyu-Team/Dism-Multi-language/releases/download/v10.1.1002.2/Dism++10.1.1002.1B.zip",
                "img/jpg/Dism++.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "LANDrop",
                "https://releases.landrop.app/landrop-v2-electron/LANDrop-2.7.2-win-x64-setup.exe",
                "img/png/banner.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "jarkViewer",
                "https://ghproxy.net/https://github.com/jark006/jarkViewer/releases/download/v1.24/jarkViewer.exe",
                "img/png/jarkViewer.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "CopyQ",
                "https://ghproxy.net/https://github.com/hluk/CopyQ/releases/download/v10.0.0/copyq-10.0.0-setup.exe",
                "img/png/CopyQ.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Bulk Crap Uninstaller",
                "https://ghproxy.net/https://github.com/Klocman/Bulk-Crap-Uninstaller/releases/download/v5.8.3/BCUninstaller_5.8.3_setup.exe",
                "img/png/Bulk-Crap-Uninstaller.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Local Send",
                "https://d.localsend.org/LocalSend-1.17.0-windows-x86-64.exe",
                "img/png/localsend.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Lively Wallpaper",
                "https://ghproxy.net/github.com/rocksdanister/lively/releases/download/v2.1.0.8/lively_setup_x86_full_v2108.exe",
                "img/png/lively.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "inno setup_Zh-CN",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.4/InnoSetup-6.4.3_zh_setup.exe.7z",
                "img/png/innosetup.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Krita",
                "https://mirror.twds.com.tw/kde/stable/krita/5.2.9/krita-x64-5.2.9-setup.exe",
                "img/png/Krita.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "LMMS",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/15778896/bceaac00-be3d-11ea-9a55-0b2b3f3add6d?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=releaseassetproduction%2F20250614%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250614T004234Z&X-Amz-Expires=300&X-Amz-Signature=7f58d4e45578c1d04d8af60b1c738b3bbf11ab69931845564eb8d61868d2570f&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3Dlmms-1.2.2-win64.exe&response-content-type=application%2Foctet-stream",
                "img/jpg/LMMS.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Joplin",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/79162682/395bfb2b-7cde-42c6-b687-b5c277de2c25?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=releaseassetproduction%2F20250614%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250614T004521Z&X-Amz-Expires=300&X-Amz-Signature=76b7c86e0025a16d2834f444fea04ce8b4e143717781a6e487453ce206d886e3&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3DJoplin-Setup-3.3.13.exe&response-content-type=application%2Foctet-stream",
                "img/png/Joplin.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Sumatra PDF",
                "https://files2.sumatrapdfreader.org/software/sumatrapdf/rel/3.5.2/SumatraPDF-3.5.2-64-install.exe",
                "img/ico/Sumatra PDF.ico"));

            flowPanel.Controls.Add(CreateAppCard(
                "Freeplane",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.5/Freeplane-Setup-1.12.11.exe.7z",
                "img/png/Freeplane.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Motrix",
                "https://dl.motrix.app/release/Motrix-Setup-1.8.19.exe",
                "img/png/Motrix.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Aria2Explorer",
                "https://cdn3.zzzmh.cn/v3/crx/1eb83fea34fe418b873e9e048796903f/mpkodccbngfoacfalldjimigbofkhgjn.zip?auth_key=1751644800-ad31ae586097e883b18b066801f9d5b9258cb5a5-0-1310adaee04ec796bc0becdb87f22315",
                "img/png/Aria2Explorer.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Fufu_Tools",
                "https://ghproxy.net/https://github.com/DuckDuckStudio/Fufu_Tools/releases/download/v1.3.10/Fufu_Tools.v1.3.10-Extreme_compression.7z",
                "img/png/Fufu_Tools.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Optimizer",
                "https://ghproxy.net/https://github.com/hellzerg/optimizer/releases/download/16.7/Optimizer-16.7.exe",
                "img/png/optimizer.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Rclone",
                "https://ghproxy.net/https://github.com/rclone/rclone/releases/download/v1.69.3/rclone-v1.69.3-windows-amd64.zip",
                "img/png/Rclone.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "RIME",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/3777237/08e5cfdc-492c-444a-80b3-f8d8caeb5a2a?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=releaseassetproduction%2F20250614%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250614T012552Z&X-Amz-Expires=300&X-Amz-Signature=9d6c9431a037a9e520d506c917b0286f904a538ee6f95063783939b4c9cf9307&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3Dweasel-0.17.4.0-installer.exe&response-content-type=application%2Foctet-stream",
                "img/png/RIME.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "PyDebloatX",
                "https://ghproxy.net/https://github.com/Teraskull/PyDebloatX/releases/download/1.12.0/PyDebloatX_setup.exe",
                "img/ico/PyDebloatX.ico"));

            flowPanel.Controls.Add(CreateAppCard(
                "DropPoint",
                "https://ghproxy.net/https://github.com/GameGodS3/DropPoint/releases/download/v1.2.1/DropPoint-Setup-1.2.1.exe",
                "img/png/DropPoint.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Dnest",
                "https://ghproxy.net/https://github.com/davidkane0526/Dnest/releases/download/V1.3.0/Dnest.exe",
                "img/png/Dnest.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "lockpass",
                "https://ghproxy.net/https://github.com/ftyszyx/lockpass/releases/download/v0.0.14/lockpass-0.0.14-win32-x64-setup.exe",
                "img/png/lockpass.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "WinMerge",
                "https://downloads.sourceforge.net/winmerge/WinMerge-2.16.48.2-x64-Setup.exe",
                "img/png/winmerge.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Keypirinha",
                "https://ghproxy.net/https://github.com/Keypirinha/Keypirinha/releases/download/v2.26/keypirinha-2.26-x64-portable.7z",
                "img/png/Keypirinha.png"));
            
            flowPanel.Controls.Add(CreateAppCard(
                "FileBrowser",
                "https://ghproxy.net/https://github.com/filebrowser/filebrowser/releases/download/v2.32.0/windows-amd64-filebrowser.zip",
                "img/png/FileBrowser.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Cloudreve",
                "https://ghproxy.net/https://github.com/cloudreve/cloudreve/releases/download/3.8.3/cloudreve_3.8.3_windows_amd64.zip",
                "img/png/cloudreve.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "ollama",
                "https://www.ghproxy.cn/https://github.com/ollama/ollama/releases/download/v0.9.5/OllamaSetup.exe",
                "img/png/ollama.png"));
	
            flowPanel.Controls.Add(CreateAppCard(
                "SeelenUI",
                "https://ghproxy.net/https://github.com/eythaann/Seelen-UI/releases/download/v2.3.8/Seelen.UI_2.3.8_x64-setup.exe",
                "img/png/SeelenUI.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "git汉化包",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.6/zh_cn.msg",
                ""));

            flowPanel.Controls.Add(CreateAppCard(
                "everything便携版",
                "https://www.voidtools.com/Everything-1.4.1.1027.x64.zip",
                "img/jpg/everything.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "BongoCat",
                "https://ghproxy.net/https://github.com/ayangweb/BongoCat/releases/download/v0.5.0/BongoCat_0.5.0_x64-setup.exe",
                "img/png/BongoCat.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "GalaceanEngine(code)",
                "https://ghproxy.net/https://github.com/galacean/engine/archive/refs/tags/v1.5.7.zip",
                "img/png/GalaceanEngine.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "trivy",
                "https://ghproxy.net/https://github.com/aquasecurity/trivy/releases/download/v0.63.0/trivy_0.63.0_windows-64bit.zip",
                "img/png/trivy.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "daytona(code)",
                "https://ghproxy.net/https://github.com/aquasecurity/trivy/releases/download/v0.63.0/trivy_0.63.0_windows-64bit.zip",
                "img/png/daytona.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "HowToCook",
                "https://ghproxy.net/https://github.com/Anduin2017/HowToCook/archive/refs/tags/1.5.0.zip",
                ""));

            flowPanel.Controls.Add(CreateAppCard(
                "code-server",
                "https://ghproxy.net/https://github.com/coder/code-server/archive/refs/tags/v4.100.3.zip",
                "img/png/code-server.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "yt-dlp",
                "https://ghproxy.net/https://github.com/yt-dlp/yt-dlp/releases/download/2025.06.09/yt-dlp_win.zip",
                "img/png/yt-dlp.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "omi(code)",
                "https://ghproxy.net/https://github.com/Tencent/omi/archive/refs/tags/v7.7.0.zip",
                "img/png/omi.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Maxun(code)",
                "https://ghproxy.net/https://github.com/getmaxun/maxun/archive/refs/tags/v0.0.16.zip",
                "img/png/Maxun.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "n8n(code)",
                "https://ghproxy.net/https://github.com/n8n-io/n8n/archive/refs/tags/n8n@1.97.1.zip",
                "img/png/n8n.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "WechatRealFriends",
                "https://ghproxy.net/https://github.com/StrayMeteor3337/WechatRealFriends/releases/download/v1.0.4/WechatRealFriends_1.0.4.zip",
                "img/jpg/wx.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "glance",
                "https://ghproxy.net/https://github.com/glanceapp/glance/releases/download/v0.8.4/glance-windows-amd64.zip",
                "img/png/glance.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "openark",
                "https://ghproxy.net/https://github.com/BlackINT3/OpenArk/releases/download/v1.3.8/OpenArk64.exe",
                "img/png/openark.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "SSM",
                "https://ghproxy.net/https://github.com/AlexanderPro/SmartSystemMenu/releases/download/v2.31.0/SmartSystemMenu_v2.31.0.zip",
                "img/png/SSM.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Ditto",
                "https://ghproxy.net/https://github.com/sabrogden/Ditto/releases/download/3.24.246.0/DittoSetup_64bit_3_24_246_0.exe",
                "img/png/Ditto.png"));
        }

        private FlowLayoutPanel downloadsFlowPanel = new FlowLayoutPanel();
        private List<DownloadItem> downloadItems = new List<DownloadItem>();

        /// <summary>
        /// 主窗体构造函数
        /// </summary>
        public MainForm()
        {
            Logger.Log("应用程序启动"); // 记录启动日志
            
            // 初始化窗体组件
            InitializeComponent();
            
            // 应用主题
            ThemeManager.ApplyTheme(this);
            
            // 订阅下载管理器事件
            DownloadManager.Instance.DownloadAdded += OnDownloadAdded; // 下载添加事件
            DownloadManager.Instance.DownloadProgressChanged += OnDownloadProgressChanged; // 下载进度变化事件
            DownloadManager.Instance.DownloadCompleted += OnDownloadCompleted; // 下载完成事件
        }

        /// <summary>
        /// 显示下载视图
        /// </summary>
        private void ShowDownloadsView()
        {
            contentPanel.Controls.Clear(); // 清空内容面板

            // 初始化下载项容器面板
            downloadsFlowPanel = new FlowLayoutPanel();
            downloadsFlowPanel.Dock = DockStyle.Fill; // 填充整个内容区域
            downloadsFlowPanel.AutoScroll = true; // 启用自动滚动
            downloadsFlowPanel.Padding = new Padding(10, 50, 10, 10); // 设置内边距(增加顶部间距)
            downloadsFlowPanel.FlowDirection = FlowDirection.TopDown; // 垂直排列下载项
            downloadsFlowPanel.WrapContents = false; // 禁止换行
            downloadsFlowPanel.AutoScrollMinSize = new Size(0, 2000); // 设置最小滚动区域
            contentPanel.Controls.Add(downloadsFlowPanel); // 添加到内容面板

            // 加载并显示所有下载项
            foreach (var item in DownloadManager.Instance.DownloadItems)
            {
                downloadsFlowPanel.Controls.Add(item); // 添加下载项到面板
            }
        }

        /// <summary>
        /// 处理下载添加事件
        /// </summary>
        /// <param name="item">新添加的下载项</param>
        private void OnDownloadAdded(DownloadItem item)
        {
            // 检查是否需要跨线程调用
            if (InvokeRequired)
            {
                Invoke(new Action<DownloadItem>(OnDownloadAdded), item);
                return;
            }

            Logger.Log($"添加新下载任务: {item.FileName}"); // 记录日志
            downloadItems.Add(item); // 添加到下载项列表
            downloadsFlowPanel?.Controls.Add(item); // 添加到下载面板显示
        }

        /// <summary>
        /// 处理下载进度更新事件
        /// </summary>
        /// <param name="item">进度更新的下载项</param>
        private void OnDownloadProgressChanged(DownloadItem item)
        {
            // 检查是否需要跨线程调用
            if (InvokeRequired)
            {
                Invoke(new Action<DownloadItem>(OnDownloadProgressChanged), item);
                return;
            }

            Logger.Log($"下载进度更新: {item.FileName}, 进度: {item.Progress}%"); // 记录日志
            item.UpdateDisplay(); // 更新UI显示
        }

        /// <summary>
        /// 处理下载完成事件
        /// </summary>
        /// <param name="item">完成的下载项</param>
        private void OnDownloadCompleted(DownloadItem item)
        {
            // 检查是否需要跨线程调用
            if (InvokeRequired)
            {
                Invoke(new Action<DownloadItem>(OnDownloadCompleted), item);
                return;
            }

            Logger.Log($"下载完成: {item.FileName}, 状态: {item.Status}"); // 记录日志
            item.UpdateDisplay(); // 更新UI显示
        }

        /// <summary>
        /// 主题切换动画效果
        /// </summary>
        private void AnimateThemeChange()
        {
            const int animationSteps = 10;
            const int animationInterval = 30;
            
            var timer = new System.Windows.Forms.Timer { Interval = animationInterval };
            int step = 0;
            
            // 保存当前和目标颜色
            var originalBackColor = this.BackColor;
            var targetBackColor = ThemeManager.BackgroundColor;
            var originalForeColor = this.ForeColor;
            var targetForeColor = ThemeManager.TextColor;
            
            timer.Tick += (s, e) => {
                if (step >= animationSteps)
                {
                    timer.Stop();
                    timer.Dispose();
                    // 确保最终颜色准确
                    ThemeManager.ApplyTheme(this);
                    return;
                }
                
                // 计算插值比例
                float ratio = (float)step / animationSteps;
                step++;
                
                // 插值计算新颜色
                var newBackColor = Color.FromArgb(
                    (int)(originalBackColor.R + (targetBackColor.R - originalBackColor.R) * ratio),
                    (int)(originalBackColor.G + (targetBackColor.G - originalBackColor.G) * ratio),
                    (int)(originalBackColor.B + (targetBackColor.B - originalBackColor.B) * ratio));
                    
                var newForeColor = Color.FromArgb(
                    (int)(originalForeColor.R + (targetForeColor.R - originalForeColor.R) * ratio),
                    (int)(originalForeColor.G + (targetForeColor.G - originalForeColor.G) * ratio),
                    (int)(originalForeColor.B + (targetForeColor.B - originalForeColor.B) * ratio));
                
                // 应用新颜色
                this.Invoke((MethodInvoker)delegate {
                    this.BackColor = newBackColor;
                    this.ForeColor = newForeColor;
                    foreach (Control control in this.Controls)
                    {
                        control.BackColor = newBackColor;
                        control.ForeColor = newForeColor;
                        
                        // 特殊处理按钮的悬停状态
                        if (control is Button button)
                        {
                            button.FlatAppearance.MouseOverBackColor = ThemeManager.ButtonHoverColor;
                            button.FlatAppearance.MouseDownBackColor = ThemeManager.ButtonActiveColor;
                        }
                    }
                });
            };
            
            timer.Start();
        }
    }
}
