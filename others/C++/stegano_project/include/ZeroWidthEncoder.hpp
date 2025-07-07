#ifndef ZERO_WIDTH_ENCODER_HPP
#define ZERO_WIDTH_ENCODER_HPP

#include "CommonDefs.hpp"
#include <string>

class ZeroWidthEncoder {
public:
    static std::wstring encode(const std::string& message);
    static std::string decode(const std::wstring& encoded);
    
private:
    static std::wstring char_to_zero_width(char c);
    static char zero_width_to_char(const std::wstring& sequence);
};

#endif // ZERO_WIDTH_ENCODER_HPP