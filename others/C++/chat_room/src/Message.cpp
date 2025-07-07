#include "Message.hpp"

Message::Message(const std::shared_ptr<User>& sender, 
                 const std::string& content,
                 MessageType type)
    : sender(sender), content(content), type(type) {
    timestamp = std::chrono::system_clock::now();
}

const std::shared_ptr<User>& Message::getSender() const {
    return sender;
}

const std::string& Message::getContent() const {
    return content;
}

MessageType Message::getType() const {
    return type;
}

std::chrono::system_clock::time_point Message::getTimestamp() const {
    return timestamp;
}
