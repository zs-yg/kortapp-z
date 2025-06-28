#define UNICODE
#define _UNICODE
#include <stdio.h>
#include <windows.h>
#include <commctrl.h>
#include <stdint.h>
#include <psapi.h>
#include "../include/memory_trainer.h"

// 全局变量
HWND g_hMainWnd;
HWND hSizeEdit, hProgressBar, hRetainCheck, hRunButton;
HFONT hFont;

// 初始化现代UI
void InitModernUI(HWND hWnd) {
    // 创建字体
    hFont = CreateFont(16, 0, 0, 0, FW_NORMAL, FALSE, FALSE, FALSE, 
                      DEFAULT_CHARSET, OUT_DEFAULT_PRECIS, CLIP_DEFAULT_PRECIS, 
                      CLEARTYPE_QUALITY, DEFAULT_PITCH, L"微软雅黑");

    // 内存大小输入
    CreateWindowW(L"STATIC", L"内存大小 (MB):", 
                WS_VISIBLE | WS_CHILD | SS_CENTERIMAGE,
                20, 20, 120, 30, hWnd, NULL, NULL, NULL);
    
    hSizeEdit = CreateWindowW(L"EDIT", L"100",
                WS_VISIBLE | WS_CHILD | WS_BORDER | ES_NUMBER,
                150, 20, 120, 30, hWnd, NULL, NULL, NULL);

    // 填充模式选择
    CreateWindowW(L"STATIC", L"填充模式:", 
                WS_VISIBLE | WS_CHILD | SS_CENTERIMAGE,
                20, 70, 120, 30, hWnd, NULL, NULL, NULL);
    
    HWND hFillZero = CreateWindowW(L"BUTTON", L"填充0",
                WS_VISIBLE | WS_CHILD | BS_AUTORADIOBUTTON | WS_GROUP,
                150, 70, 80, 30, hWnd, (HMENU)IDC_FILL_ZERO, NULL, NULL);
    
                CreateWindowW(L"BUTTON", L"随机填充",
                WS_VISIBLE | WS_CHILD | BS_AUTORADIOBUTTON,
                240, 70, 100, 30, hWnd, (HMENU)IDC_FILL_RANDOM, NULL, NULL);
    
    SendMessage(hFillZero, BM_SETCHECK, BST_CHECKED, 0);

    // 进度条
    hProgressBar = CreateWindowW(PROGRESS_CLASSW, NULL,
                WS_VISIBLE | WS_CHILD | PBS_SMOOTH,
                20, 120, 360, 30, hWnd, NULL, NULL, NULL);
    SendMessage(hProgressBar, PBM_SETRANGE, 0, MAKELPARAM(0, 100));

    // 保持内存选项
    hRetainCheck = CreateWindowW(L"BUTTON", L"保持内存内容",
                WS_VISIBLE | WS_CHILD | BS_AUTOCHECKBOX,
                20, 170, 150, 30, hWnd, NULL, NULL, NULL);

    // 执行按钮
    hRunButton = CreateWindowW(L"BUTTON", L"开始测试",
                WS_VISIBLE | WS_CHILD | BS_DEFPUSHBUTTON,
                200, 170, 180, 40, hWnd, (HMENU)IDC_RUN_TEST, NULL, NULL);

    // 设置字体
    SendMessage(hSizeEdit, WM_SETFONT, (WPARAM)hFont, TRUE);
    SendMessage(hRetainCheck, WM_SETFONT, (WPARAM)hFont, TRUE);
    SendMessage(hRunButton, WM_SETFONT, (WPARAM)hFont, TRUE);
}

// 主窗口过程
LRESULT CALLBACK WndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    switch (msg) {
        case WM_CREATE:
            InitModernUI(hWnd);
            break;
            
        case WM_COMMAND:
            if (LOWORD(wParam) == IDC_RUN_TEST) {
                wchar_t sizeText[32];
                GetWindowTextW(hSizeEdit, sizeText, 32);
                int sizeMB = _wtoi(sizeText);
                
                // 获取系统内存信息
                MEMORYSTATUSEX memStatus;
                memStatus.dwLength = sizeof(memStatus);
                GlobalMemoryStatusEx(&memStatus);
                
                // 验证输入
                if (sizeMB <= 0) {
                    MessageBoxW(hWnd, L"请输入有效的内存大小(大于0)", L"错误", MB_ICONERROR);
                    return 0;
                }
                
                // 检查是否超过系统可用内存的75%
                DWORDLONG maxRecommended = memStatus.ullAvailPhys * 3 / 4;
                if ((DWORDLONG)sizeMB * 1024 * 1024 > maxRecommended) {
                    wchar_t warning[256];
                    wsprintfW(warning, L"请求的内存大小(%dMB)超过推荐值(%.1fMB)\n可能造成系统不稳定，是否继续?", 
                            sizeMB, (float)maxRecommended/1024/1024);
                    if (MessageBoxW(hWnd, warning, L"警告", MB_YESNO | MB_ICONWARNING) != IDYES) {
                        return 0;
                    }
                }
                
                // 获取填充模式
                int mode = 0;
                if (SendMessage(GetDlgItem(hWnd, IDC_FILL_RANDOM), BM_GETCHECK, 0, 0) == BST_CHECKED) {
                    mode = 1;
                }
                
                // MB转字节
                size_t bytes = 0;
                if (sizeMB > 0 && sizeMB < 1024*1024) { // 限制最大1TB
                    bytes = (size_t)sizeMB * 1024 * 1024;
                    void* ptr = allocate_memory(bytes);
                    if (!ptr) {
                        // 错误信息已由report_error记录，直接返回
                        return 0;
                    }
                    
                    // 显示分配成功
                    SendMessage(hProgressBar, PBM_SETPOS, 10, 0);
                    
                    // 进度回调函数
                    void progress_callback(int percent) {
                        PostMessage(g_hMainWnd, WM_APP, percent, 0);
                    }
                    
                    // 填充内存
                    fill_memory(ptr, bytes, mode, progress_callback);
                    
                    // 检查填充是否完成
                    if (SendMessage(hProgressBar, PBM_GETPOS, 0, 0) < 100) {
                        // 错误信息已由fill_memory中的report_error记录
                    }
                    
                    // 确保完成
                    SendMessage(hProgressBar, PBM_SETPOS, 100, 0);
                    PROCESS_MEMORY_COUNTERS_EX pmc;
                    GetProcessMemoryInfo(GetCurrentProcess(), (PROCESS_MEMORY_COUNTERS*)&pmc, sizeof(pmc));
                    
                    wchar_t msg[256];
                    wsprintfW(msg, L"成功填充 %d MB 内存\n实际使用: %.1f GB",
                            sizeMB, 
                            (float)pmc.PrivateUsage / 1024.0f / 1024.0f / 1024.0f);
                    MessageBoxW(hWnd, msg, L"操作成功", MB_OK);
                    
                    if (SendMessage(hRetainCheck, BM_GETCHECK, 0, 0) != BST_CHECKED) {
                        free_memory(ptr, sizeMB);
                    }
                } else {
                    MessageBoxW(hWnd, L"请输入有效的内存大小", L"错误", MB_ICONERROR);
                }
            }
            break;
            
        case WM_APP:
            SendMessage(hProgressBar, PBM_SETPOS, (WPARAM)wParam, 0);
            break;
            
        case WM_DESTROY:
            DeleteObject(hFont);
            PostQuitMessage(0);
            break;
            
        default:
            return DefWindowProc(hWnd, msg, wParam, lParam);
    }
    return 0;
}

// 应用程序入口
int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
                   PWSTR pCmdLine, int nCmdShow) {
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(pCmdLine);
    // 调试输出
    OutputDebugStringW(L"内存锻炼器启动...\n");
    
    SYSTEM_INFO sysInfo;
    GetSystemInfo(&sysInfo);
    wchar_t debugMsg[256];
    wsprintfW(debugMsg, L"处理器数量: %lu\n", (unsigned long)sysInfo.dwNumberOfProcessors);
    OutputDebugStringW(debugMsg);
    wsprintfW(debugMsg, L"页面大小: %lu\n", (unsigned long)sysInfo.dwPageSize);
    OutputDebugStringW(debugMsg);
    
    MEMORYSTATUSEX memStatus;
    memStatus.dwLength = sizeof(memStatus);
    GlobalMemoryStatusEx(&memStatus);
    wsprintfW(debugMsg, L"总物理内存: %.1f GB\n", (float)memStatus.ullTotalPhys/1024/1024/1024);
    OutputDebugStringW(debugMsg);
    // 注册窗口类
    WNDCLASSEX wc = {0};
    wc.cbSize = sizeof(WNDCLASSEX);
    wc.style = CS_HREDRAW | CS_VREDRAW;
    wc.lpfnWndProc = WndProc;
    wc.hInstance = hInstance;
    wc.hCursor = LoadCursor(NULL, IDC_ARROW);
    wc.hbrBackground = (HBRUSH)(COLOR_WINDOW+1);
    wc.lpszClassName = L"MemoryTrainerModern";
    RegisterClassEx(&wc);

    // 创建主窗口
    g_hMainWnd = CreateWindowEx(0, L"MemoryTrainerModern", L"内存锻炼器",
                               WS_OVERLAPPEDWINDOW & ~WS_MAXIMIZEBOX,
                               CW_USEDEFAULT, CW_USEDEFAULT, 450, 300,
                               NULL, NULL, hInstance, NULL);

    ShowWindow(g_hMainWnd, nCmdShow);
    UpdateWindow(g_hMainWnd);

    // 消息循环
    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return (int)msg.wParam;
}
