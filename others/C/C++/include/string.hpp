#ifndef STRING_HPP
#define STRING_HPP

#include <cstddef>

class String {
public:
    String();
    String(const char* str);
    String(const String& other);
    ~String();

    size_t length() const;
    const char* c_str() const;
    
    String& operator=(const String& other);
    String operator+(const String& other) const;
    bool operator==(const String& other) const;

private:
    char* data;
    size_t len;
};

#endif // STRING_HPP
