#ifndef NETWORKEVENTHANDLER_HPP
#define NETWORKEVENTHANDLER_HPP

#include <memory>
#include "ChatRoom.hpp"
#include "MessageProtocol.hpp"

#include <memory>
#include <thread>
#include <atomic>
#include "ChatRoom.hpp"
#include "Message.hpp"

class NetworkEventHandler {
public:
    explicit NetworkEventHandler(std::shared_ptr<ChatRoom> chatRoom);
    ~NetworkEventHandler();
    
    void start();
    void stop();
    
    void onConnected();
    void onMessageReceived(const std::string& message);
    void onError(const std::string& error);

private:
    void heartbeatCheck();
    
    std::shared_ptr<ChatRoom> chatRoom;
    std::atomic<bool> running;
    std::thread heartbeatThread;
};

#endif // NETWORKEVENTHANDLER_HPP
