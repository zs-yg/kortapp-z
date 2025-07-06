#include "../include/error_handler.hpp"
#include <iostream>
#include <FL/Fl.H>
#include <FL/fl_ask.H>

void handleError(const std::exception& e) {
    // 记录错误到控制台
    std::cerr << "错误: " << e.what() << std::endl;
    
    // 显示用户友好的错误消息
    if (Fl::first_window()) {
        fl_alert("%s", e.what());
    } else {
        std::cerr << "无法显示错误对话框: 没有可用的GUI窗口" << std::endl;
    }
}

String errorCodeToString(ErrorCode code) {
    switch(code) {
        case ErrorCode::FILE_IO_ERROR: return "文件IO错误";
        case ErrorCode::OCR_INIT_ERROR: return "OCR初始化错误";
        case ErrorCode::OCR_PROCESS_ERROR: return "OCR处理错误";
        case ErrorCode::GUI_ERROR: return "GUI错误";
        case ErrorCode::UNKNOWN_ERROR: return "未知错误";
        default: return "未定义的错误代码";
    }
}
