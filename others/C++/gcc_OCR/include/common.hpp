#ifndef COMMON_HPP
#define COMMON_HPP

#include <iostream>
#include <string>
#include <memory>
#include <stdexcept>

// 公共宏定义
#define APP_NAME "OCR识别器"
#define APP_VERSION "1.0.0"

// 公共类型定义
using String = std::string;

// 错误处理宏
#define THROW_EXCEPTION(msg) throw std::runtime_error(std::string(__FILE__) + ":" + std::to_string(__LINE__) + " " + msg)

#endif // COMMON_HPP
