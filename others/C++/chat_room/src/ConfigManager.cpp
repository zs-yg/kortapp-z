#include "ConfigManager.hpp"
#include <fstream>
#include <sstream>

ConfigManager& ConfigManager::getInstance() {
    static ConfigManager instance;
    return instance;
}

void ConfigManager::loadConfig(const std::string& filename) {
    std::ifstream file(filename);
    std::string line;
    
    while (std::getline(file, line)) {
        size_t pos = line.find('=');
        if (pos != std::string::npos) {
            std::string key = line.substr(0, pos);
            std::string value = line.substr(pos + 1);
            configMap[key] = value;
        }
    }
}

void ConfigManager::saveConfig(const std::string& filename) {
    std::ofstream file(filename);
    
    for (const auto& pair : configMap) {
        file << pair.first << "=" << pair.second << "\n";
    }
}

std::string ConfigManager::getValue(const std::string& key) {
    return configMap[key];
}

void ConfigManager::setValue(const std::string& key, const std::string& value) {
    configMap[key] = value;
}
