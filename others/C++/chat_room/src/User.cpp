#include "User.hpp"
#include <chrono>

User::User(const std::string& username, const std::string& ip) 
    : username(username), ipAddress(ip) {
    updateLastActive();
}

const std::string& User::getUsername() const {
    return username;
}

const std::string& User::getIpAddress() const {
    return ipAddress;
}

bool User::isActive() const {
    auto now = std::chrono::system_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::minutes>(now - lastActive);
    return duration.count() < 5; // 5分钟内活跃
}

void User::updateLastActive() {
    lastActive = std::chrono::system_clock::now();
}
