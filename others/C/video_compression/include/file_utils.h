#ifndef FILE_UTILS_H
#define FILE_UTILS_H

#include <stdbool.h>

/**
 * 检查文件是否存在
 * @param path 文件路径
 * @return true表示存在，false表示不存在
 */
bool file_exists(const char* path);

/**
 * 获取文件大小
 * @param path 文件路径
 * @return 文件大小(字节)，-1表示错误
 */
long file_size(const char* path);

/**
 * 获取文件扩展名
 * @param path 文件路径
 * @return 扩展名字符串(包含.)，NULL表示没有扩展名
 */
const char* file_extension(const char* path);

#endif // FILE_UTILS_H
