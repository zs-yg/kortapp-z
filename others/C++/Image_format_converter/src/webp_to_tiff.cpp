#include "webp_to_tiff.hpp"
#include "common.hpp"
#include "image_loader.hpp"
#include <webp/decode.h>
#include <tiffio.h>
#include <string>

bool WebpToTiffConverter::convert(const std::string& input_path,
                               const std::string& output_path) {
    // 加载WebP图像
    ImageData data = ImageLoader::load(input_path);
    if (!data.pixels) {
        return false;
    }

    // 验证输入格式
    if (!validate_input(data)) {
        return false;
    }

    // 创建TIFF文件
    TIFF* tif = TIFFOpen(output_path.c_str(), "w");
    if (!tif) {
        return false;
    }

    // 设置TIFF文件参数
    TIFFSetField(tif, TIFFTAG_IMAGEWIDTH, data.width);
    TIFFSetField(tif, TIFFTAG_IMAGELENGTH, data.height);
    TIFFSetField(tif, TIFFTAG_SAMPLESPERPIXEL, data.channels);
    TIFFSetField(tif, TIFFTAG_BITSPERSAMPLE, 8);
    TIFFSetField(tif, TIFFTAG_ORIENTATION, ORIENTATION_TOPLEFT);
    TIFFSetField(tif, TIFFTAG_PLANARCONFIG, PLANARCONFIG_CONTIG);
    TIFFSetField(tif, TIFFTAG_PHOTOMETRIC, 
                data.channels == 3 ? PHOTOMETRIC_RGB : PHOTOMETRIC_MINISBLACK);
    TIFFSetField(tif, TIFFTAG_COMPRESSION, COMPRESSION_LZW);

    // 写入图像数据
    tsize_t linebytes = data.width * data.channels;
    unsigned char* buf = nullptr;
    if (TIFFScanlineSize(tif) == linebytes) {
        buf = (unsigned char*)data.pixels.get() + 
             (data.height - 1) * linebytes;
        for (uint32 row = 0; row < data.height; row++) {
            if (TIFFWriteScanline(tif, buf, row, 0) < 0) {
                TIFFClose(tif);
                return false;
            }
            buf -= linebytes;
        }
    } else {
        buf = (unsigned char*)_TIFFmalloc(linebytes);
        if (!buf) {
            TIFFClose(tif);
            return false;
        }
        unsigned char* src = (unsigned char*)data.pixels.get();
        for (uint32 row = 0; row < data.height; row++) {
            memcpy(buf, src, linebytes);
            if (TIFFWriteScanline(tif, buf, row, 0) < 0) {
                _TIFFfree(buf);
                TIFFClose(tif);
                return false;
            }
            src += linebytes;
        }
        _TIFFfree(buf);
    }

    TIFFClose(tif);
    return true;
}

bool WebpToTiffConverter::validate_input(const ImageData& data) {
    // 确保是有效的图像数据
    return data.width > 0 && data.height > 0 && 
          (data.channels == 3 || data.channels == 4);
}
