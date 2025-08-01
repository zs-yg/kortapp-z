#include "png_to_jpeg.hpp"
#include "common.hpp"
#include "image_loader.hpp"
#include <stb/stb_image.h>
#include <stb/stb_image_write.h>
#include <string>

bool PngToJpegConverter::convert(const std::string& input_path, 
                              const std::string& output_path,
                              int quality) {
    // 加载PNG图像
    ImageData data = ImageLoader::load(input_path);
    if (!data.pixels) {
        return false;
    }

    // 验证输入格式
    if (!validate_input(data)) {
        return false;
    }

    // 保存为JPEG
    return stbi_write_jpg(output_path.c_str(), 
                         data.width, 
                         data.height, 
                         data.channels, 
                         data.pixels.get(), 
                         quality);
}

bool PngToJpegConverter::validate_input(const ImageData& data) {
    // 确保是有效的图像数据
    return data.width > 0 && data.height > 0 && 
          (data.channels == 3 || data.channels == 4);
}
