#include "NetworkManager.hpp"
#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <stdexcept>

#pragma comment(lib, "ws2_32.lib")

NetworkManager::NetworkManager() : serverSocket(INVALID_SOCKET), clientSocket(INVALID_SOCKET) {
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        throw std::runtime_error("WSAStartup failed");
    }
}

NetworkManager::~NetworkManager() {
    if (serverSocket != INVALID_SOCKET) {
        closesocket(serverSocket);
    }
    if (clientSocket != INVALID_SOCKET) {
        closesocket(clientSocket);
    }
    WSACleanup();
}

bool NetworkManager::startServer(int port) {
    serverSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (serverSocket == INVALID_SOCKET) {
        return false;
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = htonl(INADDR_ANY); // 监听所有网络接口
    serverAddr.sin_port = htons(port);
    
    // 设置SO_REUSEADDR选项
    int opt = 1;
    setsockopt(serverSocket, SOL_SOCKET, SO_REUSEADDR, (const char*)&opt, sizeof(opt));

    if (bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        std::cerr << "绑定端口失败: " << WSAGetLastError() << std::endl;
        closesocket(serverSocket);
        return false;
    }

    if (listen(serverSocket, SOMAXCONN) == SOCKET_ERROR) {
        std::cerr << "监听失败: " << WSAGetLastError() << std::endl;
        closesocket(serverSocket);
        return false;
    }
    
    // 启动接收线程
    acceptThread = std::thread([this]() {
        while (true) {
            sockaddr_in clientAddr;
            int addrLen = sizeof(clientAddr);
            SOCKET newClient = accept(serverSocket, (sockaddr*)&clientAddr, &addrLen);
            if (newClient == INVALID_SOCKET) break;
            
            std::lock_guard<std::mutex> lock(clientsMutex);
            clients.push_back(newClient);
        }
    });
    
    std::cout << "服务器已启动，监听端口: " << port << std::endl;
    return true;
}

bool NetworkManager::connectToServer(const std::string& ip, int port) {
    clientSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (clientSocket == INVALID_SOCKET) {
        return false;
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    inet_pton(AF_INET, ip.c_str(), &serverAddr.sin_addr);
    serverAddr.sin_port = htons(port);

    if (connect(clientSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        std::cerr << "连接服务器失败: " << WSAGetLastError() << std::endl;
        closesocket(clientSocket);
        return false;
    }
    
    std::cout << "已连接到服务器: " << ip << ":" << port << std::endl;

    return true;
}

void NetworkManager::sendMessage(const std::string& message) {
    // 服务器模式：广播给所有客户端
    if (serverSocket != INVALID_SOCKET) {
        std::lock_guard<std::mutex> lock(clientsMutex);
        for (auto client : clients) {
            send(client, message.c_str(), message.size(), 0);
        }
    }
    // 客户端模式：发送到服务器
    else if (clientSocket != INVALID_SOCKET) {
        send(clientSocket, message.c_str(), message.size(), 0);
    }
    
    // 本地回显
    if (messageCallback) {
        messageCallback(message);
    }
}

std::vector<std::string> NetworkManager::receiveMessages() {
    std::vector<std::string> messages;
    if (clientSocket == INVALID_SOCKET) {
        return messages;
    }

    fd_set readSet;
    FD_ZERO(&readSet);
    FD_SET(clientSocket, &readSet);

    struct timeval timeout;
    timeout.tv_sec = 1;
    timeout.tv_usec = 0;

    if (select(clientSocket + 1, &readSet, nullptr, nullptr, &timeout) > 0) {
        char buffer[1024];
        int bytesReceived;
        while ((bytesReceived = recv(clientSocket, buffer, sizeof(buffer), 0)) > 0) {
            messages.emplace_back(buffer, bytesReceived);
        }
    }

    return messages;
}

bool NetworkManager::isConnected() const {
    return clientSocket != INVALID_SOCKET;
}
