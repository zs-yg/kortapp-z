#ifndef HEXCONVERTER_HPP
#define HEXCONVERTER_HPP

#include "Converter.hpp"
#include <string>
#include <iomanip>
#include <sstream>

class HexConverter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "十六进制"; }
};

#endif // HEXCONVERTER_HPP
