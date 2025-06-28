#ifndef LOGGER_H
#define LOGGER_H

#include <stdio.h>

typedef enum {
    LOG_DEBUG,
    LOG_INFO,
    LOG_WARNING,
    LOG_ERROR
} LogLevel;

/**
 * 初始化日志系统
 * @param log_file 日志文件路径，NULL表示输出到stdout
 */
void logger_init(const char* log_file);

/**
 * 设置日志级别
 * @param level 日志级别
 */
void logger_set_level(LogLevel level);

/**
 * 记录日志
 * @param level 日志级别
 * @param format 格式化字符串
 * @param ... 可变参数
 */
void logger_log(LogLevel level, const char* format, ...);

/**
 * 关闭日志系统
 */
void logger_close();

#endif // LOGGER_H
