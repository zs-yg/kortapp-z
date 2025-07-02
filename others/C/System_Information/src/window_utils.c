#include "window_utils.h"
#include "main_window.h"
#include <tchar.h>
#include <stdio.h>
#include <io.h>
#include <fcntl.h>

#define IDC_MAIN_BUTTON 1001

// 全局变量存储系统信息
static SystemInfo* g_sysInfo = NULL;

BOOL register_window_class(HINSTANCE hInstance) {
    WNDCLASSEX wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = MainWndProc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = hInstance;
    wcex.hIcon = LoadIcon(hInstance, IDI_APPLICATION);
    wcex.hCursor = LoadCursor(NULL, IDC_ARROW);
    wcex.hbrBackground = CreateSolidBrush(RGB(240, 240, 240));
    wcex.lpszMenuName = NULL;
    wcex.lpszClassName = _T("SystemInfoWindowClass");
    wcex.hIconSm = LoadIcon(hInstance, IDI_APPLICATION);

    return RegisterClassEx(&wcex);
}

int create_main_window(HINSTANCE hInstance, SystemInfo* sysInfo, UINT codePage) {
    // 设置控制台编码
    if (codePage == 65001) {
        SetConsoleOutputCP(65001);
        _setmode(_fileno(stdout), _O_U16TEXT);
    }

    // 设置窗口标题为宽字符
    LPCWSTR windowTitle = L"系统信息查看器";
    g_sysInfo = sysInfo;

    if (!register_window_class(hInstance)) {
        return 0;
    }

        HWND hWnd = CreateWindowW(
        L"SystemInfoWindowClass",
        windowTitle,
        WS_POPUP | WS_VISIBLE,
        CW_USEDEFAULT, CW_USEDEFAULT,
        800, 600,
        NULL, NULL, hInstance, sysInfo);

    if (!hWnd) {
        return 0;
    }

    ShowWindow(hWnd, SW_SHOW);
    UpdateWindow(hWnd);

    // 初始更新窗口内容
    update_main_window(hWnd, sysInfo);

    // 消息循环
    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return (int)msg.wParam;
}

LRESULT CALLBACK MainWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
        case WM_CREATE: {
            // 安全初始化系统信息指针
            if (lParam) {
                g_sysInfo = (SystemInfo*)((CREATESTRUCT*)lParam)->lpCreateParams;
            }
            if (!g_sysInfo) {
                MessageBoxW(hWnd, L"系统信息初始化失败", L"错误", MB_ICONERROR);
                return -1;
            }
            
            // 创建显示系统信息的按钮
            CreateWindowW(L"BUTTON", L"刷新信息",
                         WS_TABSTOP | WS_VISIBLE | WS_CHILD | BS_DEFPUSHBUTTON | WS_BORDER,
                         10, 10, 150, 30,
                         hWnd, (HMENU)IDC_MAIN_BUTTON,
                         (HINSTANCE)GetWindowLongPtr(hWnd, GWLP_HINSTANCE), NULL);
            break;
        }
        case WM_SIZE: {
            // 窗口大小变化时更新布局
            update_main_window(hWnd, g_sysInfo);
            break;
        }
        case WM_COMMAND: {
            if (LOWORD(wParam) == IDC_MAIN_BUTTON) {
                // 刷新系统信息
                init_system_info(g_sysInfo);
                update_main_window(hWnd, g_sysInfo);
            }
            break;
        }
        case WM_KEYDOWN: {
            // F11键切换全屏
            if (wParam == VK_F11) {
                toggle_fullscreen(hWnd);
                update_main_window(hWnd, g_sysInfo);
            }
            break;
        }
        case WM_NCCALCSIZE:
            if (wParam) {
                return 0;
            }
            break;
        case WM_ERASEBKGND: {
            RECT rc;
            GetClientRect(hWnd, &rc);
            FillRect((HDC)wParam, &rc, (HBRUSH)GetClassLongPtr(hWnd, GCLP_HBRBACKGROUND));
            return 1;
        }
        case WM_DESTROY: {
            PostQuitMessage(0);
            break;
        }
        default:
            return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}
