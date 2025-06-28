#include "main_window.h"
#include <tchar.h>
#include <commctrl.h>
#include <wchar.h>
#include <stdio.h>

#define IDC_INFO_TEXT 1002

void update_main_window(HWND hWnd, SystemInfo* sysInfo) {
    HWND hInfoText = GetDlgItem(hWnd, IDC_INFO_TEXT);
    if (!hInfoText) {
        // 创建信息显示控件
        hInfoText = CreateWindow(
            _T("EDIT"),
            _T(""),
            WS_CHILD | WS_VISIBLE | WS_VSCROLL | ES_MULTILINE | ES_READONLY,
            20, 50, 800, 550,
            hWnd,
            (HMENU)IDC_INFO_TEXT,
            (HINSTANCE)GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
            NULL);
        
        SendMessage(hInfoText, WM_SETFONT, 
                   (WPARAM)GetStockObject(DEFAULT_GUI_FONT), TRUE);
    }

    // 使用宽字符处理所有文本
    wchar_t infoText[2048];
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

    SetWindowTextW(hInfoText, infoText);
}
