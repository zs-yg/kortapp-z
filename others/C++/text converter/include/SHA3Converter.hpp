#ifndef SHA3CONVERTER_HPP
#define SHA3CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class SHA3Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "SHA3"; }
};

#endif // SHA3CONVERTER_HPP
