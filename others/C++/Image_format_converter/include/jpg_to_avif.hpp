#pragma once
#include "converter_base.hpp"
#include "common.hpp"

class JpgToAvifConverter : public ConverterBase {
public:
    bool convert(const std::string& input, const std::string& output) override;
    
protected:
    bool validate(const ImageData& data) override;
};
