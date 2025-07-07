#include "NetworkEventHandler.hpp"
#include <iostream>
#include <chrono>
#include "MessageProtocol.hpp"

NetworkEventHandler::NetworkEventHandler(std::shared_ptr<ChatRoom> chatRoom)
    : chatRoom(chatRoom), running(false) {}

NetworkEventHandler::~NetworkEventHandler() {
    stop();
}

void NetworkEventHandler::start() {
    if (running) return;
    running = true;
    heartbeatThread = std::thread(&NetworkEventHandler::heartbeatCheck, this);
}

void NetworkEventHandler::stop() {
    running = false;
    if (heartbeatThread.joinable()) {
        heartbeatThread.join();
    }
}

void NetworkEventHandler::onConnected() {
    auto users = chatRoom->getUsers();
    if (!users.empty()) {
        auto user = users[0]; // 假设第一个用户是当前用户
        Message joinMsg(user, "", MessageType::USER_JOIN);
        chatRoom->sendChatMessage(MessageProtocol::encodeMessage(joinMsg));
    }
}

void NetworkEventHandler::onMessageReceived(const std::string& message) {
    try {
        if (!MessageProtocol::validateMessage(message)) {
            return;
        }

        auto msg = MessageProtocol::decodeMessage(message);
        switch (msg.getType()) {
            case MessageType::TEXT_MESSAGE:
                chatRoom->sendChatMessage(msg.getSender()->getUsername() + ": " + msg.getContent());
                break;
            case MessageType::USER_JOIN:
                chatRoom->sendChatMessage(msg.getSender()->getUsername() + " 加入了聊天室");
                break;
            case MessageType::USER_LEAVE:
                chatRoom->sendChatMessage(msg.getSender()->getUsername() + " 离开了聊天室");
                break;
            case MessageType::SYSTEM:
                chatRoom->sendChatMessage("[系统] " + msg.getContent());
                break;
            case MessageType::AUTH:
                if (msg.getContent() == chatRoom->getRoomPassword()) {
                    chatRoom->sendChatMessage("AUTH_SUCCESS");
                } else {
                    chatRoom->sendChatMessage("AUTH_FAILED");
                }
                break;
            default:
                break;
        }
    } catch (const std::exception& e) {
        std::cerr << "消息处理错误: " << e.what() << std::endl;
    }
}

void NetworkEventHandler::onError(const std::string& error) {
    chatRoom->sendChatMessage("[网络错误] " + error);
}

void NetworkEventHandler::heartbeatCheck() {
    while (running) {
        std::this_thread::sleep_for(std::chrono::seconds(30));
        if (!chatRoom->getNetworkManager()->isConnected()) {
            onError("连接已断开");
        }
    }
}
