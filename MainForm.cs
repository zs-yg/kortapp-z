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
            this.Text = "应用商店";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = new Icon("img/ico/icon.ico");

            // 顶部按钮面板
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Height = 50;
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
            contentPanel.Padding = new Padding(20);
            this.Controls.Add(contentPanel);

            // 默认显示软件下载视图
            ShowAppsView();
        }

        private void ShowAppsView()
        {
            contentPanel.Controls.Clear();

            // 使用FlowLayoutPanel来组织应用卡片
            // 使用FlowLayoutPanel实现自动流式布局
            FlowLayoutPanel flowPanel = new FlowLayoutPanel();
            flowPanel.Dock = DockStyle.Fill;
            flowPanel.AutoScroll = true;
            flowPanel.Padding = new Padding(10, 30, 10, 10); // 增加顶部间距
            flowPanel.WrapContents = true;
            contentPanel.Controls.Add(flowPanel);

            // 创建WindowsCleaner应用卡片
            AppCard windowsCleanerCard = new AppCard();
            windowsCleanerCard.AppName = "WindowsCleaner";
            windowsCleanerCard.DownloadUrl = "https://ghproxy.net/https://github.com/darkmatter2048/WindowsCleaner/releases/download/v5.0.8/windowscleaner_v5.0.8_amd64_x64_setup.exe";
            
            try
            {
                // 加载应用图标
                windowsCleanerCard.AppIcon = Image.FromFile("img/png/WindowsCleaner.png");
            }
            catch
            {
                // 如果图标加载失败，使用默认图标
                windowsCleanerCard.AppIcon = SystemIcons.Application.ToBitmap();
            }
            
            windowsCleanerCard.UpdateDisplay();
            // 添加卡片到流式布局
            flowPanel.Controls.Add(windowsCleanerCard);
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
