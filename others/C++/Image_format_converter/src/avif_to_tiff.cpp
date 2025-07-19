#include "avif_to_tiff.hpp"
#include "common.hpp"
#include "image_loader.hpp"
#include <avif/avif.h>
#include <tiffio.h>

bool AvifToTiffConverter::convert(const std::string& input_path,
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
    TIFFSetField(tif, TIFFTAG_PHOTOMETRIC, 
                (data.channels == 1) ? PHOTOMETRIC_MINISBLACK : 
                (data.channels == 3) ? PHOTOMETRIC_RGB : PHOTOMETRIC_RGB);
    TIFFSetField(tif, TIFFTAG_COMPRESSION, COMPRESSION_LZW);

    // 写入图像数据
    tsize_t linebytes = data.width * data.channels;
    unsigned char* buf = nullptr;
    if (TIFFScanlineSize(tif) == linebytes) {
        buf = (unsigned char*)_TIFFmalloc(linebytes);
    } else {
        buf = (unsigned char*)_TIFFmalloc(TIFFScanlineSize(tif));
    }

    for (uint32_t row = 0; row < data.height; row++) {
        memcpy(buf, &data.pixels.get()[row * linebytes], linebytes);
        if (TIFFWriteScanline(tif, buf, row, 0) < 0) {
            _TIFFfree(buf);
            TIFFClose(tif);
            return false;
        }
    }

    // 清理资源
    _TIFFfree(buf);
    TIFFClose(tif);
    return true;
}

bool AvifToTiffConverter::validate(const ImageData& data) {
    return data.width > 0 && data.height > 0 && 
          (data.channels == 1 || data.channels == 3 || data.channels == 4);
}
