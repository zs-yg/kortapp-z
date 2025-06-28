#ifndef ERROR_H
#define ERROR_H

typedef enum {
    ERR_NONE = 0,
    ERR_FILE_NOT_FOUND,
    ERR_INVALID_ARGUMENT,
    ERR_MEMORY_ALLOC,
    ERR_FFMPEG,
    ERR_UNKNOWN
} ErrorCode;

/**
 * 设置当前错误代码
 * @param code 错误代码
 * @param message 错误信息(可选)
 */
void error_set(ErrorCode code, const char* message);

/**
 * 获取当前错误代码
 * @return 错误代码
 */
ErrorCode error_get_code();

/**
 * 获取当前错误信息
 * @return 错误信息字符串
 */
const char* error_get_message();

/**
 * 清除错误状态
 */
void error_clear();

#endif // ERROR_H
