#ifndef SYSTEM_INFO_H
#define SYSTEM_INFO_H

#include <windows.h>

typedef struct {
    char cpuName[256];
    DWORD cpuCores;
    DWORD cpuThreads;
    MEMORYSTATUSEX memoryStatus;
    SYSTEM_INFO systemInfo;
    OSVERSIONINFOEX osVersion;
} SystemInfo;

// 初始化系统信息
void init_system_info(SystemInfo* sysInfo);

// 创建主窗口
int create_main_window(HINSTANCE hInstance, SystemInfo* sysInfo, UINT codePage);

#endif // SYSTEM_INFO_H
