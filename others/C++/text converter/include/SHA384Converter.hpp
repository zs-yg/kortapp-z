#ifndef SHA384CONVERTER_HPP
#define SHA384CONVERTER_HPP

#include "Converter.hpp"
#include <string>

class SHA384Converter : public Converter {
public:
    std::string convert(const std::string& input) override;
    std::string getName() const override { return "SHA384"; }
};

#endif // SHA384CONVERTER_HPP
