#include "../include/Base32Converter.hpp"
#include <string>
#include <stdexcept>

const std::string BASE32_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

std::string Base32Converter::convert(const std::string& input) {
    std::string result;
    int buffer = 0;
    int bitsLeft = 0;
    int count = 0;

    for (unsigned char c : input) {
        buffer <<= 8;
        buffer |= c;
        bitsLeft += 8;
        count++;

        while (bitsLeft >= 5) {
            int index = (buffer >> (bitsLeft - 5)) & 0x1F;
            result += BASE32_CHARS[index];
            bitsLeft -= 5;
        }
    }

    if (bitsLeft > 0) {
        int index = (buffer << (5 - bitsLeft)) & 0x1F;
        result += BASE32_CHARS[index];
    }

    // 添加填充字符
    while (result.size() % 8 != 0) {
        result += '=';
    }

    return result;
}
