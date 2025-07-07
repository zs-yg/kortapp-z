#ifndef MESSAGE_HPP
#define MESSAGE_HPP

#include <string>
#include <chrono>
#include <memory>
#include "User.hpp"

enum class MessageType {
    NORMAL,        // 普通消息
    SYSTEM,        // 系统消息
    TEXT_MESSAGE,  // 文本消息
    USER_JOIN,     // 用户加入
    USER_LEAVE,    // 用户离开
    AUTH           // 认证消息
};

class Message {
public:
    Message(const std::shared_ptr<User>& sender, 
            const std::string& content,
            MessageType type = MessageType::NORMAL);
            
    const std::shared_ptr<User>& getSender() const;
    const std::string& getContent() const;
    MessageType getType() const;
    std::chrono::system_clock::time_point getTimestamp() const;
    
private:
    std::shared_ptr<User> sender;
    std::string content;
    MessageType type;
    std::chrono::system_clock::time_point timestamp;
};

#endif // MESSAGE_HPP
