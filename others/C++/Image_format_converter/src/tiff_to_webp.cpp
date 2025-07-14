#include "tiff_to_webp.hpp"
#include "common.hpp"
#include "image_loader.hpp"
#include <webp/encode.h>
#include <string>

bool TiffToWebpConverter::convert(const std::string& input_path,
                               const std::string& output_path,
                               int quality) {
    // 加载TIFF图像
    ImageData data = ImageLoader::load(input_path);
    if (!data.pixels) {
        return false;
    }

    // 验证输入格式
    if (!validate_input(data)) {
        return false;
    }

    // 编码为WebP
    uint8_t* output = nullptr;
    size_t output_size;
    if (data.channels == 3) {
        output_size = WebPEncodeRGB(data.pixels.get(),
                                  data.width,
                                  data.height,
                                  data.width * data.channels,
                                  quality,
                                  &output);
    } else {
        output_size = WebPEncodeRGBA(data.pixels.get(),
                                   data.width,
                                   data.height,
                                   data.width * data.channels,
                                   quality,
                                   &output);
    }

    if (output_size == 0) {
        return false;
    }

    // 保存WebP文件
    FILE* file = fopen(output_path.c_str(), "wb");
    if (!file) {
        WebPFree(output);
        return false;
    }

    fwrite(output, 1, output_size, file);
    fclose(file);
    WebPFree(output);
    return true;
}

bool TiffToWebpConverter::validate_input(const ImageData& data) {
    // 确保是有效的图像数据
    return data.width > 0 && data.height > 0 && 
          (data.channels == 3 || data.channels == 4);
}
