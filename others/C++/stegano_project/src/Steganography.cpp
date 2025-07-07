#include "Steganography.hpp"
#include "CommonDefs.hpp"
#include <stdexcept>

std::wstring Steganography::embed(const std::wstring& cover_text, const std::string& message) {
    std::wstring encoded = ZeroWidthEncoder::encode(message);
    return cover_text + CommonDefs::ZERO_WIDTH_JOINER + encoded;
}

std::string Steganography::extract(const std::wstring& stego_text) {
    // 查找分隔符
    size_t pos = stego_text.find(CommonDefs::ZERO_WIDTH_JOINER);
    if (pos == std::wstring::npos) {
        throw std::runtime_error("No hidden message detected");
    }
    
    std::wstring encoded = stego_text.substr(pos + 1);
    return ZeroWidthEncoder::decode(encoded);
}

bool Steganography::is_zero_width(wchar_t c) {
    return c == CommonDefs::ZERO_WIDTH_SPACE || 
           c == CommonDefs::ZERO_WIDTH_NON_JOINER || 
           c == CommonDefs::ZERO_WIDTH_JOINER;
}