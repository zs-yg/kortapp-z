#ifndef MEMORY_TRAINER_H
#define MEMORY_TRAINER_H

#include <stddef.h>
#include <windows.h>

// 错误代码定义
typedef enum {
    ERR_SUCCESS = 0,
    ERR_ALLOCATION,
    ERR_INVALID_PARAM,
    ERR_SYSTEM
} ErrorCode;

// 进度回调函数类型
typedef void (*ProgressCallback)(int percent);

// 内存操作函数声明
void* allocate_memory(size_t size);
void fill_memory(void* ptr, size_t size, int mode, ProgressCallback callback);
void free_memory(void* ptr, size_t size);

// 错误处理函数
void report_error(ErrorCode code, const wchar_t* message);

// 全局窗口句柄(外部声明)
extern HWND g_hMainWnd;

// 进度回调函数
void update_progress(int percent);

// UI控件ID定义
#define IDC_RUN_TEST 1001
#define IDC_PROGRESS 1002
#define IDC_RETAIN_MEM 1003
#define IDC_FILL_ZERO 1004
#define IDC_FILL_RANDOM 1005

// 窗口过程函数声明
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

// UI函数声明
void init_main_window_ui(HWND hWnd);
LRESULT handle_ui_message(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

// 初始化函数声明
void initialize_application(HINSTANCE hInstance);
HINSTANCE get_app_instance(void);

// 版本信息函数
const wchar_t* get_version_string(void);
const wchar_t* get_build_date(void);

#endif // MEMORY_TRAINER_H
