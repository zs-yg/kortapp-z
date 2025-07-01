#include "string.hpp"
#include <cstring>

String::String() : data(nullptr), len(0) {}

String::String(const char* str) {
    len = strlen(str);
    data = new char[len + 1];
    strcpy(data, str);
}

String::String(const String& other) {
    len = other.len;
    data = new char[len + 1];
    strcpy(data, other.data);
}

String::~String() {
    delete[] data;
}

size_t String::length() const {
    return len;
}

const char* String::c_str() const {
    return data;
}

String& String::operator=(const String& other) {
    if (this != &other) {
        delete[] data;
        len = other.len;
        data = new char[len + 1];
        strcpy(data, other.data);
    }
    return *this;
}

String String::operator+(const String& other) const {
    String result;
    result.len = len + other.len;
    result.data = new char[result.len + 1];
    strcpy(result.data, data);
    strcat(result.data, other.data);
    return result;
}

bool String::operator==(const String& other) const {
    return strcmp(data, other.data) == 0;
}
