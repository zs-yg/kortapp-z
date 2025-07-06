#ifndef UTILS_HPP
#define UTILS_HPP

#include "../include/common.hpp"
#include <vector>

// 字符串工具
String trim(const String& str);
std::vector<String> split(const String& str, char delimiter);
String join(const std::vector<String>& strings, const String& delimiter);

// 图像工具
bool isImageFile(const String& filePath);
String getFileExtension(const String& filePath);

// 系统工具
String getCurrentDateTime();
String getHomeDirectory();

#endif // UTILS_HPP
