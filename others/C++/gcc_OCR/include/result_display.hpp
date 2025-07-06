#ifndef RESULT_DISPLAY_HPP
#define RESULT_DISPLAY_HPP

#include "../include/common.hpp"
#include <vector>

class ResultDisplay {
public:
    // 格式化OCR结果
    static String formatResult(const String& rawText);
    
    // 校正常见OCR错误
    static String correctCommonErrors(const String& text);
    
    // 分割段落
    static std::vector<String> splitParagraphs(const String& text);
    
    // 高亮低置信度区域
    static String highlightLowConfidence(const String& text, const std::vector<float>& confidences);
    
    // 导出为不同格式
    static bool exportAsText(const String& filePath, const String& content);
    static bool exportAsHtml(const String& filePath, const String& content);
    static bool exportAsPdf(const String& filePath, const String& content);
};

#endif // RESULT_DISPLAY_HPP
