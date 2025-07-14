#pragma once
#include <string>
#include "common.hpp"

class WebpToTiffConverter {
public:
    static bool convert(const std::string& input_path,
                      const std::string& output_path);
    
private:
    static bool validate_input(const ImageData& data);
};
