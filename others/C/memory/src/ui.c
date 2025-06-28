#include "../include/memory_trainer.h"
#include <windows.h>
#include <commctrl.h>

// 进度回调函数
void update_progress(int percent) {
    HWND hProgress = GetDlgItem(g_hMainWnd, IDC_PROGRESS);
    if (hProgress) {
        SendMessage(hProgress, PBM_SETPOS, percent, 0);
    }
}

// 初始化主窗口UI
void init_main_window_ui(HWND hWnd) {
    g_hMainWnd = hWnd;
    
    // 创建内存大小输入框
    CreateWindowW(L"STATIC", L"内存大小(MB):", 
                 WS_VISIBLE | WS_CHILD,
                 20, 20, 100, 20, hWnd, NULL, NULL, NULL);
    
    CreateWindowW(L"EDIT", L"100",
                 WS_VISIBLE | WS_CHILD | WS_BORDER | ES_NUMBER,
                 130, 20, 80, 20, hWnd, NULL, NULL, NULL);
    
    // 创建填充模式单选按钮
    CreateWindowW(L"STATIC", L"填充模式:", 
                 WS_VISIBLE | WS_CHILD,
                 20, 50, 100, 20, hWnd, NULL, NULL, NULL);
    
    CreateWindowW(L"BUTTON", L"填充0", 
                 WS_VISIBLE | WS_CHILD | BS_AUTORADIOBUTTON | WS_GROUP,
                 130, 50, 80, 20, hWnd, NULL, NULL, NULL);
    
    CreateWindowW(L"BUTTON", L"填充随机", 
                 WS_VISIBLE | WS_CHILD | BS_AUTORADIOBUTTON,
                 220, 50, 80, 20, hWnd, NULL, NULL, NULL);
    
    // 创建内存保持选项(更显眼的位置)
    CreateWindowW(L"BUTTON", L"✔ 保持内存", 
                 WS_VISIBLE | WS_CHILD | BS_AUTOCHECKBOX,
                 130, 80, 100, 20, hWnd, (HMENU)IDC_RETAIN_MEM, NULL, NULL);
    
    // 创建进度条(更显眼的位置和大小)
    CreateWindowW(PROGRESS_CLASSW, NULL,
                 WS_VISIBLE | WS_CHILD | PBS_SMOOTH,
                 20, 110, 360, 25, hWnd, (HMENU)IDC_PROGRESS, NULL, NULL);
    SendDlgItemMessage(hWnd, IDC_PROGRESS, PBM_SETRANGE, 0, MAKELPARAM(0, 100));
    
    // 创建操作按钮
    CreateWindowW(L"BUTTON", L"执行内存测试", 
                 WS_VISIBLE | WS_CHILD | BS_DEFPUSHBUTTON,
                 20, 140, 280, 30, hWnd, (HMENU)IDC_RUN_TEST, NULL, NULL);
}

// 处理UI消息
LRESULT handle_ui_message(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
        case WM_CREATE:
            init_main_window_ui(hWnd);
            break;
            
        default:
            return DefWindowProcW(hWnd, message, wParam, lParam);
    }
    return 0;
}
