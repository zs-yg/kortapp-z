#include "ChatRoom.hpp"
#include "Message.hpp"
#include "MessageProtocol.hpp"
#include <iostream>
#include <algorithm>
#include <winsock2.h>
#include <ws2tcpip.h>

ChatRoom::ChatRoom() : network(std::make_unique<NetworkManager>()) {}

ChatRoom::~ChatRoom() {}

bool ChatRoom::createRoom(int port, const std::string& password) {
    roomPassword = password;
    if (!network->startServer(port)) {
        std::cerr << "Failed to create room on port " << port << std::endl;
        return false;
    }
    return true;
}

bool ChatRoom::joinRoom(const std::string& ip, int port, const std::string& password) {
    if (!network->connectToServer(ip, port)) {
        std::cerr << "Failed to connect to " << ip << ":" << port << std::endl;
        return false;
    }
    
    // 发送密码验证
    Message authMsg(nullptr, password, MessageType::AUTH);
    network->sendMessage(MessageProtocol::encodeMessage(authMsg));
    
    // 等待验证响应
    auto response = network->receiveMessages();
    if (response.empty() || response[0] != "AUTH_SUCCESS") {
        return false;
    }
    
    return true;
}

std::string ChatRoom::getRoomPassword() const {
    return roomPassword;
}

std::string ChatRoom::getLocalIP() const {
    char host[256];
    if (gethostname(host, sizeof(host)) == SOCKET_ERROR) {
        return "127.0.0.1";
    }
    
    struct hostent* phe = gethostbyname(host);
    if (phe == nullptr) {
        return "127.0.0.1";
    }
    
    for (int i = 0; phe->h_addr_list[i] != nullptr; ++i) {
        struct in_addr addr;
        memcpy(&addr, phe->h_addr_list[i], sizeof(struct in_addr));
        std::string ip = inet_ntoa(addr);
        if (ip != "127.0.0.1") {
            return ip;
        }
    }
    
    return "127.0.0.1";
}

void ChatRoom::sendChatMessage(const std::string& message) {
    network->sendMessage(message);
    messages.push_back(message);
}

void ChatRoom::addUser(const std::string& username) {
    auto user = std::make_shared<User>(username, "");
    users.push_back(user);
}

void ChatRoom::removeUser(const std::string& username) {
    users.erase(
        std::remove_if(users.begin(), users.end(), 
            [&username](const std::shared_ptr<User>& user) {
                return user->getUsername() == username;
            }),
        users.end());
}

const std::vector<std::string>& ChatRoom::getMessages() const {
    return messages;
}

const std::vector<std::shared_ptr<User>>& ChatRoom::getUsers() const {
    return users;
}

std::string ChatRoom::getCurrentUsername() const {
    if (!users.empty()) {
        return users[0]->getUsername();
    }
    return "";
}

NetworkManager* ChatRoom::getNetworkManager() const {
    return network.get();
}
