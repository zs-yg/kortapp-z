#ifndef USER_HPP
#define USER_HPP

#include <string>
#include <chrono>

class User {
public:
    User(const std::string& username, const std::string& ip);
    
    const std::string& getUsername() const;
    const std::string& getIpAddress() const;
    bool isActive() const;
    void updateLastActive();
    
private:
    std::string username;
    std::string ipAddress;
    std::chrono::system_clock::time_point lastActive;
};

#endif // USER_HPP
