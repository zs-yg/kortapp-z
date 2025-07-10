#include "image_loader.hpp"
#include "common.hpp"
#include <stb/stb_image.h>
#include <stb/stb_image_write.h>
#include <stdexcept>
#include <iostream>

ImageData ImageLoader::load(const std::string& path) {
    ImageData data;
    
    // 加载图像
    unsigned char* pixels = stbi_load(path.c_str(), 
                                     &data.width, 
                                     &data.height, 
                                     &data.channels, 
                                     0);
    if (!pixels) {
        throw std::runtime_error("无法加载图像: " + std::string(stbi_failure_reason()));
    }

    // 验证图像数据
    try {
        validate_image(pixels, data.width, data.height);
    } catch (...) {
        stbi_image_free(pixels);
        throw;
    }

    // 转移所有权到智能指针
    data.pixels.reset(pixels);
    return data;
}

bool ImageLoader::save_png(const std::string& path, const ImageData& data) {
    if (!data.pixels || data.width <= 0 || data.height <= 0) {
        return false;
    }
    return stbi_write_png(path.c_str(),
                         data.width,
                         data.height,
                         data.channels,
                         data.pixels.get(),
                         data.width * data.channels);
}

bool ImageLoader::save_jpg(const std::string& path, const ImageData& data, int quality) {
    if (!data.pixels || data.width <= 0 || data.height <= 0) {
        return false;
    }
    return stbi_write_jpg(path.c_str(),
                         data.width,
                         data.height,
                         data.channels,
                         data.pixels.get(),
                         quality);
}

void ImageLoader::validate_image(const unsigned char* data, int width, int height) {
    if (!data || width <= 0 || height <= 0) {
        throw std::runtime_error("无效的图像数据");
    }
}
