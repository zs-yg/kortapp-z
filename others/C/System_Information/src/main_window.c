#include "main_window.h"
#include "disk_info.h"
#include <tchar.h>
#include <commctrl.h>
#include <wchar.h>
#include <stdio.h>

#define IDC_INFO_TEXT 1002
#define IDM_FULLSCREEN 1003

// 全屏状态标志
static BOOL g_isFullScreen = FALSE;
// 保存原始窗口位置和大小
static RECT g_windowRect;

void toggle_fullscreen(HWND hWnd) {
    g_isFullScreen = !g_isFullScreen;
    
    if (g_isFullScreen) {
        // 保存当前窗口位置和大小
        GetWindowRect(hWnd, &g_windowRect);
        
        // 设置全屏样式
        SetWindowLong(hWnd, GWL_STYLE, 
                     WS_OVERLAPPEDWINDOW & ~(WS_CAPTION | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_SYSMENU));
        
        // 设置全屏尺寸
        MONITORINFO mi = {0};
        mi.cbSize = sizeof(mi);
        GetMonitorInfo(MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST), &mi);
        SetWindowPos(hWnd, HWND_TOP,
                    mi.rcMonitor.left,
                    mi.rcMonitor.top,
                    mi.rcMonitor.right - mi.rcMonitor.left,
                    mi.rcMonitor.bottom - mi.rcMonitor.top,
                    SWP_FRAMECHANGED);
    } else {
        // 恢复窗口样式
        SetWindowLong(hWnd, GWL_STYLE, WS_OVERLAPPEDWINDOW);
        
        // 恢复原始大小和位置
        SetWindowPos(hWnd, NULL,
                    g_windowRect.left,
                    g_windowRect.top,
                    g_windowRect.right - g_windowRect.left,
                    g_windowRect.bottom - g_windowRect.top,
                    SWP_FRAMECHANGED);
    }
}

void update_main_window(HWND hWnd, SystemInfo* sysInfo) {
    // 根据全屏状态计算窗口尺寸
    int windowWidth, windowHeight;
    if (g_isFullScreen) {
        MONITORINFO mi = {0};
        mi.cbSize = sizeof(mi);
        GetMonitorInfo(MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST), &mi);
        windowWidth = mi.rcMonitor.right - mi.rcMonitor.left;
        windowHeight = mi.rcMonitor.bottom - mi.rcMonitor.top;
    } else {
        // 普通模式下使用70%屏幕尺寸
        windowWidth = (int)(GetSystemMetrics(SM_CXSCREEN) * 0.7);
        windowHeight = (int)(GetSystemMetrics(SM_CYSCREEN) * 0.7);
    }
    
    HWND hInfoText = GetDlgItem(hWnd, IDC_INFO_TEXT);
    if (hInfoText) {
        // 更新现有控件大小和位置
        SetWindowPos(hInfoText, NULL, 
                    30, 50, windowWidth - 60, windowHeight - 80,
                    SWP_NOZORDER);
    } else {
        // 创建信息显示控件
        hInfoText = CreateWindow(
            _T("EDIT"), 
            _T(""),
            WS_CHILD | WS_VISIBLE | WS_VSCROLL | ES_MULTILINE | ES_READONLY | WS_BORDER,
            30, 50, windowWidth - 60, windowHeight - 80,
            hWnd,
            (HMENU)IDC_INFO_TEXT,
            (HINSTANCE)GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
            NULL);
        
    // 计算动态字体大小
    int fontSize = max(16, windowHeight / 30);
    
    // 创建支持中文的字体
    HFONT hFont = CreateFont(
        fontSize, 0, 0, 0, FW_NORMAL, FALSE, FALSE, FALSE,
        DEFAULT_CHARSET, OUT_DEFAULT_PRECIS, CLIP_DEFAULT_PRECIS,
        DEFAULT_QUALITY, DEFAULT_PITCH | FF_DONTCARE,
        _T("Microsoft YaHei"));
        
        SendMessage(hInfoText, WM_SETFONT, (WPARAM)hFont, TRUE);
    }

    // 使用宽字符处理所有文本
    wchar_t infoText[4096];  // 增大缓冲区以适应磁盘信息
    wchar_t cpuNameW[256];
    MultiByteToWideChar(CP_UTF8, 0, sysInfo->cpuName, -1, cpuNameW, 256);
    
    // 格式化系统信息，确保每个部分正确换行
    swprintf(infoText, 2048,
        L"===== 系统信息 =====\r\n\r\n"
        L"[处理器信息]\r\n"
        L"型号: %s\r\n"
        L"物理核心数: %d\r\n"
        L"逻辑核心数: %d\r\n\r\n"
        L"[内存信息]\r\n"
        L"总内存: %.2f GB\r\n"
        L"可用内存: %.2f GB\r\n\r\n"
        L"[操作系统]\r\n"
        L"版本: Windows %d.%d\r\n"
        L"构建版本号: %d\r\n",
        cpuNameW,
        sysInfo->cpuCores,
        sysInfo->cpuThreads,
        (float)sysInfo->memoryStatus.ullTotalPhys / (1024 * 1024 * 1024),
        (float)sysInfo->memoryStatus.ullAvailPhys / (1024 * 1024 * 1024),
        sysInfo->osVersion.dwMajorVersion,
        sysInfo->osVersion.dwMinorVersion,
        sysInfo->osVersion.dwBuildNumber);

    // 添加磁盘信息
    DiskInfo disks[26];
    int diskCount;
    get_disk_info(disks, &diskCount);
    
    wchar_t diskInfoText[2048];
    swprintf(diskInfoText, 2048,
        L"\r\n[磁盘信息]\r\n"
        L"磁盘数量: %d\r\n", diskCount);
    wcscat(infoText, diskInfoText);
    
    for (int i = 0; i < diskCount; i++) {
        swprintf(diskInfoText, 2048,
            L"%c: 文件系统: %ls, 总容量: %.2f GB, 剩余容量: %.2f GB\r\n",
            disks[i].driveLetter,
            disks[i].fileSystem[0] ? L"NTFS" : L"",
            (float)disks[i].totalBytes / (1024 * 1024 * 1024),
            (float)disks[i].freeBytes / (1024 * 1024 * 1024));
        wcscat(infoText, diskInfoText);
    }

    SetWindowTextW(hInfoText, infoText);
}
