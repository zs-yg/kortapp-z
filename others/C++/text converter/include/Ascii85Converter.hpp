#ifndef ASCII85CONVERTER_HPP
#define ASCII85CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class Ascii85Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "Ascii85"; }
};

#endif // ASCII85CONVERTER_HPP
