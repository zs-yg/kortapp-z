#ifndef ROT13CONVERTER_HPP
#define ROT13CONVERTER_HPP

#include "Converter.hpp"
#include <string>
#include <cctype>

class ROT13Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "ROT13"; }
};

#endif // ROT13CONVERTER_HPP
