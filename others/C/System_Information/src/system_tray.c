#include "system_tray.h"
#include <shellapi.h>
#include <strsafe.h>

void create_tray_icon(HWND hWnd, HICON hIcon) {
    NOTIFYICONDATA nid = {0};
    nid.cbSize = sizeof(NOTIFYICONDATA);
    nid.hWnd = hWnd;
    nid.uID = ID_TRAYICON;
    nid.uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP;
    nid.uCallbackMessage = WM_TRAYICON;
    nid.hIcon = hIcon;
    #if defined(UNICODE) || defined(_UNICODE)
    StringCbCopyW(nid.szTip, sizeof(nid.szTip), L"系统信息查看器");
    #else
    StringCbCopyA(nid.szTip, sizeof(nid.szTip), "系统信息查看器");
    #endif
    
    Shell_NotifyIcon(NIM_ADD, &nid);
}

void update_tray_icon(HWND hWnd, HICON hIcon, LPCTSTR tooltip) {
    NOTIFYICONDATA nid = {0};
    nid.cbSize = sizeof(NOTIFYICONDATA);
    nid.hWnd = hWnd;
    nid.uID = ID_TRAYICON;
    nid.uFlags = NIF_ICON | NIF_TIP;
    nid.hIcon = hIcon;
    #if defined(UNICODE) || defined(_UNICODE)
    StringCbCopyW(nid.szTip, sizeof(nid.szTip), tooltip);
    #else
    StringCbCopyA(nid.szTip, sizeof(nid.szTip), tooltip);
    #endif
    
    Shell_NotifyIcon(NIM_MODIFY, &nid);
}

void remove_tray_icon(HWND hWnd) {
    NOTIFYICONDATA nid = {0};
    nid.cbSize = sizeof(NOTIFYICONDATA);
    nid.hWnd = hWnd;
    nid.uID = ID_TRAYICON;
    
    Shell_NotifyIcon(NIM_DELETE, &nid);
}
