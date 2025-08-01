// 确保使用Unicode字符集
#define UNICODE
#define _UNICODE

#include <windows.h>
#include <commdlg.h>
#include "hash_calculator.h"
#include <openssl/evp.h>
#include <wchar.h>

#define IDC_ALGORITHM_COMBO 1001
#define IDC_FILE_EDIT 1002
#define IDC_HASH_EDIT 1003
#define IDC_BROWSE_BUTTON 1004
#define IDC_CALCULATE_BUTTON 1005
#define IDC_COPY_BUTTON 1006

// 中文UI字符串定义
static const wchar_t* APP_TITLE = L"哈希值提取器";
static const wchar_t* ALGORITHMS[] = {L"MD5", L"SHA-256", L"SHA-512"};
static const wchar_t* CALCULATE_BTN = L"计算哈希";
static const wchar_t* BROWSE_BTN = L"浏览...";
static const wchar_t* COPY_BTN = L"复制哈希值";
static const wchar_t* CALC_FAILED = L"计算哈希值失败";

// 全局变量
HWND g_hAlgorithmCombo, g_hFileEdit, g_hHashEdit;

// 初始化控件
void InitControls(HWND hWnd) {
    // 算法选择下拉框
    g_hAlgorithmCombo = CreateWindowW(L"COMBOBOX", NULL, 
        WS_CHILD | WS_VISIBLE | CBS_DROPDOWNLIST | WS_VSCROLL,
        10, 10, 200, 200, hWnd, (HMENU)IDC_ALGORITHM_COMBO, NULL, NULL);
    
    // 添加算法选项
    SendMessageW(g_hAlgorithmCombo, CB_ADDSTRING, 0, (LPARAM)L"MD5");
    SendMessageW(g_hAlgorithmCombo, CB_ADDSTRING, 0, (LPARAM)L"SHA-256");
    SendMessageW(g_hAlgorithmCombo, CB_ADDSTRING, 0, (LPARAM)L"SHA-512");
    SendMessageW(g_hAlgorithmCombo, CB_SETCURSEL, 0, 0);
    
    // 文件路径编辑框
    g_hFileEdit = CreateWindowW(L"EDIT", NULL, 
        WS_CHILD | WS_VISIBLE | WS_BORDER | ES_AUTOHSCROLL,
        10, 40, 300, 25, hWnd, (HMENU)IDC_FILE_EDIT, NULL, NULL);
    
    // 浏览按钮
    CreateWindowW(L"BUTTON", L"浏览...", 
        WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
        320, 40, 80, 25, hWnd, (HMENU)IDC_BROWSE_BUTTON, NULL, NULL);
    
    // 计算按钮
    CreateWindowW(L"BUTTON", L"计算哈希", 
        WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
        10, 75, 100, 30, hWnd, (HMENU)IDC_CALCULATE_BUTTON, NULL, NULL);
    
    // 哈希结果显示框
    g_hHashEdit = CreateWindowW(L"EDIT", NULL, 
        WS_CHILD | WS_VISIBLE | WS_BORDER | ES_MULTILINE | ES_AUTOVSCROLL | ES_READONLY,
        10, 115, 380, 100, hWnd, (HMENU)IDC_HASH_EDIT, NULL, NULL);
    
    // 复制按钮
    CreateWindowW(L"BUTTON", L"复制哈希值", 
        WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
        10, 225, 100, 30, hWnd, (HMENU)IDC_COPY_BUTTON, NULL, NULL);
}

// 选择文件
void BrowseFile(HWND hWnd) {
    OPENFILENAMEW ofn;
    WCHAR szFile[MAX_PATH] = {0};
    
    ZeroMemory(&ofn, sizeof(ofn));
    ofn.lStructSize = sizeof(ofn);
    ofn.hwndOwner = hWnd;
    ofn.lpstrFile = szFile;
    ofn.nMaxFile = sizeof(szFile)/sizeof(szFile[0]);
    ofn.lpstrFilter = L"所有文件\0*.*\0";
    ofn.nFilterIndex = 1;
    ofn.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;
    
    if (GetOpenFileNameW(&ofn)) {
        SetWindowTextW(g_hFileEdit, szFile);
    }
}

// 计算哈希值
void CalculateHash() {
    WCHAR wszFile[MAX_PATH];
    char szFile[MAX_PATH];
    char szHash[EVP_MAX_MD_SIZE * 2 + 1];
    
    GetWindowTextW(g_hFileEdit, wszFile, MAX_PATH);
    WideCharToMultiByte(CP_UTF8, 0, wszFile, -1, szFile, MAX_PATH, NULL, NULL);
    
    int algorithm = SendMessageW(g_hAlgorithmCombo, CB_GETCURSEL, 0, 0);
    
    if (calculate_file_hash(szFile, algorithm, szHash) == 0) {
        WCHAR wszHash[EVP_MAX_MD_SIZE * 2 + 1];
        MultiByteToWideChar(CP_UTF8, 0, szHash, -1, wszHash, EVP_MAX_MD_SIZE * 2 + 1);
        SetWindowTextW(g_hHashEdit, wszHash);
    } else {
        SetWindowTextW(g_hHashEdit, L"计算哈希值失败");
    }
}

// 复制哈希值到剪贴板
void CopyHashToClipboard() {
    if (OpenClipboard(NULL)) {
        EmptyClipboard();
        
        int len = GetWindowTextLengthW(g_hHashEdit) + 1;
        HGLOBAL hMem = GlobalAlloc(GMEM_MOVEABLE, len * sizeof(WCHAR));
        
        if (hMem) {
            WCHAR* pszMem = (WCHAR*)GlobalLock(hMem);
            GetWindowTextW(g_hHashEdit, pszMem, len);
            GlobalUnlock(hMem);
            
            SetClipboardData(CF_UNICODETEXT, hMem);
        }
        
        CloseClipboard();
    }
}

// 窗口过程
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
        case WM_CREATE:
            InitControls(hWnd);
            break;
            
        case WM_COMMAND:
            switch (LOWORD(wParam)) {
                case IDC_BROWSE_BUTTON:
                    BrowseFile(hWnd);
                    break;
                    
                case IDC_CALCULATE_BUTTON:
                    CalculateHash();
                    break;
                    
                case IDC_COPY_BUTTON:
                    CopyHashToClipboard();
                    break;
            }
            break;
            
        case WM_DESTROY:
            PostQuitMessage(0);
            break;
            
        default:
            return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// 程序入口
int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
    // 注册窗口类
    WNDCLASSEXW wcex;
    wcex.cbSize = sizeof(WNDCLASSEX);
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = WndProc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = hInstance;
    wcex.hIcon = LoadIcon(NULL, IDI_APPLICATION);
    wcex.hCursor = LoadCursor(NULL, IDC_ARROW);
    wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName = NULL;
    wcex.lpszClassName = L"HashValueExtractor";
    wcex.hIconSm = LoadIcon(NULL, IDI_APPLICATION);
    
    if (!RegisterClassExW(&wcex)) {
        return 1;
    }
    
    // 创建窗口
    HWND hWnd = CreateWindowW(
        L"HashValueExtractor", L"哈希值提取器",
        WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, CW_USEDEFAULT, 420, 300,
        NULL, NULL, hInstance, NULL);
    
    if (!hWnd) {
        return 1;
    }
    
    // 显示窗口
    ShowWindow(hWnd, nCmdShow);
    UpdateWindow(hWnd);
    
    // 消息循环
    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
    
    return (int)msg.wParam;
}
