#include <windows.h>
#include "video_compressor.h"
#include "gui.h"

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, 
                   LPSTR lpCmdLine, int nCmdShow)
{
    // 抑制未使用参数警告
    (void)hPrevInstance;
    (void)lpCmdLine;
    (void)nCmdShow;

    // 初始化GUI
    if (!init_gui(hInstance)) {
        return 1;
    }

    // 主消息循环
    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return (int)msg.wParam;
}
