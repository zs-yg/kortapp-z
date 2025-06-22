#include <iostream>
#include <filesystem>
#include <string>
#include <windows.h>

#define STB_IMAGE_IMPLEMENTATION
#include <stb/stb_image.h>
#define STB_IMAGE_WRITE_IMPLEMENTATION
#include <stb/stb_image_write.h>

namespace fs = std::filesystem;

void ShowHelp() {
    std::cout << "图片压缩工具 v1.0\n";
    std::cout << "用法: image_compressor [选项] 输入文件 输出文件\n";
    std::cout << "选项:\n";
    std::cout << "  -t <type>      压缩类型: lossy(有损)/lossless(无损) (默认: lossy)\n";
    std::cout << "  -q <quality>   压缩质量 (1-1000) (默认: 80)\n";
    std::cout << "  -e             保留EXIF信息 (默认: 不保留)\n";
    std::cout << "  -h             显示帮助信息\n";
}

int main(int argc, char** argv) {
    // 默认参数
    std::string compressType = "lossy";
    int quality = 80;
    bool keepExif = false;
    std::string inputFile;
    std::string outputFile;

    // 解析命令行参数
    for (int i = 1; i < argc; ++i) {
        std::string arg = argv[i];
        if (arg == "-h") {
            ShowHelp();
            return 0;
        } else if (arg == "-t" && i + 1 < argc) {
            compressType = argv[++i];
        } else if (arg == "-q" && i + 1 < argc) {
            quality = std::stoi(argv[++i]);
        } else if (arg == "-e") {
            keepExif = true;
        } else if (inputFile.empty()) {
            inputFile = arg;
        } else {
            outputFile = arg;
        }
    }

    // 验证参数
    if (inputFile.empty() || outputFile.empty()) {
        std::cerr << "错误: 必须指定输入文件和输出文件\n";
        ShowHelp();
        return 1;
    }

    if (quality < 1 || quality > 1000) {
        std::cerr << "错误: 压缩质量必须在1-1000之间\n";
        return 1;
    }

    try {
        std::cout << "正在压缩: " << inputFile << " -> " << outputFile << std::endl;
        std::cout << "类型: " << compressType << std::endl;
        std::cout << "质量: " << quality << std::endl;
        std::cout << "保留EXIF: " << (keepExif ? "是" : "否") << std::endl;

        // 加载图片
        int width, height, channels;
        unsigned char* image = stbi_load(inputFile.c_str(), &width, &height, &channels, 0);
        if (!image) {
            std::cerr << "错误: 无法加载图片 " << inputFile << " - " << stbi_failure_reason() << std::endl;
            return 1;
        }

        // 压缩图片
        int result = 0;
        if (compressType == "lossy") {
            // 有损压缩 (JPEG)
            int jpeg_quality = quality / 10;  // 将1-1000映射到1-100
            jpeg_quality = std::max(1, std::min(jpeg_quality, 100));
            result = stbi_write_jpg(outputFile.c_str(), width, height, channels, image, jpeg_quality);
        } else {
            // 无损压缩 (PNG)
            result = stbi_write_png(outputFile.c_str(), width, height, channels, image, width * channels);
        }

        // 释放内存
        stbi_image_free(image);

        if (!result) {
            std::cerr << "错误: 无法保存图片 " << outputFile << std::endl;
            return 1;
        }

        std::cout << "压缩完成\n";
        return 0;

    } catch (const std::exception& e) {
        std::cerr << "发生错误: " << e.what() << std::endl;
        return 1;
    }
}
