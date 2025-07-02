#include "logging.h"
#include <stdio.h>
#include <stdarg.h>
#include <time.h>

static const char* level_strings[] = {
    "DEBUG",
    "INFO",
    "WARNING",
    "ERROR"
};

void log_message(LogLevel level, const char* format, ...) {
    va_list args;
    va_start(args, format);

    // 获取当前时间
    time_t now;
    time(&now);
    struct tm* timeinfo = localtime(&now);
    char time_str[20];
    strftime(time_str, sizeof(time_str), "%Y-%m-%d %H:%M:%S", timeinfo);

    // 格式化日志消息
    char message[1024];
    vsnprintf(message, sizeof(message), format, args);

    // 输出到调试控制台
    char output[2048];
    snprintf(output, sizeof(output), "[%s] [%s] %s\n", time_str, level_strings[level], message);
    OutputDebugStringA(output);

    // 输出到文件
    FILE* log_file = fopen("system_info.log", "a");
    if (log_file) {
        fprintf(log_file, "%s", output);
        fclose(log_file);
    }

    va_end(args);
}
