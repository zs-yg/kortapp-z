#ifndef SHA512CONVERTER_HPP
#define SHA512CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class SHA512Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "SHA512"; }
};

#endif // SHA512CONVERTER_HPP
