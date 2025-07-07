#ifndef BINARYCONVERTER_HPP
#define BINARYCONVERTER_HPP

#include "Converter.hpp"
#include <string>
#include <bitset>

class BinaryConverter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "二进制"; }
};

#endif // BINARYCONVERTER_HPP
