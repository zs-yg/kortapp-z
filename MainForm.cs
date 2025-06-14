#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;
using AppStore;

namespace AppStore
{
    public class MainForm : Form
    {
        private Button btnApps = null!;
        private Button btnDownloads = null!;
        private Button btnSettings = null!;
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
            btnApps.Click += (s, e) => 
            {
                Logger.Log("用户点击了'软件下载'按钮");
                ShowAppsView();
            };
            buttonPanel.Controls.Add(btnApps);
            
            // 下载进度按钮
            btnDownloads = new Button();
            btnDownloads.Text = "下载进度";
            btnDownloads.Size = new Size(100, 30);
            btnDownloads.Location = new Point(140, 10);
            btnDownloads.Font = new Font("Microsoft YaHei", 9);
            btnDownloads.Click += (s, e) => 
            {
                Logger.Log("用户点击了'下载进度'按钮");
                ShowDownloadsView();
            };
            buttonPanel.Controls.Add(btnDownloads);

            // 设置按钮
            btnSettings = new Button
            {
                Text = "设置",
                Size = new Size(100, 30),
                Location = new Point(260, 10),
                Font = new Font("Microsoft YaHei", 9)
            };
            btnSettings.Click += (s, e) => 
            {
                Logger.Log("用户点击了'设置'按钮");
                ShowSettingsView();
            };
            buttonPanel.Controls.Add(btnSettings);
            
            // 内容区域
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Padding = new Padding(10);
            this.Controls.Add(contentPanel);

            this.Controls.Add(buttonPanel);

            // 默认显示软件下载视图
            ShowAppsView();
        }

        private void ShowSettingsView()
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private AppCard CreateAppCard(string appName, string downloadUrl, string iconPath)
        {
            AppCard card = new AppCard();
            card.AppName = appName;
            card.DownloadUrl = downloadUrl;
            
            try
            {
                card.AppIcon = Image.FromFile(iconPath);
                Logger.Log($"成功创建应用卡片: {appName}, 图标路径: {iconPath}");
            }
            catch (Exception ex)
            {
                card.AppIcon = SystemIcons.Application.ToBitmap();
                Logger.LogError($"创建应用卡片时加载图标失败: {appName}, 使用默认图标", ex);
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
            flowPanel.Padding = new Padding(15, 50, 15, 15); // 恢复原有内边距
            flowPanel.WrapContents = true;
            flowPanel.Margin = new Padding(0);
            flowPanel.AutoSize = true;
            flowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowPanel.AutoScrollMinSize = new Size(0, 2000); // 增加滑动距离
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
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/466446150/1645817e-3677-4207-93ff-e62de7e147be?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=releaseassetproduction%2F20250613%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250613T035936Z&X-Amz-Expires=300&X-Amz-Signature=5e02d5fc34f45bd8308029c9fc78052007e9475ce0e32775619921cb8f3b83ea&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3D7z2409-x64.exe&response-content-type=application%2Foctet-stream",
                "img/png/7ziplogo.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "7-Zip Lite",
                "https://ghproxy.net/https://github.com/zs-yg/package/releases/download/v0.1/7-Zip.7z",
                "img/png/7ziplogo.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "GreenShot",
                "https://objects.githubusercontent.com/github-production-release-asset-2e65be/36756917/239aedb0-7d29-11e7-9f9c-d36ec4466ade?X-Amz-Algorithm=AWS4-HMAC-SSHA256&X-Amz-Credential=releaseassetproduction%2F20250613%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250613T041723Z&X-Amz-Expires=300&X-Amz-Signature=be1ef88a68bbc7065af5111809d11de881022933b44f6d961eb6bd6e6b7e60a8&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3DGreenshot-INSTALLER-1.2.10.6-RELEASE.exe&response-content-type=application%2Foctet-stream",
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
                "LosslessCut",
                "https://ghproxy.net/https://github.com/mifi/lossless-cut/releases/download/v3.64.0/LosslessCut-win-x64.7z",
                "img/png/LosslessCut.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "Edge",
                "https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/cb21e6b5-3f63-4df2-bec3-a2015b80dc56/MicrosoftEdgeEnterpriseX64.msi",
                "img/jpg/edge.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Firefox",
                "https://download-ssl.firefox.com.cn/releases-sha2/full/116.0/zh-CN/Firefox-full-latest-win64.exe",
                "img/jpg/firefox.jpg"));

            flowPanel.Controls.Add(CreateAppCard(
                "Msys2",
                "https://github.com/msys2/msys2-installer/releases/download/2025-02-21/msys2-x86_64-20250221.exe",
                "img/png/MSYS2.png"));

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
                "PowerToys",
                "https://ghproxy.net/https://github.com/microsoft/PowerToys/releases/download/v0.91.1/PowerToysSetup-0.91.1-x64.exe",
                "img/png/PowerToys.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "terminal",
                "https://ghproxy.net/https://github.com/microsoft/terminal/releases/download/v1.22.11141.0/Microsoft.WindowsTerminal_1.22.11141.0_x64.zip",
                "img/png/terminal.png"));

            flowPanel.Controls.Add(CreateAppCard(
                "typescript",
                "https://ghproxy.net/https://github.com/microsoft/TypeScript/releases/download/v5.8.3/typescript-5.8.3.tgz",
                ""));

            flowPanel.Controls.Add(CreateAppCard(
                "peazip",
                "https://ghproxy.net/https://github.com/peazip/PeaZip/releases/download/10.4.0/peazip-10.4.0.WIN64.exe",
                "img/jpg/peazip.jpg"));

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
        }

        private FlowLayoutPanel downloadsFlowPanel = new FlowLayoutPanel();
        private List<DownloadItem> downloadItems = new List<DownloadItem>();

        public MainForm()
        {
            Logger.Log("应用程序启动");
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
            downloadsFlowPanel.Padding = new Padding(10, 50, 10, 10); // 增加顶部间距
            downloadsFlowPanel.FlowDirection = FlowDirection.TopDown;
            downloadsFlowPanel.WrapContents = false;
            downloadsFlowPanel.AutoScrollMinSize = new Size(0, 2000); // 增加滑动距离
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

            Logger.Log($"添加新下载任务: {item.FileName}");
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

            Logger.Log($"下载进度更新: {item.FileName}, 进度: {item.Progress}%");
            item.UpdateDisplay();
        }

        private void OnDownloadCompleted(DownloadItem item)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DownloadItem>(OnDownloadCompleted), item);
                return;
            }

            Logger.Log($"下载完成: {item.FileName}, 状态: {item.Status}");
            item.UpdateDisplay();
        }
    }
}
