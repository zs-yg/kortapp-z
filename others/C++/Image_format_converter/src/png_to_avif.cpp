#include "png_to_avif.hpp"
#include "common.hpp"
#include "image_loader.hpp"
#include <avif/avif.h>
#include <stb/stb_image.h>

bool PngToAvifConverter::convert(const std::string& input_path,
                              const std::string& output_path) {
    // 加载PNG图像
    ImageData data = ImageLoader::load(input_path);
    if (!data.pixels) {
        return false;
    }

    // 验证输入格式
    if (!validate(data)) {
        return false;
    }

    // 创建AVIF编码器
    avifEncoder* encoder = avifEncoderCreate();
    encoder->maxThreads = 4;
    encoder->minQuantizer = 20;
    encoder->maxQuantizer = 20;
    encoder->speed = 6;

    // 创建AVIF图像
    avifImage* image = avifImageCreate(data.width, data.height, 8, AVIF_PIXEL_FORMAT_YUV420);
    avifRGBImage rgbImage;
    avifRGBImageSetDefaults(&rgbImage, image);
    rgbImage.pixels = data.pixels.get();
    rgbImage.rowBytes = data.width * data.channels;
    rgbImage.format = (data.channels == 3) ? AVIF_RGB_FORMAT_RGB : AVIF_RGB_FORMAT_RGBA;
    
    // 转换RGB到YUV
    if (avifImageRGBToYUV(image, &rgbImage) != AVIF_RESULT_OK) {
        avifEncoderDestroy(encoder);
        avifImageDestroy(image);
        return false;
    }

    // 编码AVIF图像
    avifRWData output = AVIF_DATA_EMPTY;
    if (avifEncoderWrite(encoder, image, &output) != AVIF_RESULT_OK) {
        avifEncoderDestroy(encoder);
        avifImageDestroy(image);
        return false;
    }

    // 保存AVIF文件
    FILE* f = fopen(output_path.c_str(), "wb");
    if (!f) {
        avifRWDataFree(&output);
        avifEncoderDestroy(encoder);
        avifImageDestroy(image);
        return false;
    }
    fwrite(output.data, 1, output.size, f);
    fclose(f);

    // 清理资源
    avifRWDataFree(&output);
    avifEncoderDestroy(encoder);
    avifImageDestroy(image);
    return true;
}

bool PngToAvifConverter::validate(const ImageData& data) {
    return data.width > 0 && data.height > 0 && 
          (data.channels == 3 || data.channels == 4);
}
