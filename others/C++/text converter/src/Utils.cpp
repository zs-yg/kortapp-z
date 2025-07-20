#include "../include/Utils.hpp"
#include "../include/ROT13Converter.hpp"
#include "../include/MD5Converter.hpp"
#include "../include/SHA1Converter.hpp"
#include "../include/SHA256Converter.hpp"
#include "../include/Base64Converter.hpp"
#include "../include/Base32Converter.hpp"
#include "../include/Ascii85Converter.hpp"
#include "../include/CRC32Converter.hpp"

std::unique_ptr<Converter> Utils::createConverter(int type) {
    switch (type) {
        case 0: return std::unique_ptr<Converter>(new BinaryConverter());
        case 1: return std::unique_ptr<Converter>(new HexConverter());
        case 2: return std::unique_ptr<Converter>(new ROT13Converter());
        case 3: return std::unique_ptr<Converter>(new MD5Converter());
        case 4: return std::unique_ptr<Converter>(new SHA1Converter());
        case 5: return std::unique_ptr<Converter>(new SHA256Converter());
        case 6: return std::unique_ptr<Converter>(new Base64Converter());
        case 7: return std::unique_ptr<Converter>(new Base32Converter());
        case 8: return std::unique_ptr<Converter>(new Ascii85Converter());
        case 9: return std::unique_ptr<Converter>(new CRC32Converter());
        default: return nullptr;
    }
}
