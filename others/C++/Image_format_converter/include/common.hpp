#pragma once
#include <vector>
#include <string>
#include <memory>
#include <stb/stb_image.h>

struct ImageData {
    int width;
    int height;
    int channels;
    std::unique_ptr<unsigned char, void(*)(void*)> pixels;
    
    ImageData() : pixels(nullptr, stbi_image_free) {}
};

enum class ImageFormat {
    PNG,
    JPG,
    TIFF,
    WEBP,
    AVIF,
    UNKNOWN
};

ImageFormat get_format_from_extension(const std::string& path);
