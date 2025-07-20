#include "image_loader.hpp"
#include "common.hpp"
#include <stb/stb_image.h>
#include <stb/stb_image_write.h>
#include <webp/decode.h>
#include <avif/avif.h>
#include <fstream>
#include <stdexcept>
#include <iostream>
#include <cstring>



static bool is_avif_file(const std::string& path) {
    std::ifstream file(path, std::ios::binary);
    if (!file) return false;
    
    char header[12];
    if (!file.read(header, 12)) return false;
    
    return !memcmp(header, "\0\0\0 ftypavif", 12) || 
           !memcmp(header, "\0\0\0 ftypavis", 12);
}

static bool is_webp_file(const std::string& path) {
    std::ifstream file(path, std::ios::binary);
    if (!file) return false;
    
    char header[12];
    if (!file.read(header, 12)) return false;
    
    return !memcmp(header, "RIFF", 4) && !memcmp(header + 8, "WEBP", 4);
}

static ImageData load_avif(const std::string& path) {
    ImageData data;
    
    avifDecoder* decoder = avifDecoderCreate();
    if (!decoder) {
        throw std::runtime_error("无法创建AVIF解码器");
    }
    
    avifResult result = avifDecoderSetIOFile(decoder, path.c_str());
    if (result != AVIF_RESULT_OK) {
        avifDecoderDestroy(decoder);
        throw std::runtime_error("无法读取AVIF文件");
    }
    
    result = avifDecoderParse(decoder);
    if (result != AVIF_RESULT_OK) {
        avifDecoderDestroy(decoder);
        throw std::runtime_error("无效的AVIF图像");
    }
    
    result = avifDecoderNextImage(decoder);
    if (result != AVIF_RESULT_OK) {
        avifDecoderDestroy(decoder);
        throw std::runtime_error("无法解码AVIF图像");
    }
    
    data.width = decoder->image->width;
    data.height = decoder->image->height;
    data.channels = 4; // AVIF解码为RGBA
    
    // 分配内存并转换图像数据
    uint8_t* rgba_pixels = new uint8_t[data.width * data.height * 4];
    avifRGBImage rgb;
    avifRGBImageSetDefaults(&rgb, decoder->image);
    rgb.format = AVIF_RGB_FORMAT_RGBA;
    rgb.depth = 8;
    rgb.pixels = rgba_pixels;
    rgb.rowBytes = data.width * 4;
    
    if (avifImageYUVToRGB(decoder->image, &rgb) != AVIF_RESULT_OK) {
        delete[] rgba_pixels;
        avifDecoderDestroy(decoder);
        throw std::runtime_error("AVIF颜色空间转换失败");
    }
    
    avifDecoderDestroy(decoder);
    
    // 验证图像数据
    try {
        ImageLoader::validate_image(rgba_pixels, data.width, data.height);
    } catch (...) {
        delete[] rgba_pixels;
        throw;
    }
    
    data.pixels = std::unique_ptr<unsigned char, void(*)(void*)>(rgba_pixels, [](void* p) { delete[] static_cast<uint8_t*>(p); });
    return data;
}

ImageData ImageLoader::load(const std::string& path) {
    ImageData data;
    
    // 检查是否为AVIF格式
    if (is_avif_file(path)) {
        return load_avif(path);
    }
    // 检查是否为WebP格式
    else if (is_webp_file(path)) {
        // 读取WebP文件数据
        std::ifstream file(path, std::ios::binary | std::ios::ate);
        if (!file) {
            throw std::runtime_error("无法打开WebP文件");
        }
        
        size_t size = file.tellg();
        file.seekg(0, std::ios::beg);
        
        std::vector<uint8_t> webp_data(size);
        if (!file.read(reinterpret_cast<char*>(webp_data.data()), size)) {
            throw std::runtime_error("无法读取WebP文件");
        }
        
        // 解码WebP图像
        WebPBitstreamFeatures features;
        if (WebPGetFeatures(webp_data.data(), webp_data.size(), &features) != VP8_STATUS_OK) {
            throw std::runtime_error("无效的WebP图像");
        }
        
        data.width = features.width;
        data.height = features.height;
        data.channels = features.has_alpha ? 4 : 3;
        
        // 解码WebP图像为RGBA格式
        uint8_t* rgba_pixels = WebPDecodeRGBA(webp_data.data(), webp_data.size(), &data.width, &data.height);
        if (!rgba_pixels) {
            throw std::runtime_error("无法解码WebP图像");
        }
        
        // 如果没有alpha通道,转换为RGB格式
        if (data.channels == 3) {
            uint8_t* rgb_pixels = new uint8_t[data.width * data.height * 3];
            for (int i = 0; i < data.width * data.height; ++i) {
                rgb_pixels[i*3] = rgba_pixels[i*4];
                rgb_pixels[i*3+1] = rgba_pixels[i*4+1];
                rgb_pixels[i*3+2] = rgba_pixels[i*4+2];
            }
            WebPFree(rgba_pixels);
            
            // 验证图像数据
            try {
        ImageLoader::validate_image(rgb_pixels, data.width, data.height);
            } catch (...) {
                delete[] rgb_pixels;
                throw;
            }
            
            data.pixels = std::unique_ptr<unsigned char, void(*)(void*)>(rgb_pixels, [](void* p) { delete[] static_cast<uint8_t*>(p); });
        } else {
            // 验证图像数据
            try {
        ImageLoader::validate_image(rgba_pixels, data.width, data.height);
            } catch (...) {
                WebPFree(rgba_pixels);
                throw;
            }
            
            data.pixels = std::unique_ptr<unsigned char, void(*)(void*)>(rgba_pixels, WebPFree);
        }
    } else {
        // 使用STB加载其他格式图像
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
        ImageLoader::validate_image(pixels, data.width, data.height);
        } catch (...) {
            stbi_image_free(pixels);
            throw;
        }
        
        data.pixels = std::unique_ptr<unsigned char, void(*)(void*)>(pixels, stbi_image_free);
    }
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
