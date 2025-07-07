#ifndef SHA1CONVERTER_HPP
#define SHA1CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class SHA1Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "SHA1"; }
};

#endif // SHA1CONVERTER_HPP
