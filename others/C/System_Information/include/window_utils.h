#ifndef WINDOW_UTILS_H
#define WINDOW_UTILS_H

#include <windows.h>
#include "system_info.h"

// 窗口过程函数
LRESULT CALLBACK MainWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

// 创建主窗口
int create_main_window(HINSTANCE hInstance, SystemInfo* sysInfo, UINT codePage);

// 注册窗口类
BOOL register_window_class(HINSTANCE hInstance);

#endif // WINDOW_UTILS_H
