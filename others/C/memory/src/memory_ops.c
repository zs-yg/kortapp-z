#include "../include/memory_trainer.h"
#include <windows.h>
#include <stdlib.h>
#include <time.h>
#include <stdint.h>

// 分配指定大小的内存
void* allocate_memory(size_t size_bytes) {
    if (size_bytes == 0) {
        report_error(ERR_INVALID_PARAM, L"无效的内存大小");
        return NULL;
    }
    
    // 获取系统内存状态
    MEMORYSTATUSEX memStatus;
    memStatus.dwLength = sizeof(memStatus);
    GlobalMemoryStatusEx(&memStatus);
    
    // 统一debugMsg变量声明
    wchar_t debugMsg[512];
    
    // 详细调试输出
    wsprintfW(debugMsg, L"开始内存分配...\n请求大小: %zu bytes (%.2f MB)\n系统总内存: %.2f GB\n可用内存: %.2f GB\n",
            size_bytes, 
            (double)size_bytes / (1024 * 1024),
            (double)memStatus.ullTotalPhys / (1024 * 1024 * 1024),
            (double)memStatus.ullAvailPhys / (1024 * 1024 * 1024));
    OutputDebugStringW(debugMsg);
    
    // 分配并锁定内存(调整为系统页面大小的整数倍)
    SYSTEM_INFO sysInfo;
    GetSystemInfo(&sysInfo);
    size_t actual_size = ((size_bytes + sysInfo.dwPageSize - 1) / sysInfo.dwPageSize) * sysInfo.dwPageSize;
    
    wsprintfW(debugMsg, L"实际分配大小: %zu bytes (%.2f MB), 页面大小: %lu\n",
            actual_size, 
            (double)actual_size / (1024 * 1024),
            sysInfo.dwPageSize);
    OutputDebugStringW(debugMsg);
    
    void* ptr = VirtualAlloc(NULL, actual_size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
    if (!ptr) {
        DWORD err = GetLastError();
        wsprintfW(debugMsg, L"内存分配失败，错误代码: %lu, 请求大小: %zu\n", err, actual_size);
        OutputDebugStringW(debugMsg);
        
        LPWSTR errMsg = NULL;
        FormatMessageW(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
                     NULL, err, 0, (LPWSTR)&errMsg, 0, NULL);
        
        wchar_t userMsg[512];
        wsprintfW(userMsg, L"内存分配失败(错误 %lu): %s", err, errMsg ? errMsg : L"未知错误");
        report_error(ERR_ALLOCATION, userMsg);
        
        if (errMsg) LocalFree(errMsg);
        return NULL;
    }
    
    wsprintfW(debugMsg, L"成功分配 %zu 字节内存，地址: %p\n", actual_size, ptr);
    OutputDebugStringW(debugMsg);
    
    // 检查系统内存锁定限制
    SIZE_T minLock, maxLock;
    GetProcessWorkingSetSize(GetCurrentProcess(), &minLock, &maxLock);
    wsprintfW(debugMsg, L"进程工作集大小: 最小=%zu KB, 最大=%zu KB\n",
             minLock/1024, maxLock/1024);
    OutputDebugStringW(debugMsg);

    // 尝试锁定内存(限制锁定大小为工作集最大值的50%)
    size_t lock_size = min(actual_size, maxLock/2);
    if (!VirtualLock(ptr, lock_size)) {
        DWORD err = GetLastError();
        wsprintfW(debugMsg, L"内存锁定失败，错误代码: %lu, 锁定大小: %zu MB\n", 
                err, lock_size/1024/1024);
        OutputDebugStringW(debugMsg);
        
        // 使用系统错误代码继续执行
        report_error(ERR_SYSTEM, L"内存锁定部分失败，继续运行");
    } else {
        wsprintfW(debugMsg, L"成功锁定 %zu MB 内存\n", lock_size/1024/1024);
        OutputDebugStringW(debugMsg);
    }
    OutputDebugStringW(L"内存锁定成功\n");
    
    // 写入所有页确保实际分配
    memset(ptr, 0, actual_size);
    
    return ptr;
}

// 填充内存(带进度回调)
void fill_memory(void* ptr, size_t size_bytes, int mode,
                ProgressCallback progress_callback) {
    if (!ptr) {
        report_error(ERR_INVALID_PARAM, L"空指针");
        return;
    }
    
    // 验证内存范围
    MEMORY_BASIC_INFORMATION mbi;
    if (VirtualQuery(ptr, &mbi, sizeof(mbi)) == 0) {
        report_error(ERR_SYSTEM, L"内存查询失败");
        return;
    }
    
    const size_t block_size = 1024 * 1024; // 1MB块
    size_t filled = 0;
    SYSTEM_INFO sysInfo;
    GetSystemInfo(&sysInfo);
    
    if (mode == 0) { // 填充0
        memset(ptr, 0, size_bytes);
        if (progress_callback) progress_callback(100);
    } else { // 填充随机值
        srand((unsigned)time(NULL));
        uint32_t* block = (uint32_t*)malloc(block_size);
        
        while (filled < size_bytes) {
            size_t remaining = size_bytes - filled;
            size_t current_block = remaining > block_size ? block_size : remaining;
            
            // 批量填充随机值
            for (size_t i = 0; i < current_block/sizeof(uint32_t); i++) {
                block[i] = rand();
            }
            memcpy((char*)ptr + filled, block, current_block);
            
            filled += current_block;
            if (progress_callback) {
                int percent = (int)(filled * 100 / size_bytes);
                progress_callback(percent);
            }
        }
        free(block);
    }
}

// 释放内存
void free_memory(void* ptr, size_t size_bytes) {
    if (ptr) {
        size_t actual_size = size_bytes + 4096;
        VirtualUnlock(ptr, actual_size);
        VirtualFree(ptr, 0, MEM_RELEASE);
    }
}
