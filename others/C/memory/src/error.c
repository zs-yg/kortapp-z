#include "../include/memory_trainer.h"
#include "../include/log.h"
#include <windows.h>

// 错误代码定义
const wchar_t* error_messages[] = {
    L"操作成功",
    L"内存分配失败",
    L"无效参数",
    L"系统错误"
};



// 获取错误描述
const wchar_t* get_error_message(ErrorCode code) {
    if (code < 0 || code >= sizeof(error_messages)/sizeof(error_messages[0])) {
        return L"未知错误";
    }
    return error_messages[code];
}
