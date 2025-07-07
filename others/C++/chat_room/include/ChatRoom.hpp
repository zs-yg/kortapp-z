#ifndef CHATROOM_HPP
#define CHATROOM_HPP

#include "NetworkManager.hpp"
#include "User.hpp"
#include <string>
#include <vector>
#include <memory>

class ChatRoom {
public:
    ChatRoom();
    ~ChatRoom();
    
    bool createRoom(int port, const std::string& password = "");
    bool joinRoom(const std::string& ip, int port, const std::string& password);
    void sendChatMessage(const std::string& message);
    void addUser(const std::string& username);
    void removeUser(const std::string& username);
    
    const std::vector<std::string>& getMessages() const;
    const std::vector<std::shared_ptr<User>>& getUsers() const;
    std::string getCurrentUsername() const;
    bool isConnected() const;
    NetworkManager* getNetworkManager() const;
    std::string getRoomPassword() const;
    std::string getLocalIP() const;
    
private:
    std::unique_ptr<NetworkManager> network;
    std::vector<std::string> messages;
    std::vector<std::shared_ptr<User>> users;
    std::string roomPassword;
};

#endif // CHATROOM_HPP
