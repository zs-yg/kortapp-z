#pragma once
#include <string>
#include "common.hpp"

class BmpToPngConverter {
public:
    static bool convert(const std::string& input_path,
                      const std::string& output_path,
                      int compression_level = 6);
    
private:
    static bool validate_input(const ImageData& data);
};
