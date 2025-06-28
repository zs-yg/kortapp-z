#ifndef UTILS_H
#define UTILS_H

#include <stdbool.h>

/**
 * 获取当前时间戳(毫秒)
 * @return 时间戳
 */
long long get_timestamp();

/**
 * 生成随机字符串
 * @param buffer 输出缓冲区
 * @param length 字符串长度
 */
void generate_random_string(char* buffer, int length);

/**
 * 检查指针是否有效
 * @param ptr 要检查的指针
 * @return true表示有效，false表示无效
 */
bool is_pointer_valid(const void* ptr);

#endif // UTILS_H
