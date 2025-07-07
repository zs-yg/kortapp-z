#include "../include/ROT13Converter.hpp"

std::string ROT13Converter::convert(const std::string& input) {
    std::string result;
    for (char c : input) {
        if (isalpha(c)) {
            char base = isupper(c) ? 'A' : 'a';
            c = (c - base + 13) % 26 + base;
        }
        result += c;
    }
    return result;
}
