#include "system_info.h"
#include <intrin.h>

void init_system_info(SystemInfo* sysInfo) {
    // 获取CPU信息
    int cpuInfo[4] = {0};
    __cpuid(cpuInfo, 0x80000002);
    memcpy(sysInfo->cpuName, cpuInfo, sizeof(cpuInfo));
    __cpuid(cpuInfo, 0x80000003);
    memcpy(sysInfo->cpuName + 16, cpuInfo, sizeof(cpuInfo));
    __cpuid(cpuInfo, 0x80000004);
    memcpy(sysInfo->cpuName + 32, cpuInfo, sizeof(cpuInfo));

    // 获取系统信息
    GetSystemInfo(&sysInfo->systemInfo);
    sysInfo->cpuCores = sysInfo->systemInfo.dwNumberOfProcessors;
    sysInfo->cpuThreads = sysInfo->cpuCores; // 简化处理

    // 获取内存信息
    sysInfo->memoryStatus.dwLength = sizeof(sysInfo->memoryStatus);
    GlobalMemoryStatusEx(&sysInfo->memoryStatus);

    // 获取操作系统版本
    sysInfo->osVersion.dwOSVersionInfoSize = sizeof(sysInfo->osVersion);
    GetVersionEx((LPOSVERSIONINFO)&sysInfo->osVersion);
}
