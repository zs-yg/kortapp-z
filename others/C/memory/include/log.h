#ifndef LOG_H
#define LOG_H

#include <stdarg.h>
#include <stddef.h>

// 日志级别
typedef enum {
    LOG_DEBUG,
    LOG_INFO,
    LOG_WARNING,
    LOG_ERROR
} LogLevel;

// 初始化日志系统
void init_logger();

// 记录日志(支持中文)
void log_message(LogLevel level, const wchar_t* format, ...);

#endif // LOG_H
