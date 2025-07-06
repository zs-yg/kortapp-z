#ifndef IMAGE_PROCESSOR_HPP
#define IMAGE_PROCESSOR_HPP

#include "../include/common.hpp"
#include <leptonica/allheaders.h>

class ImageProcessor {
public:
    // 从文件加载图像并进行预处理
    static Pix* loadAndPreprocess(const String& filePath);
    
    // 图像预处理
    static Pix* preprocess(Pix* image);
    
    // 转换为灰度图像
    static Pix* convertToGrayscale(Pix* image);
    
    // 二值化处理
    static Pix* binarize(Pix* image);
    
    // 调整对比度
    static Pix* adjustContrast(Pix* image, float factor);
    
    // 调整亮度
    static Pix* adjustBrightness(Pix* image, int delta);
    
    // 降噪处理
    static Pix* removeNoise(Pix* image);
};

#endif // IMAGE_PROCESSOR_HPP
