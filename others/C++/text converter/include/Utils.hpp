#ifndef UTILS_HPP
#define UTILS_HPP

#include <memory>
#include "Converter.hpp"
#include "BinaryConverter.hpp"
#include "HexConverter.hpp"

class Utils {
public:
    static std::unique_ptr<Converter> createConverter(int type);
};

#endif // UTILS_HPP
