#include <windows.h>
#include <tchar.h>
#include <locale.h>
#include "system_info.h"
#include "window_utils.h"
#include "logging.h"

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);
    UNREFERENCED_PARAMETER(nCmdShow);
    
    // 设置控制台编码为UTF-8
    SetConsoleOutputCP(65001);
    setlocale(LC_ALL, "chs");

    // 初始化日志系统
    log_message(LOG_INFO, "应用程序启动");

    // 初始化系统信息收集
    SystemInfo sysInfo;
    init_system_info(&sysInfo);

    // 创建并显示主窗口，传递UTF-8编码标识
    int result = create_main_window(hInstance, &sysInfo, 65001);

    log_message(LOG_INFO, "应用程序退出");
    return result;
}
