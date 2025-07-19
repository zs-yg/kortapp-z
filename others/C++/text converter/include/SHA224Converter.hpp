#ifndef SHA224CONVERTER_HPP
#define SHA224CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class SHA224Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "SHA224"; }
};

#endif // SHA224CONVERTER_HPP
