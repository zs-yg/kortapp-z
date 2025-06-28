#ifndef BENCHMARK_H
#define BENCHMARK_H

#include <stddef.h>

// 内存性能测试结果
typedef struct {
    double allocation_time;  // 分配时间(ms)
    double fill_time;        // 填充时间(ms)
    double free_time;        // 释放时间(ms)
} BenchmarkResult;

// 运行内存性能测试
BenchmarkResult run_memory_benchmark(size_t size_mb);

// 打印测试结果(支持中文)
void print_benchmark_result(const BenchmarkResult* result);

#endif // BENCHMARK_H
