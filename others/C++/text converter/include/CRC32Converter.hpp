#ifndef CRC32CONVERTER_HPP
#define CRC32CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class CRC32Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "CRC32"; }
};

#endif // CRC32CONVERTER_HPP
