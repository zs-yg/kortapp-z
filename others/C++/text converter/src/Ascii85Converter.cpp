#include "../include/Ascii85Converter.hpp"
#include <string>
#include <cmath>
#include <sstream>
#include <cstdint>

const std::string ASCII85_PREFIX = "<~";
const std::string ASCII85_SUFFIX = "~>";

std::string Ascii85Converter::convert(const std::string& input) {
    std::stringstream result;
    result << ASCII85_PREFIX;

    size_t i = 0;
    while (i < input.size()) {
        uint32_t value = 0;
        int bytes = 0;

        // 读取4个字节
        for (int j = 0; j < 4 && (i + j) < input.size(); j++) {
            value = (value << 8) | static_cast<unsigned char>(input[i + j]);
            bytes++;
        }
        i += bytes;

        // 特殊处理全0的4字节
        if (value == 0 && bytes == 4) {
            result << 'z';
            continue;
        }

        // 转换为Ascii85
        char chars[5];
        for (int j = 4; j >= 0; j--) {
            chars[j] = value % 85 + '!';
            value /= 85;
        }

        // 写入结果(只写入bytes+1个字符)
        for (int j = 0; j < bytes + 1; j++) {
            result << chars[j];
        }
    }

    result << ASCII85_SUFFIX;
    return result.str();
}
