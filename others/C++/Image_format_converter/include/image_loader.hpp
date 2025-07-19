#pragma once
#include "common.hpp"
#include <string>

class ImageLoader {
public:
    static ImageData load(const std::string& path);
    static bool save_png(const std::string& path, const ImageData& data);
    static bool save_jpg(const std::string& path, const ImageData& data, int quality = 90);
    static void validate_image(const unsigned char* data, int width, int height);
    
private:
};
