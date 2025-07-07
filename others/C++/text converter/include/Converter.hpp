#ifndef CONVERTER_HPP
#define CONVERTER_HPP

#include <string>

class Converter {
public:
    virtual ~Converter() = default;
    virtual std::string convert(const std::string& input) = 0;
    virtual std::string getName() const = 0;
};

#endif // CONVERTER_HPP
