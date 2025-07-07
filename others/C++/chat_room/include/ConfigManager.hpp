#ifndef CONFIGMANAGER_HPP
#define CONFIGMANAGER_HPP

#include <string>
#include <map>

class ConfigManager {
public:
    static ConfigManager& getInstance();
    
    void loadConfig(const std::string& filename);
    void saveConfig(const std::string& filename);
    
    std::string getValue(const std::string& key);
    void setValue(const std::string& key, const std::string& value);

private:
    ConfigManager() = default;
    std::map<std::string, std::string> configMap;
};

#endif // CONFIGMANAGER_HPP
