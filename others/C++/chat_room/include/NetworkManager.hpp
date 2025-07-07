#ifndef NETWORKMANAGER_HPP
#define NETWORKMANAGER_HPP

#include <string>
#include <vector>
#include <memory>
#include <thread>
#include <mutex>
#include <functional>
#include <winsock2.h>

class NetworkManager {
public:
    using MessageCallback = std::function<void(const std::string&)>;
    
    NetworkManager();
    ~NetworkManager();
    
    bool startServer(int port);
    bool connectToServer(const std::string& ip, int port);
    void sendMessage(const std::string& message);
    std::vector<std::string> receiveMessages();
    bool isConnected() const;
    
    void setMessageCallback(MessageCallback callback) {
        messageCallback = callback;
    }
    
private:
    SOCKET serverSocket;
    SOCKET clientSocket;
    std::vector<SOCKET> clients;
    std::mutex clientsMutex;
    std::thread acceptThread;
    MessageCallback messageCallback;
};

#endif // NETWORKMANAGER_HPP
