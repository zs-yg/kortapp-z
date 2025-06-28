#include "logger.h"
#include <time.h>
#include <stdarg.h>
#include <stdlib.h>
#include <string.h>

static FILE* log_file = NULL;
static LogLevel current_level = LOG_INFO;

void logger_init(const char* log_file_path) {
    if (log_file_path != NULL) {
        log_file = fopen(log_file_path, "a");
    } else {
        log_file = stdout;
    }
}

void logger_set_level(LogLevel level) {
    current_level = level;
}

void logger_log(LogLevel level, const char* format, ...) {
    if (log_file == NULL || level < current_level) {
        return;
    }

    // 获取当前时间
    time_t now;
    time(&now);
    char time_str[20];
    strftime(time_str, sizeof(time_str), "%Y-%m-%d %H:%M:%S", localtime(&now));

    // 日志级别字符串
    const char* level_str;
    switch (level) {
        case LOG_DEBUG: level_str = "DEBUG"; break;
        case LOG_INFO: level_str = "INFO"; break;
        case LOG_WARNING: level_str = "WARNING"; break;
        case LOG_ERROR: level_str = "ERROR"; break;
        default: level_str = "UNKNOWN"; break;
    }

    // 写入时间戳和日志级别
    fprintf(log_file, "[%s] [%s] ", time_str, level_str);

    // 写入日志内容
    va_list args;
    va_start(args, format);
    vfprintf(log_file, format, args);
    va_end(args);

    fprintf(log_file, "\n");
    fflush(log_file);
}

void logger_close() {
    if (log_file != NULL && log_file != stdout) {
        fclose(log_file);
    }
    log_file = NULL;
}
