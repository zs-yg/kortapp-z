#ifndef ABOUT_HPP
#define ABOUT_HPP

#include <string>

class About {
public:
    static std::string getName() { return "文本转换器"; }
    static std::string getVersion() { return "1.0.0"; }
    static std::string getAuthor() { return "zsyg"; }
    static std::string getDescription() { 
        return "一个简单的文本转换工具，支持文本转换"; 
    }
};

#endif // ABOUT_HPP
