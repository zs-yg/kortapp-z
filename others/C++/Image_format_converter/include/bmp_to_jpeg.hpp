#pragma once
#include <string>
#include "common.hpp"

class BmpToJpegConverter {
public:
    static bool convert(const std::string& input_path,
                      const std::string& output_path,
                      int quality = 90);
    
private:
    static bool validate_input(const ImageData& data);
};
