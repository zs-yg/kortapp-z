#ifndef UTILS_HPP
#define UTILS_HPP

#include <memory>
#include "Converter.hpp"
#include "BinaryConverter.hpp"
#include "HexConverter.hpp"
#include "ROT13Converter.hpp"
#include "MD5Converter.hpp"
#include "SHA1Converter.hpp"
#include "SHA256Converter.hpp"
#include "SHA224Converter.hpp"
#include "SHA384Converter.hpp"
#include "SHA512Converter.hpp"
#include "SHA3Converter.hpp"
#include "Base64Converter.hpp"
#include "Base32Converter.hpp"
#include "Ascii85Converter.hpp"
#include "CRC32Converter.hpp"

class Utils {
public:
    static std::unique_ptr<Converter> createConverter(int type);
};

#endif // UTILS_HPP
