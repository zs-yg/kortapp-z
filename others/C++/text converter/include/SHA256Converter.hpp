#ifndef SHA256CONVERTER_HPP
#define SHA256CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class SHA256Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "SHA256"; }
};

#endif // SHA256CONVERTER_HPP
