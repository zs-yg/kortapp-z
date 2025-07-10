#pragma once
#include <string>
#include "common.hpp"

class ConverterBase {
public:
    virtual ~ConverterBase() = default;
    virtual bool convert(const std::string& input, 
                       const std::string& output) = 0;
    
protected:
    virtual bool validate(const ImageData& data) = 0;
};
