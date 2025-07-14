#include "jpeg_to_tiff.hpp"
#include "common.hpp"
#include "image_loader.hpp"
#include <tiffio.h>
#include <stb/stb_image.h>
#include <string>

bool JpegToTiffConverter::convert(const std::string& input_path,
                               const std::string& output_path) {
    // 加载JPEG图像
    ImageData data = ImageLoader::load(input_path);
    if (!data.pixels) {
        return false;
    }

    // 验证输入
    if (!validate_input(data)) {
        return false;
    }

    // 创建TIFF文件
    TIFF* tif = TIFFOpen(output_path.c_str(), "w");
    if (!tif) {
        return false;
    }

    // 设置TIFF标签
    TIFFSetField(tif, TIFFTAG_IMAGEWIDTH, data.width);
    TIFFSetField(tif, TIFFTAG_IMAGELENGTH, data.height);
    TIFFSetField(tif, TIFFTAG_SAMPLESPERPIXEL, data.channels);
    TIFFSetField(tif, TIFFTAG_BITSPERSAMPLE, 8);
    TIFFSetField(tif, TIFFTAG_ORIENTATION, ORIENTATION_TOPLEFT);
    TIFFSetField(tif, TIFFTAG_PLANARCONFIG, PLANARCONFIG_CONTIG);
    
    // 根据通道数设置PhotometricInterpretation
    if (data.channels == 1) {
        TIFFSetField(tif, TIFFTAG_PHOTOMETRIC, PHOTOMETRIC_MINISBLACK);
    } else if (data.channels == 3 || data.channels == 4) {
        TIFFSetField(tif, TIFFTAG_PHOTOMETRIC, PHOTOMETRIC_RGB);
    } else {
        TIFFClose(tif);
        return false;
    }

    // 写入图像数据
    tsize_t linebytes = data.width * data.channels;
    unsigned char* buf = (unsigned char*)_TIFFmalloc(linebytes);
    for (int y = 0; y < data.height; y++) {
        memcpy(buf, &data.pixels.get()[y * linebytes], linebytes);
        TIFFWriteScanline(tif, buf, y, 0);
    }

    _TIFFfree(buf);
    TIFFClose(tif);
    return true;
}

bool JpegToTiffConverter::validate_input(const ImageData& data) {
    return data.width > 0 && data.height > 0 && 
          (data.channels == 1 || data.channels == 3 || data.channels == 4);
}
