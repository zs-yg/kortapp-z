#include "../include/log.h"
#include "../include/memory_trainer.h"
#include "../include/error.h"
#include <windows.h>
#include <stdio.h>
#include <time.h>

static FILE* log_file = NULL;
static LogLevel current_level = LOG_INFO;

// 初始化日志系统
void init_logger() {
    log_file = _wfopen(L"memory_trainer.log", L"a, ccs=UTF-8");
    if (!log_file) {
        log_file = stdout;
    }
}

// 记录日志(支持中文)
void log_message(LogLevel level, const wchar_t* format, ...) {
    if (level < current_level) return;

    // 获取当前时间
    time_t now;
    time(&now);
    struct tm* tm_info = localtime(&now);
    wchar_t time_buf[20];
    wcsftime(time_buf, 20, L"%Y-%m-%d %H:%M:%S", tm_info);

    // 格式化日志消息
    va_list args;
    va_start(args, format);
    fwprintf(log_file, L"[%s] ", time_buf);
    vfwprintf(log_file, format, args);
    fwprintf(log_file, L"\n");
    va_end(args);

    fflush(log_file);
}

// 报告错误并显示给用户
void report_error(ErrorCode code, const wchar_t* message) {
    // 获取标准错误描述
    const wchar_t* error_desc = get_error_message(code);
    
    // 构建完整错误消息
    wchar_t full_msg[512];
    wsprintfW(full_msg, L"%s: %s", error_desc, message);
    
    // 记录到日志
    log_message(LOG_ERROR, L"错误代码 %d: %s", code, full_msg);
    
    // 显示给用户
    if (g_hMainWnd) {
        MessageBoxW(g_hMainWnd, full_msg, L"内存锻炼器 - 错误", MB_ICONERROR);
    } else {
        OutputDebugStringW(full_msg);
    }
}
