#ifndef GUI_H
#define GUI_H

#include <windows.h>

// 初始化GUI界面
BOOL init_gui(HINSTANCE hInstance);

// 主窗口过程函数
LRESULT CALLBACK MainWndProc(HWND hwnd, UINT msg, 
                            WPARAM wParam, LPARAM lParam);

// 创建压缩参数设置控件
void create_compression_controls(HWND hwnd);

#endif // GUI_H
