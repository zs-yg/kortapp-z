#ifndef MD5CONVERTER_HPP
#define MD5CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class MD5Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "MD5"; }
};

#endif // MD5CONVERTER_HPP
