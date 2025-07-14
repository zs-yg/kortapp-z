#include "tiff_to_jpeg.hpp"
#include "common.hpp"
#include <tiffio.h>
#include <stb/stb_image_write.h>
#include <string>

bool TiffToJpegConverter::convert(const std::string& input_path,
                               const std::string& output_path,
                               int quality) {
    // 使用libtiff加载TIFF图像
    TIFF* tif = TIFFOpen(input_path.c_str(), "r");
    if (!tif) {
        return false;
    }

    // 获取图像信息
    uint32 width, height;
    TIFFGetField(tif, TIFFTAG_IMAGEWIDTH, &width);
    TIFFGetField(tif, TIFFTAG_IMAGELENGTH, &height);
    
    // 获取TIFF格式信息
    uint16 samplesperpixel, bitspersample, photometric;
    TIFFGetField(tif, TIFFTAG_SAMPLESPERPIXEL, &samplesperpixel);
    TIFFGetField(tif, TIFFTAG_BITSPERSAMPLE, &bitspersample);
    TIFFGetField(tif, TIFFTAG_PHOTOMETRIC, &photometric);

    // 验证TIFF格式
    if (bitspersample != 8) {
        TIFFClose(tif);
        return false;
    }

    // 设置输出通道数
    ImageData data;
    data.width = width;
    data.height = height;
    data.channels = samplesperpixel;
    data.pixels.reset(new unsigned char[width * height * data.channels]);

    // 读取图像数据
    tdata_t buf = _TIFFmalloc(TIFFScanlineSize(tif));
    for (uint32 row = 0; row < height; row++) {
        if (TIFFReadScanline(tif, buf, row) == -1) {
            _TIFFfree(buf);
            TIFFClose(tif);
            return false;
        }
        memcpy(&data.pixels.get()[row * width * data.channels], 
              buf, 
              width * data.channels);
    }

    _TIFFfree(buf);
    TIFFClose(tif);

    // 验证输入
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

bool TiffToJpegConverter::validate_input(const ImageData& data) {
    return data.width > 0 && data.height > 0 && 
          (data.channels == 1 || data.channels == 3 || data.channels == 4);
}
