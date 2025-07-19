#include "avif_to_webp.hpp"
#include "common.hpp"
#include "image_loader.hpp"
#include <avif/avif.h>
#include <webp/encode.h>

bool AvifToWebpConverter::convert(const std::string& input_path,
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

    // 保存为WEBP
    FILE* f = fopen(output_path.c_str(), "wb");
    if (!f) {
        return false;
    }

    uint8_t* output = nullptr;
    size_t size = 0;
    if (data.channels == 3) {
        size = WebPEncodeRGB(data.pixels.get(), data.width, data.height,
                            data.width * 3, 90, &output);
    } else if (data.channels == 4) {
        size = WebPEncodeRGBA(data.pixels.get(), data.width, data.height,
                            data.width * 4, 90, &output);
    }

    if (size == 0 || !output) {
        fclose(f);
        return false;
    }

    fwrite(output, 1, size, f);
    fclose(f);
    WebPFree(output);
    return true;
}

bool AvifToWebpConverter::validate(const ImageData& data) {
    return data.width > 0 && data.height > 0 && 
          (data.channels == 3 || data.channels == 4);
}
