#ifndef STRING_UTILS_H
#define STRING_UTILS_H

#include <stddef.h>

/**
 * 安全的字符串拷贝
 * @param dest 目标缓冲区
 * @param src 源字符串
 * @param dest_size 目标缓冲区大小
 * @return 目标字符串
 */
char* str_copy(char* dest, const char* src, size_t dest_size);

/**
 * 安全的字符串连接
 * @param dest 目标缓冲区
 * @param src 要连接的字符串
 * @param dest_size 目标缓冲区大小
 * @return 目标字符串
 */
char* str_concat(char* dest, const char* src, size_t dest_size);

/**
 * 去除字符串两端的空白字符
 * @param str 要处理的字符串
 * @return 处理后的字符串
 */
char* str_trim(char* str);

/**
 * 检查字符串是否以指定前缀开头
 * @param str 要检查的字符串
 * @param prefix 前缀
 * @return 1表示是，0表示否
 */
int str_starts_with(const char* str, const char* prefix);

/**
 * 检查字符串是否以指定后缀结尾
 * @param str 要检查的字符串
 * @param suffix 后缀
 * @return 1表示是，0表示否
 */
int str_ends_with(const char* str, const char* suffix);

/**
 * 将字符串转换为小写
 * @param str 要转换的字符串
 * @return 转换后的字符串
 */
char* str_to_lower(char* str);

/**
 * 将字符串转换为大写
 * @param str 要转换的字符串
 * @return 转换后的字符串
 */
char* str_to_upper(char* str);

#endif // STRING_UTILS_H
