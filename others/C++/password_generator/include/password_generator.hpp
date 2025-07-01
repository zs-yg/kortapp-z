#ifndef PASSWORD_GENERATOR_HPP
#define PASSWORD_GENERATOR_HPP

#include "string.hpp"

class PasswordGenerator {
public:
    enum Mode {
        DIGITS,         // 纯数字
        ENGLISH,        // 纯英文
        SYMBOLS,        // 纯符号
        DIGITS_ENGLISH, // 数字+英文
        DIGITS_SYMBOLS, // 数字+符号
        ENGLISH_SYMBOLS // 英文+符号
    };

    PasswordGenerator();
    String generate(int length, Mode mode = DIGITS);

private:
    String generateDigits(int length);
    String generateEnglish(int length);
    String generateSymbols(int length);
    String generateMixed(int length, const String& charSet);
};

#endif // PASSWORD_GENERATOR_HPP
