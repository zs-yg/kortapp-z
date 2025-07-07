#ifndef COMMON_DEFS_HPP
#define COMMON_DEFS_HPP

#include <vector>
#include <string>
#include <cstdint>

namespace CommonDefs {
    // 零宽字符定义
    constexpr wchar_t ZERO_WIDTH_SPACE = 0x200B;      // 用于表示二进制0
    constexpr wchar_t ZERO_WIDTH_NON_JOINER = 0x200C; // 用于表示二进制1
    constexpr wchar_t ZERO_WIDTH_JOINER = 0x200D;     // 用于分隔符

    // 转换函数声明
    std::wstring utf8_to_wstring(const std::string& str);
    std::string wstring_to_utf8(const std::wstring& wstr);
}

#endif // COMMON_DEFS_HPP