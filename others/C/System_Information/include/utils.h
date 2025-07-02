#ifndef UTILS_H
#define UTILS_H

#include <windows.h>

// 安全释放内存
#define SAFE_FREE(ptr) if (ptr) { free(ptr); ptr = NULL; }

// 宽字符转多字节字符串
char* wchar_to_mb(const wchar_t* wstr);

// 多字节字符串转宽字符
wchar_t* mb_to_wchar(const char* str);

// 获取当前时间字符串
char* get_current_time_string();

#endif // UTILS_H
