#include "avif_to_jpg.hpp"
#include "common.hpp"
#include "image_loader.hpp"
#include <avif/avif.h>
#include <stb/stb_image_write.h>

bool AvifToJpgConverter::convert(const std::string& input_path,
                              const std::string& output_path) {
    // 加载AVIF图像
    ImageData data = ImageLoader::load(input_path);
    if (!data.pixels) {
        return false;
    }

    // 验证输入格式
    if (!validate(data)) {
        return false;
    }

    // 保存为JPG(默认质量90)
    return stbi_write_jpg(output_path.c_str(),
                        data.width,
                        data.height,
                        data.channels,
                        data.pixels.get(),
                        90);
}

bool AvifToJpgConverter::validate(const ImageData& data) {
    return data.width > 0 && data.height > 0 && 
          (data.channels == 3 || data.channels == 4);
}
