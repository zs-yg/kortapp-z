#include "../include/image_processor.hpp"
#include "../include/error_handler.hpp"
#include <leptonica/allheaders.h>

Pix* ImageProcessor::loadAndPreprocess(const String& filePath) {
    Pix* image = pixRead(filePath.c_str());
    if (!image) {
        THROW_EXCEPTION("无法加载图像文件: " + filePath);
    }
    return preprocess(image);
}

Pix* ImageProcessor::preprocess(Pix* image) {
    Pix* processed = convertToGrayscale(image);
    processed = adjustContrast(processed, 1.5f);
    processed = binarize(processed);
    processed = removeNoise(processed);
    return processed;
}

Pix* ImageProcessor::convertToGrayscale(Pix* image) {
    Pix* gray = pixConvertRGBToGray(image, 0.3f, 0.59f, 0.11f);
    if (!gray) {
        pixDestroy(&image);
        THROW_EXCEPTION("灰度转换失败");
    }
    return gray;
}

Pix* ImageProcessor::binarize(Pix* image) {
    // 使用更精细的局部二值化
    if (pixOtsuAdaptiveThreshold(image, 16, 16, 5, 5, 0.1f, nullptr, nullptr) != 0) {
        pixDestroy(&image);
        THROW_EXCEPTION("二值化失败");
    }
    return image;
}

Pix* ImageProcessor::adjustContrast(Pix* image, float factor) {
    Pix* contrast = pixContrastTRC(nullptr, image, factor);
    if (!contrast) {
        pixDestroy(&image);
        THROW_EXCEPTION("对比度调整失败");
    }
    return contrast;
}

Pix* ImageProcessor::adjustBrightness(Pix* image, int delta) {
    if (pixMultConstantGray(image, 1.0f + delta/255.0f) != 0) {
        pixDestroy(&image);
        THROW_EXCEPTION("亮度调整失败");
    }
    return image;
}

Pix* ImageProcessor::removeNoise(Pix* image) {
    // 增强降噪效果
    Pix* denoised = pixCleanImage(image, 2, 0, 2, 10);
    if (!denoised) {
        pixDestroy(&image);
        THROW_EXCEPTION("降噪失败");
    }
    return denoised;
}
