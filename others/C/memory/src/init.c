#include "../include/memory_trainer.h"
#include "../include/log.h"
#include <windows.h>

// 初始化应用程序
void initialize_application(HINSTANCE hInstance) {
    // 初始化日志系统
    init_logger();
    
    // 注册窗口类(Unicode版本)
    WNDCLASSW wc = {0};
    wc.lpfnWndProc = WndProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = L"MemoryTrainer";
    RegisterClassW(&wc);
    
    // 记录初始化完成
    log_message(LOG_INFO, L"应用程序初始化完成");
}

// 获取应用程序实例句柄
HINSTANCE get_app_instance(void) {
    return GetModuleHandleW(NULL);
}
