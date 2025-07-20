#include "../include/BinaryConverter.hpp"
#include <bitset>
#include <sstream>
#include <algorithm>

std::string BinaryConverter::convert(const std::string& input) {
    if (input.empty()) {
        return "";
    }

    std::stringstream result;
    for (char c : input) {
        std::string binary = std::bitset<8>(c).to_string();
        // 去除前导0,保留后6位
        size_t firstOne = binary.find('1');
        if (firstOne != std::string::npos) {
            binary = binary.substr(firstOne);
        }
        result << binary << " ";
    }

    std::string output = result.str();
    if (!output.empty()) {
        output.pop_back(); // 移除最后一个空格
    }
    return output;
}
