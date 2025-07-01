#include "password_generator.hpp"
#include <cstdlib>
#include <ctime>

PasswordGenerator::PasswordGenerator() {
    srand(time(nullptr));
}

String PasswordGenerator::generate(int length, Mode mode) {
    if (length < 1 || length > 100) {
        return String("无效长度(1-100)");
    }

    switch (mode) {
        case DIGITS:
            return generateDigits(length);
        case ENGLISH:
            return generateEnglish(length);
        case SYMBOLS:
            return generateSymbols(length);
        case DIGITS_ENGLISH: {
            String charSet = String("0123456789") + 
                           String("abcdefghijklmnopqrstuvwxyz") + 
                           String("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            return generateMixed(length, charSet);
        }
        case DIGITS_SYMBOLS: {
            String charSet = String("0123456789") + String("!@#$%^&*()_+-=[]{}|;:,.<>?");
            return generateMixed(length, charSet);
        }
        case ENGLISH_SYMBOLS: {
            String charSet = String("abcdefghijklmnopqrstuvwxyz") + 
                           String("ABCDEFGHIJKLMNOPQRSTUVWXYZ") + 
                           String("!@#$%^&*()_+-=[]{}|;:,.<>?");
            return generateMixed(length, charSet);
        }
        default:
            return String("不支持的模式");
    }
}

String PasswordGenerator::generateDigits(int length) {
    char* buffer = new char[length + 1];
    for (int i = 0; i < length; ++i) {
        buffer[i] = '0' + (rand() % 10);
    }
    buffer[length] = '\0';
    
    String result(buffer);
    delete[] buffer;
    return result;
}

String PasswordGenerator::generateEnglish(int length) {
    char* buffer = new char[length + 1];
    for (int i = 0; i < length; ++i) {
        int choice = rand() % 2;
        if (choice == 0) {
            buffer[i] = 'a' + (rand() % 26); // 小写字母
        } else {
            buffer[i] = 'A' + (rand() % 26); // 大写字母
        }
    }
    buffer[length] = '\0';
    
    String result(buffer);
    delete[] buffer;
    return result;
}

String PasswordGenerator::generateSymbols(int length) {
    const char symbols[] = "!@#$%^&*()_+-=[]{}|;:,.<>?";
    const int symbolCount = sizeof(symbols) - 1; // 减去末尾的\0
    
    char* buffer = new char[length + 1];
    for (int i = 0; i < length; ++i) {
        buffer[i] = symbols[rand() % symbolCount];
    }
    buffer[length] = '\0';
    
    String result(buffer);
    delete[] buffer;
    return result;
}

String PasswordGenerator::generateMixed(int length, const String& charSet) {
    char* buffer = new char[length + 1];
    for (int i = 0; i < length; ++i) {
        buffer[i] = charSet.c_str()[rand() % charSet.length()];
    }
    buffer[length] = '\0';
    
    String result(buffer);
    delete[] buffer;
    return result;
}
