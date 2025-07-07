#include "../include/CRC32Converter.hpp"
#include <string>
#include <sstream>
#include <iomanip>
#include <cstdint>

// CRC32多项式
const uint32_t CRC32_POLY = 0xEDB88320;

uint32_t computeCRC32(const std::string& input) {
    uint32_t crc = 0xFFFFFFFF;
    
    for (char c : input) {
        crc ^= static_cast<unsigned char>(c);
        for (int i = 0; i < 8; i++) {
            crc = (crc >> 1) ^ ((crc & 1) ? CRC32_POLY : 0);
        }
    }
    
    return ~crc;
}

std::string CRC32Converter::convert(const std::string& input) {
    uint32_t crc = computeCRC32(input);
    std::stringstream ss;
    ss << std::hex << std::setw(8) << std::setfill('0') << crc;
    return ss.str();
}
