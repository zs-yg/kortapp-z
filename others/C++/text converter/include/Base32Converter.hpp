#ifndef BASE32CONVERTER_HPP
#define BASE32CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class Base32Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "Base32"; }
};

#endif // BASE32CONVERTER_HPP
