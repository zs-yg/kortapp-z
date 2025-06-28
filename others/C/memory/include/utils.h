#ifndef UTILS_H
#define UTILS_H

#include <stddef.h>

// 字节转换工具：MB转字节
size_t mb_to_bytes(size_t mb);

// 获取当前时间戳(毫秒)
long long get_timestamp();

// 打印调试信息(支持中文)
void debug_print(const wchar_t* message);

#endif // UTILS_H
