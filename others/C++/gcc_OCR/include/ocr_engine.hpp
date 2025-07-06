#ifndef OCR_ENGINE_HPP
#define OCR_ENGINE_HPP

#include <tesseract/baseapi.h>
#include <leptonica/allheaders.h>
#include "../include/common.hpp"

class OCREngine {
public:
    OCREngine();
    ~OCREngine();
    
    // 设置识别语言
    bool setLanguage(const String& lang);
    
    // 从图像文件识别文本
    String recognizeFromFile(const String& filePath);
    
    // 从内存图像识别文本
    String recognizeFromImage(Pix* image);
    
private:
    tesseract::TessBaseAPI* api;
    String currentLanguage;
    
    // 初始化Tesseract
    void initTesseract();
};

#endif // OCR_ENGINE_HPP
