#include <iostream>
#include <filesystem>
#include <chrono>

namespace fs = std::filesystem;

int main() {
    try {
        auto start = std::chrono::high_resolution_clock::now();
        
        // 定义日志目录路径
        fs::path logDir;
        
        #ifdef _WIN32
            // Windows系统获取AppData路径
            char* appData = nullptr;
            size_t len = 0;
            if (_dupenv_s(&appData, &len, "APPDATA") == 0 && appData != nullptr) {
                logDir = fs::path(appData) / "zsyg" / "kortapp-z" / ".logs";
                free(appData);
            } else {
                std::cerr << "无法获取APPDATA环境变量" << std::endl;
                return 1;
            }
        #else
            // 非Windows系统使用默认路径
            logDir = fs::path(getenv("HOME")) / ".zsyg" / "kortapp-z" / ".logs";
        #endif
        size_t deletedCount = 0;
        size_t errorCount = 0;

        // 检查目录是否存在
        if (fs::exists(logDir) && fs::is_directory(logDir)) {
            // 遍历并删除所有日志文件
            for (const auto& entry : fs::directory_iterator(logDir)) {
                try {
                    if (fs::is_regular_file(entry)) {
                        fs::remove(entry);
                        deletedCount++;
                    }
                } catch (const std::exception& e) {
                    std::cerr << "删除文件失败: " << entry.path() << " - " << e.what() << std::endl;
                    errorCount++;
                }
            }
        } else {
            std::cout << "日志目录不存在，无需清理" << std::endl;
            return 0;
        }

        auto end = std::chrono::high_resolution_clock::now();
        auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start);

        std::cout << "日志清理完成: " << std::endl;
        std::cout << "删除文件数: " << deletedCount << std::endl;
        std::cout << "错误数: " << errorCount << std::endl;
        std::cout << "耗时: " << duration.count() << " 毫秒" << std::endl;

    } catch (const std::exception& e) {
        std::cerr << "发生错误: " << e.what() << std::endl;
        return 1;
    }

    return 0;
}
