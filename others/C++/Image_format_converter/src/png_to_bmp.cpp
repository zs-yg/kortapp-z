#include "png_to_bmp.hpp"
#include "common.hpp"
#include "image_loader.hpp"
#include <stb/stb_image.h>
#include <stb/stb_image_write.h>
#include <string>

bool PngToBmpConverter::convert(const std::string& input_path,
                             const std::string& output_path) {
    // 加载PNG图像
    ImageData data = ImageLoader::load(input_path);
    if (!data.pixels) {
        return false;
    }

    // 验证输入格式
    if (!validate_input(data)) {
        return false;
    }

    // 保存为BMP
    return stbi_write_bmp(output_path.c_str(),
                         data.width,
                         data.height,
                         data.channels,
                         data.pixels.get());
}

bool PngToBmpConverter::validate_input(const ImageData& data) {
    // 确保是有效的图像数据
    return data.width > 0 && data.height > 0 && 
          (data.channels == 1 || data.channels == 3 || data.channels == 4);
}
