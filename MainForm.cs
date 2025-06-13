#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AppStore
{
    public class MainForm : Form
    {
        private Button btnApps = null!;
        private Button btnDownloads = null!;
        private Panel contentPanel = null!;

        private void InitializeComponent()
        {
            // 窗体基本设置
            this.Text = "kortapp-z";
            this.Size = new Size(1430, 1050); // 增加窗体高度
            this.MinimumSize = new Size(600, 600); // 设置最小尺寸
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = new Icon("img/ico/icon.ico");

            // 顶部按钮面板
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Height = 60;
            buttonPanel.BackColor = Color.LightGray;
            
            // 软件下载按钮
            btnApps = new Button();
            btnApps.Text = "软件下载";
            btnApps.Size = new Size(100, 30);
            btnApps.Location = new Point(20, 10);
            btnApps.Font = new Font("Microsoft YaHei", 9);
            btnApps.Click += (s, e) => ShowAppsView();
            buttonPanel.Controls.Add(btnApps);
            
            // 下载进度按钮
            btnDownloads = new Button();
            btnDownloads.Text = "下载进度";
            btnDownloads.Size = new Size(100, 30);
            btnDownloads.Location = new Point(140, 10);
            btnDownloads.Font = new Font("Microsoft YaHei", 9);
            btnDownloads.Click += (s, e) => ShowDownloadsView();
            buttonPanel.Controls.Add(btnDownloads);
            
            this.Controls.Add(buttonPanel);

            // 内容区域
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Padding = new Padding(10); // 减少内边距
            this.Controls.Add(contentPanel);

            // 默认显示软件下载视图
            ShowAppsView();
        }

        private AppCard CreateAppCard(string appName, string downloadUrl, string iconPath)
        {
            AppCard card = new AppCard();
            card.AppName = appName;
            card.DownloadUrl = downloadUrl;
            
            try
            {
                card.AppIcon = Image.FromFile(iconPath);
            }
            catch
            {
                card.AppIcon = SystemIcons.Application.ToBitmap();
            }
            
            card.UpdateDisplay();
            return card;
        }

        private void ShowAppsView()
        {
            contentPanel.Controls.Clear();

            // 使用FlowLayoutPanel来组织应用卡片
            FlowLayoutPanel flowPanel = new FlowLayoutPanel();
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.AutoScroll = true;
            flowPanel.Padding = new Padding(15, 50, 15, 15); // 进一步增加顶部内边距
            flowPanel.WrapContents = true;
            flowPanel.Margin = new Padding(0);
            contentPanel.Controls.Add(flowPanel);

            // 添加所有应用卡片
            flowPanel.Controls.Add(CreateAppCard(
                "WindowsCleaner",
                "https://ghproxy.net/https://github.com/darkmatter2048/WindowsCleaner/releases/download/v5.0.8/windowscleaner_v5.0.8_amd64_x64_setup.exe",
                "img/png/WindowsCleaner.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Alist",
                "https://ghproxy.net/https://github.com/AlistGo/alist/releases/download/v3.45.0/alist-windows-amd64.zip",
                "img/png/alist.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "OpenSpeedy",
                "https://ghproxy.net/https://github.com/game1024/OpenSpeedy/releases/download/v1.7.1/OpenSpeedy-v1.7.1.zip",
                "img/png/openspeedy.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "QuickLook",
                "https://ghproxy.net/https://github.com/QL-Win/QuickLook/releases/download/4.0.2/QuickLook-4.0.2.exe",
                "img/png/quicklook.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "VSCode",
                "https://vscode.download.prss.microsoft.com/dbazure/download/stable/dfaf44141ea9deb3b4096f7cd6d24e00c147a4b1/VSCodeSetup-x64-1.101.0.exe",
                "img/png/vscode.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Notepad++",
                "https://ghproxy.net/https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v8.8.1/npp.8.8.1.Installer.exe",
                "img/png/notepad++.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "7-Zip",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/466446150/1645817e-3677-4207-93ff-e62de7e147be?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=releaseassetproduction%2F20250613%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250613T035936Z&X-Amz-Expires=300&X-Amz-Signature=5e02d5fc34f45bd8308029c9fc78052007e9475ce0e32775619921cb8f3b83ea&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3D7z2409-x64.exe&response-content-type=application%2Foctet-stream"，
                "img/png/7ziplogo.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "7-Zip Lite",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.1/7-Zip.7z",
                "img/png/7ziplogo.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "GreenShot",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/36756917/239aedb0-7d29-11e7-9f9c-d36ec4466ade?X-Amz-Algorithm=AWS4-HMAC-SSHA256&X-Amz-Credential=releaseassetproduction%2F20250613%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250613T041723Z&X-Amz-Expires=300&X-Amz-Signature=be1ef88a68bbc7065af5111809d11de881022933b44f6d961eb6bd6e6b7e60a8&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3DGreenshot-INSTALLER-1.2.10.6-RELEASE.exe&response-content-type=application%2Foctet-stream"，
                "img/png/greenshot-logo.png"));

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
                "fluxy",
                "https://ghproxy.net/https://github.com/alley-rs/fluxy/releases/download/v0.1.17/fluxy_0.1.17_x64-setup.exe",
                "img/png/fluxy.png"));
        }

        private FlowLayoutPanel downloadsFlowPanel = new FlowLayoutPanel();
        private List<DownloadItem> downloadItems = new List<DownloadItem>();

        public MainForm()
        {
            InitializeComponent();
            // 订阅下载管理器事件
            DownloadManager.Instance.DownloadAdded += OnDownloadAdded;
            DownloadManager.Instance.DownloadProgressChanged += OnDownloadProgressChanged;
            DownloadManager.Instance.DownloadCompleted += OnDownloadCompleted;
        }

        private void ShowDownloadsView()
        {
            contentPanel.Controls.Clear();

            // 使用FlowLayoutPanel组织下载项
            downloadsFlowPanel = new FlowLayoutPanel();
            downloadsFlowPanel.Dock = DockStyle.Fill;
            downloadsFlowPanel.AutoScroll = true;
            downloadsFlowPanel.Padding = new Padding(10, 30, 10, 10); // 增加顶部间距
            downloadsFlowPanel.FlowDirection = FlowDirection.TopDown;
            downloadsFlowPanel.WrapContents = false;
            contentPanel.Controls.Add(downloadsFlowPanel);

            // 显示所有下载项
            foreach (var item in DownloadManager.Instance.DownloadItems)
            {
                downloadsFlowPanel.Controls.Add(item);
            }
        }

        private void OnDownloadAdded(DownloadItem item)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DownloadItem>(OnDownloadAdded), item);
                return;
            }

            downloadItems.Add(item);
            downloadsFlowPanel?.Controls.Add(item);
        }

        private void OnDownloadProgressChanged(DownloadItem item)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DownloadItem>(OnDownloadProgressChanged), item);
                return;
            }

            item.UpdateDisplay();
        }

        private void OnDownloadCompleted(DownloadItem item)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DownloadItem>(OnDownloadCompleted), item);
                return;
            }

            item.UpdateDisplay();
        }
    }
}
