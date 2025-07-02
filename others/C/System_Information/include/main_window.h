#ifndef MAIN_WINDOW_H
#define MAIN_WINDOW_H

#include <windows.h>
#include "system_info.h"

void update_main_window(HWND hWnd, SystemInfo* sysInfo);
void toggle_fullscreen(HWND hWnd);
LRESULT CALLBACK MainWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

#endif // MAIN_WINDOW_H
