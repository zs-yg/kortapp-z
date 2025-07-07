#ifndef STEGANOGRAPHY_HPP
#define STEGANOGRAPHY_HPP

#include "ZeroWidthEncoder.hpp"
#include <string>

class Steganography {
public:
    static std::wstring embed(const std::wstring& cover_text, const std::string& message);
    static std::string extract(const std::wstring& stego_text);
    
private:
    static bool is_zero_width(wchar_t c);
};

#endif // STEGANOGRAPHY_HPP