#include "../include/file_io.hpp"
#include "../include/error_handler.hpp"
#include <fstream>
#include <sstream>

bool saveTextToFile(const String& filePath, const String& content) {
    std::ofstream outFile(filePath);
    if (!outFile) {
        THROW_EXCEPTION("无法打开文件进行写入: " + filePath);
        return false;
    }
    
    outFile << content;
    outFile.close();
    return true;
}

String loadTextFromFile(const String& filePath) {
    std::ifstream inFile(filePath);
    if (!inFile) {
        THROW_EXCEPTION("无法打开文件进行读取: " + filePath);
    }
    
    std::stringstream buffer;
    buffer << inFile.rdbuf();
    inFile.close();
    return buffer.str();
}

std::vector<String> getSupportedImageFormats() {
    return {
        "*.png",
        "*.jpg",
        "*.jpeg",
        "*.bmp",
        "*.tif",
        "*.tiff"
    };
}
