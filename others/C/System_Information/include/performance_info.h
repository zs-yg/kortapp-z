#ifndef PERFORMANCE_INFO_H
#define PERFORMANCE_INFO_H

#include <windows.h>
#include <pdh.h>

// 确保PDH函数声明
#ifndef _PDH_H_
#define _PDH_H_
PDH_STATUS PdhOpenQueryA(LPCSTR szDataSource, DWORD_PTR dwUserData, PDH_HQUERY* phQuery);
PDH_STATUS PdhAddCounterA(PDH_HQUERY hQuery, LPCSTR szFullCounterPath, DWORD_PTR dwUserData, PDH_HCOUNTER* phCounter);
PDH_STATUS PdhCollectQueryData(PDH_HQUERY hQuery);
PDH_STATUS PdhGetFormattedCounterValue(PDH_HCOUNTER hCounter, DWORD dwFormat, LPDWORD lpdwType, PPDH_FMT_COUNTERVALUE pValue);
#endif

typedef struct {
    DWORD cpuUsage;         // CPU使用率百分比
    DWORD memoryUsage;       // 内存使用率百分比
    DWORD processesCount;   // 进程数量
    DWORD threadsCount;      // 线程数量
    DWORD handlesCount;      // 句柄数量
} PerformanceInfo;

void get_performance_info(PerformanceInfo* perfInfo);

#endif // PERFORMANCE_INFO_H
