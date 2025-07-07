#include "../include/HexConverter.hpp"
#include <iomanip>
#include <sstream>

std::string HexConverter::convert(const std::string& input) {
    if (input.empty()) {
        return "";
    }

    std::stringstream result;
    for (char c : input) {
        result << std::hex << std::setw(2) << std::setfill('0') 
               << static_cast<int>(static_cast<unsigned char>(c)) << " ";
    }

    std::string output = result.str();
    // 移除最后一个空格
    if (!output.empty()) {
        output.pop_back();
    }

    return output;
}
