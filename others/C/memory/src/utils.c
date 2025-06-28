#include "../include/utils.h"
#include <windows.h>
#include <stdio.h>

// MB转字节
size_t mb_to_bytes(size_t mb) {
    return mb * 1024 * 1024;
}

// 获取当前时间戳(毫秒)
long long get_timestamp() {
    SYSTEMTIME st;
    GetSystemTime(&st);
    return (long long)st.wMilliseconds + 
           st.wSecond * 1000LL + 
           st.wMinute * 60000LL + 
           st.wHour * 3600000LL;
}

// 打印调试信息(支持中文)
void debug_print(const wchar_t* message) {
    wprintf(L"[DEBUG] %s\n", message);
}
