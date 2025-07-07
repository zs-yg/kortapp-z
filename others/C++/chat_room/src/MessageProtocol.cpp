#include "MessageProtocol.hpp"
#include <sstream>
#include "StringUtils.hpp"

std::string MessageProtocol::encodeMessage(const Message& message) {
    std::ostringstream oss;
    oss << static_cast<int>(message.getType()) << "|"
        << (message.getSender() ? message.getSender()->getUsername() : "") << "|"
        << message.getContent();
    return oss.str();
}

Message MessageProtocol::decodeMessage(const std::string& data) {
    auto parts = StringUtils::split(data, '|');
    if (parts.size() < 3) {
        throw std::runtime_error("Invalid message format");
    }

    std::shared_ptr<User> sender = nullptr;
    if (!parts[1].empty()) {
        sender = std::make_shared<User>(parts[1], "");
    }
    
    MessageType type = static_cast<MessageType>(std::stoi(parts[0]));
    std::string content = parts[2];
    
    if (type == MessageType::USER_JOIN || type == MessageType::USER_LEAVE) {
        content = "";
    }

    return Message(sender, content, type);
}

bool MessageProtocol::validateMessage(const std::string& data) {
    auto parts = StringUtils::split(data, '|');
    if (parts.size() < 3) {
        return false;
    }
    
    try {
        int type = std::stoi(parts[0]);
        return type >= 0 && type <= 5; // 检查消息类型是否有效(0-5对应MessageType枚举)
    } catch (...) {
        return false;
    }
}
