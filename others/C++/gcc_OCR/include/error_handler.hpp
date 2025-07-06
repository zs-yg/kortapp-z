#ifndef ERROR_HANDLER_HPP
#define ERROR_HANDLER_HPP

#include "../include/common.hpp"
#include <stdexcept>

// 错误代码枚举
enum class ErrorCode {
    FILE_IO_ERROR,
    OCR_INIT_ERROR,
    OCR_PROCESS_ERROR,
    GUI_ERROR,
    UNKNOWN_ERROR
};

// 自定义异常类
class OCRException : public std::runtime_error {
public:
    ErrorCode code;
    
    OCRException(ErrorCode ec, const String& msg)
        : std::runtime_error(msg), code(ec) {}
};

// 错误处理函数
void handleError(const std::exception& e);

#endif // ERROR_HANDLER_HPP
