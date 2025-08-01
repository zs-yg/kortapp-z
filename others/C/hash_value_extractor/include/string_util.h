#ifndef STRING_UTIL_H
#define STRING_UTIL_H

#include <stddef.h>

// 快速分配字符串内存
char* str_alloc(size_t size);

// 快速释放字符串内存
void str_free(char* str);

// 快速字符串复制
char* str_copy(const char* src);

// 快速字符串连接
char* str_concat(const char* str1, const char* str2);

// 二进制转十六进制字符串
char* bin_to_hex(const unsigned char* data, size_t len);

#endif // STRING_UTIL_H
