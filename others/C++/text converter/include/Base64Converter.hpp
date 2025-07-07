#ifndef BASE64CONVERTER_HPP
#define BASE64CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class Base64Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "Base64"; }
};

#endif // BASE64CONVERTER_HPP
