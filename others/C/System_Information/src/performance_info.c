#include "performance_info.h"
#include <pdh.h>
#include <psapi.h>



#include "performance_info.h"
#include <windows.h>
#include <psapi.h>

// 定义函数指针类型
typedef PDH_STATUS (WINAPI *PdhOpenQueryFunc)(_In_opt_ LPCSTR, _In_ DWORD_PTR, _Out_ PDH_HQUERY*);
typedef PDH_STATUS (WINAPI *PdhAddCounterFunc)(_In_ PDH_HQUERY, _In_ LPCSTR, _In_ DWORD_PTR, _Out_ PDH_HCOUNTER*);
typedef PDH_STATUS (WINAPI *PdhCollectQueryDataFunc)(_In_ PDH_HQUERY);
typedef PDH_STATUS (WINAPI *PdhGetFormattedCounterValueFunc)(_In_ PDH_HCOUNTER, _In_ DWORD, _Out_opt_ LPDWORD, _Out_ PPDH_FMT_COUNTERVALUE);

// 安全的函数指针转换函数
static FARPROC safe_get_proc_address(HMODULE module, const char* name) {
    FARPROC proc = GetProcAddress(module, name);
    if (!proc) {
        return NULL;
    }
    return proc;
}

// 辅助宏用于安全的函数指针转换
#define GET_PROC_ADDRESS(module, name, type) \
    ((type)(void*)safe_get_proc_address(module, name))

static PDH_HQUERY cpuQuery;
static PDH_HCOUNTER cpuTotal;
static HMODULE hPdhModule = NULL;
static PdhOpenQueryFunc pPdhOpenQuery = NULL;
static PdhAddCounterFunc pPdhAddCounter = NULL;
static PdhCollectQueryDataFunc pPdhCollectQueryData = NULL;
static PdhGetFormattedCounterValueFunc pPdhGetFormattedCounterValue = NULL;

void init_performance_counters() {
    hPdhModule = LoadLibrary("pdh.dll");
    if (!hPdhModule) {
        return;
    }

    pPdhOpenQuery = GET_PROC_ADDRESS(hPdhModule, "PdhOpenQueryA", PdhOpenQueryFunc);
    pPdhAddCounter = GET_PROC_ADDRESS(hPdhModule, "PdhAddCounterA", PdhAddCounterFunc);
    pPdhCollectQueryData = GET_PROC_ADDRESS(hPdhModule, "PdhCollectQueryData", PdhCollectQueryDataFunc);
    pPdhGetFormattedCounterValue = GET_PROC_ADDRESS(hPdhModule, "PdhGetFormattedCounterValue", PdhGetFormattedCounterValueFunc);

    if (pPdhOpenQuery && pPdhAddCounter && pPdhCollectQueryData) {
        // 初始化性能计数器
        if (pPdhOpenQuery(NULL, 0, &cpuQuery) != ERROR_SUCCESS) {
            return;
        }
        
        // 添加CPU计数器
        if (pPdhAddCounter(cpuQuery, "\\Processor(_Total)\\% Processor Time", 0, &cpuTotal) != ERROR_SUCCESS) {
            return;
        }
        
        // 第一次收集数据，用于初始化
        pPdhCollectQueryData(cpuQuery);
        Sleep(1000); // 等待1秒获取基准数据
        pPdhCollectQueryData(cpuQuery);
    }
}

void get_performance_info(PerformanceInfo* perfInfo) {
    static BOOL initialized = FALSE;
    if (!initialized) {
        init_performance_counters();
        initialized = TRUE;
    }

    // 获取CPU使用率
    if (pPdhCollectQueryData && pPdhGetFormattedCounterValue) {
        // 第一次收集数据作为基准
        pPdhCollectQueryData(cpuQuery);
        Sleep(1000); // 等待1秒
        pPdhCollectQueryData(cpuQuery); // 第二次收集数据
        
        PDH_FMT_COUNTERVALUE counterVal;
        if (pPdhGetFormattedCounterValue(cpuTotal, PDH_FMT_DOUBLE, NULL, &counterVal) == ERROR_SUCCESS) {
            perfInfo->cpuUsage = (DWORD)counterVal.doubleValue;
        } else {
            perfInfo->cpuUsage = 0;
        }
    } else {
        // 如果PDH不可用，使用GetSystemTimes作为备用方案
        FILETIME idleTime, kernelTime, userTime;
        if (GetSystemTimes(&idleTime, &kernelTime, &userTime)) {
            ULONGLONG idle = ((ULONGLONG)idleTime.dwHighDateTime << 32) | idleTime.dwLowDateTime;
            ULONGLONG kernel = ((ULONGLONG)kernelTime.dwHighDateTime << 32) | kernelTime.dwLowDateTime;
            ULONGLONG user = ((ULONGLONG)userTime.dwHighDateTime << 32) | userTime.dwLowDateTime;
            
            static ULONGLONG prevIdle = 0, prevKernel = 0, prevUser = 0;
            ULONGLONG idleDiff = idle - prevIdle;
            ULONGLONG kernelDiff = kernel - prevKernel;
            ULONGLONG userDiff = user - prevUser;
            
            if (prevIdle != 0 && (kernelDiff + userDiff) > 0) {
                perfInfo->cpuUsage = (DWORD)(100.0 - (100.0 * idleDiff) / (kernelDiff + userDiff));
            } else {
                perfInfo->cpuUsage = 0;
            }
            
            prevIdle = idle;
            prevKernel = kernel;
            prevUser = user;
        } else {
            perfInfo->cpuUsage = 0;
        }
    }

    // 获取内存使用率
    MEMORYSTATUSEX memInfo;
    memInfo.dwLength = sizeof(MEMORYSTATUSEX);
    GlobalMemoryStatusEx(&memInfo);
    // 使用GlobalMemoryStatusEx获取更精确的内存使用率
    MEMORYSTATUSEX memStatus;
    memStatus.dwLength = sizeof(memStatus);
    GlobalMemoryStatusEx(&memStatus);
    perfInfo->memoryUsage = memStatus.dwMemoryLoad;

    // 获取进程和线程数量
    PERFORMANCE_INFORMATION perfInfoStruct;
    GetPerformanceInfo(&perfInfoStruct, sizeof(perfInfoStruct));
    perfInfo->processesCount = perfInfoStruct.ProcessCount;
    perfInfo->threadsCount = perfInfoStruct.ThreadCount;
    perfInfo->handlesCount = perfInfoStruct.HandleCount;
}

void cleanup_performance_counters() {
    if (hPdhModule) {
        FreeLibrary(hPdhModule);
        hPdhModule = NULL;
    }
}