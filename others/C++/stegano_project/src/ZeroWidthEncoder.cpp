#include "ZeroWidthEncoder.hpp"
#include "CommonDefs.hpp"
#include <bitset>

std::wstring ZeroWidthEncoder::encode(const std::string& message) {
    std::wstring result;
    for (char c : message) {
        result += char_to_zero_width(c);
    }
    return result;
}

std::string ZeroWidthEncoder::decode(const std::wstring& encoded) {
    std::string result;
    for (size_t i = 0; i < encoded.size(); i += 8) {
        if (i + 8 > encoded.size()) break;
        result += zero_width_to_char(encoded.substr(i, 8));
    }
    return result;
}

std::wstring ZeroWidthEncoder::char_to_zero_width(char c) {
    std::bitset<8> bits(c);
    std::wstring seq;
    for (int i = 7; i >= 0; --i) {
        seq += bits.test(i) ? 
            CommonDefs::ZERO_WIDTH_NON_JOINER : 
            CommonDefs::ZERO_WIDTH_SPACE;
    }
    return seq;
}

char ZeroWidthEncoder::zero_width_to_char(const std::wstring& sequence) {
    if (sequence.size() != 8) return 0;
    
    std::bitset<8> bits;
    for (int i = 0; i < 8; ++i) {
        bits.set(7 - i, sequence[i] == CommonDefs::ZERO_WIDTH_NON_JOINER);
    }
    return static_cast<char>(bits.to_ulong());
}