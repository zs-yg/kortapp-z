#ifndef LOGGING_H
#define LOGGING_H

#include <windows.h>

typedef enum {
    LOG_DEBUG,
    LOG_INFO,
    LOG_WARNING,
    LOG_ERROR
} LogLevel;

void log_message(LogLevel level, const char* format, ...);

#endif // LOGGING_H
