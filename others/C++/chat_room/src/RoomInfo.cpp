#include "RoomInfo.hpp"

RoomInfo::RoomInfo(const std::string& name, 
                   const std::string& ip,
                   int port,
                   const std::string& password)
    : name(name), ipAddress(ip), port(port), password(password), userCount(0) {}

const std::string& RoomInfo::getName() const {
    return name;
}

const std::string& RoomInfo::getIpAddress() const {
    return ipAddress;
}

int RoomInfo::getPort() const {
    return port;
}

const std::string& RoomInfo::getPassword() const {
    return password;
}

int RoomInfo::getUserCount() const {
    return userCount;
}

void RoomInfo::setUserCount(int count) {
    userCount = count;
}
