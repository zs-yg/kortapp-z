#ifndef CONFIG_HPP
#define CONFIG_HPP

#include "../include/common.hpp"
#include <map>

struct AppConfig {
    String language;  // OCR识别语言
    String lastDir;    // 最后打开的目录
    int windowWidth;   // 窗口宽度
    int windowHeight;  // 窗口高度
};

class ConfigManager {
public:
    ConfigManager();
    
    // 加载配置
    bool loadConfig(const String& filePath = "config.ini");
    
    // 保存配置
    bool saveConfig(const String& filePath = "config.ini");
    
    // 获取配置
    AppConfig getConfig() const;
    
    // 更新配置
    void updateConfig(const AppConfig& newConfig);
    
private:
    AppConfig config;
    
    // 解析INI文件
    void parseIni(const String& content);
    
    // 生成INI文件内容
    String generateIni() const;
};

#endif // CONFIG_HPP
