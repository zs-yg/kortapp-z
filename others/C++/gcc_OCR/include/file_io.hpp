#ifndef FILE_IO_HPP
#define FILE_IO_HPP

#include "../include/common.hpp"
#include <vector>

// 保存文本到文件
bool saveTextToFile(const String& filePath, const String& content);

// 从文件加载文本
String loadTextFromFile(const String& filePath);

// 获取支持的图像格式列表
std::vector<String> getSupportedImageFormats();

#endif // FILE_IO_HPP
