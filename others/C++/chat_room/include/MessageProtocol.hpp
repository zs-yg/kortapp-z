#ifndef MESSAGEPROTOCOL_HPP
#define MESSAGEPROTOCOL_HPP

#include <string>
#include <vector>
#include "Message.hpp"

class MessageProtocol {
public:
    static std::string encodeMessage(const Message& message);
    static Message decodeMessage(const std::string& data);
    static bool validateMessage(const std::string& data);
};

#endif // MESSAGEPROTOCOL_HPP
