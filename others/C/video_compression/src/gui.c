#include "gui.h"
#include "video_compressor.h"
#include "string_utils.h"
#include <windows.h>
#include <commctrl.h>

#define ID_COMPRESS_BUTTON 101
#define ID_QUALITY_SLIDER 102
#define ID_INPUT_FILE_EDIT 103
#define ID_OUTPUT_FILE_EDIT 104
#define ID_BROWSE_INPUT_BUTTON 105
#define ID_BROWSE_OUTPUT_BUTTON 106

// 全局变量
static HWND g_hwndQualitySlider;
static HWND g_hwndInputFileEdit;
static HWND g_hwndOutputFileEdit;
static HFONT g_hFont = NULL;

// 设置中文字体
static void SetChineseFont(HWND hwnd) {
    if (g_hFont == NULL) {
        g_hFont = CreateFontW(
            16, 0, 0, 0, FW_NORMAL, FALSE, FALSE, FALSE,
            GB2312_CHARSET,
            OUT_DEFAULT_PRECIS,
            CLIP_DEFAULT_PRECIS,
            DEFAULT_QUALITY,
            DEFAULT_PITCH | FF_SWISS,
            L"Microsoft YaHei"
        );
    }
    if (g_hFont) {
        SendMessageW(hwnd, WM_SETFONT, (WPARAM)g_hFont, TRUE);
    }
}

BOOL init_gui(HINSTANCE hInstance) {
    // 注册窗口类
    WNDCLASSEXW wc = {0};
    wc.cbSize = sizeof(WNDCLASSEXW);
    wc.style = CS_HREDRAW | CS_VREDRAW;
    wc.lpfnWndProc = MainWndProc;
    wc.hInstance = hInstance;
    wc.hCursor = LoadCursorW(NULL, MAKEINTRESOURCEW(IDC_ARROW));
    wc.hbrBackground = (HBRUSH)(COLOR_WINDOW+1);
    wc.lpszClassName = L"VideoCompressorClass";
    
    if (!RegisterClassExW(&wc)) {
        return FALSE;
    }

    // 创建主窗口
    HWND hwnd = CreateWindowExW(0, wc.lpszClassName, L"视频压缩工具",
                               WS_OVERLAPPEDWINDOW, CW_USEDEFAULT, CW_USEDEFAULT,
                               500, 300, NULL, NULL, hInstance, NULL);
    if (!hwnd) {
        return FALSE;
    }

    // 创建控件
    create_compression_controls(hwnd);

    ShowWindow(hwnd, SW_SHOW);
    UpdateWindow(hwnd);
    return TRUE;
}

void create_compression_controls(HWND hwnd) {
    // 设置字体
    SetChineseFont(hwnd);

    // 创建输入文件选择控件
    CreateWindowW(L"STATIC", L"输入文件:", WS_CHILD | WS_VISIBLE,
                20, 20, 80, 20, hwnd, NULL, NULL, NULL);
    
    g_hwndInputFileEdit = CreateWindowW(L"EDIT", L"", WS_CHILD | WS_VISIBLE | WS_BORDER,
                                     110, 20, 250, 20, hwnd, (HMENU)ID_INPUT_FILE_EDIT, NULL, NULL);
    CreateWindowW(L"BUTTON", L"浏览...", WS_CHILD | WS_VISIBLE,
                                     370, 20, 60, 20, hwnd, (HMENU)ID_BROWSE_INPUT_BUTTON, NULL, NULL);

    // 创建输出文件选择控件
    CreateWindowW(L"STATIC", L"输出文件:", WS_CHILD | WS_VISIBLE,
                20, 50, 80, 20, hwnd, NULL, NULL, NULL);
    
    g_hwndOutputFileEdit = CreateWindowW(L"EDIT", L"", WS_CHILD | WS_VISIBLE | WS_BORDER,
                                      110, 50, 250, 20, hwnd, (HMENU)ID_OUTPUT_FILE_EDIT, NULL, NULL);
    CreateWindowW(L"BUTTON", L"浏览...", WS_CHILD | WS_VISIBLE,
                                     370, 50, 60, 20, hwnd, (HMENU)ID_BROWSE_OUTPUT_BUTTON, NULL, NULL);

    // 创建质量滑块
    CreateWindowW(L"STATIC", L"压缩质量(1-10000):", WS_CHILD | WS_VISIBLE,
                20, 80, 150, 20, hwnd, NULL, NULL, NULL);
    
    g_hwndQualitySlider = CreateWindowW(TRACKBAR_CLASSW, L"", WS_CHILD | WS_VISIBLE | TBS_AUTOTICKS,
                                     20, 100, 400, 30, hwnd, (HMENU)ID_QUALITY_SLIDER, NULL, NULL);
    
    SendMessageW(g_hwndQualitySlider, TBM_SETRANGE, TRUE, MAKELONG(1, 10000));
    SendMessageW(g_hwndQualitySlider, TBM_SETPOS, TRUE, 5000);

    // 创建压缩按钮
    CreateWindowW(L"BUTTON", L"开始压缩", WS_CHILD | WS_VISIBLE | BS_DEFPUSHBUTTON,
                180, 150, 100, 30, hwnd, (HMENU)ID_COMPRESS_BUTTON, NULL, NULL);
}

LRESULT CALLBACK MainWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    switch (msg) {
        case WM_COMMAND:
            if (LOWORD(wParam) == ID_COMPRESS_BUTTON) {
                // 处理压缩按钮点击事件
                int quality = (int)SendMessageW(g_hwndQualitySlider, TBM_GETPOS, 0, 0);
                
                wchar_t inputFile[MAX_PATH];
                GetWindowTextW(g_hwndInputFileEdit, inputFile, MAX_PATH);
                
                wchar_t outputFile[MAX_PATH];
                GetWindowTextW(g_hwndOutputFileEdit, outputFile, MAX_PATH);
                
                // 转换为UTF-8
                char inputFileUtf8[MAX_PATH * 4];
                char outputFileUtf8[MAX_PATH * 4];
                WideCharToMultiByte(CP_UTF8, 0, inputFile, -1, inputFileUtf8, sizeof(inputFileUtf8), NULL, NULL);
                WideCharToMultiByte(CP_UTF8, 0, outputFile, -1, outputFileUtf8, sizeof(outputFileUtf8), NULL, NULL);
                
                // 调用压缩函数
                compress_video(inputFileUtf8, outputFileUtf8, quality);
                return 0;
            } else if (LOWORD(wParam) == ID_BROWSE_INPUT_BUTTON || LOWORD(wParam) == ID_BROWSE_OUTPUT_BUTTON) {
                // 文件选择对话框
                OPENFILENAMEW ofn;
                wchar_t szFile[MAX_PATH] = L"";

                ZeroMemory(&ofn, sizeof(ofn));
                ofn.lStructSize = sizeof(ofn);
                ofn.hwndOwner = hwnd;
                ofn.lpstrFile = szFile;
                ofn.nMaxFile = sizeof(szFile)/sizeof(szFile[0]);
                ofn.lpstrFilter = L"视频文件\0*.mp4;*.avi;*.mkv;*.mov\0所有文件\0*.*\0";
                ofn.nFilterIndex = 1;
                ofn.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;

                if (GetOpenFileNameW(&ofn)) {
                    HWND targetEdit = (LOWORD(wParam) == ID_BROWSE_INPUT_BUTTON) ? 
                                     g_hwndInputFileEdit : g_hwndOutputFileEdit;
                    SetWindowTextW(targetEdit, szFile);
                }
                return 0;
            }
            break;
            
        case WM_DESTROY:
            if (g_hFont) {
                DeleteObject(g_hFont);
                g_hFont = NULL;
            }
            PostQuitMessage(0);
            return 0;
    }
    
    return DefWindowProcW(hwnd, msg, wParam, lParam);
}
