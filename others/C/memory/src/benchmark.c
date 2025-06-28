#include "../include/benchmark.h"
#include "../include/utils.h"
#include "../include/memory_trainer.h"
#include <windows.h>
#include <stdio.h>

// 运行内存性能测试
BenchmarkResult run_memory_benchmark(size_t size_mb) {
    BenchmarkResult result = {0};
    LARGE_INTEGER freq, start, end;
    QueryPerformanceFrequency(&freq);

    // 测试内存分配
    QueryPerformanceCounter(&start);
    void* ptr = allocate_memory(size_mb);
    QueryPerformanceCounter(&end);
    result.allocation_time = (end.QuadPart - start.QuadPart) * 1000.0 / freq.QuadPart;

    // 测试内存填充
    QueryPerformanceCounter(&start);
    fill_memory(ptr, mb_to_bytes(size_mb), 0, NULL);
    QueryPerformanceCounter(&end);
    result.fill_time = (end.QuadPart - start.QuadPart) * 1000.0 / freq.QuadPart;

    // 测试内存释放
    QueryPerformanceCounter(&start);
    free_memory(ptr, size_mb);
    QueryPerformanceCounter(&end);
    result.free_time = (end.QuadPart - start.QuadPart) * 1000.0 / freq.QuadPart;

    return result;
}

// 打印测试结果(支持中文)
void print_benchmark_result(const BenchmarkResult* result) {
    wprintf(L"=== 内存性能测试结果 ===\n");
    wprintf(L"分配时间: %.2f 毫秒\n", result->allocation_time);
    wprintf(L"填充时间: %.2f 毫秒\n", result->fill_time);
    wprintf(L"释放时间: %.2f 毫秒\n", result->free_time);
}
