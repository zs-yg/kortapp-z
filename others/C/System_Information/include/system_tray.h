#ifndef SYSTEM_TRAY_H
#define SYSTEM_TRAY_H

#include <windows.h>

#define WM_TRAYICON (WM_USER + 1)
#define ID_TRAYICON 100

void create_tray_icon(HWND hWnd, HICON hIcon);
void update_tray_icon(HWND hWnd, HICON hIcon, LPCTSTR tooltip);
void remove_tray_icon(HWND hWnd);

#endif // SYSTEM_TRAY_H
