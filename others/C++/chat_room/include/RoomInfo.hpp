#ifndef ROOMINFO_HPP
#define ROOMINFO_HPP

#include <string>

class RoomInfo {
public:
    RoomInfo(const std::string& name, 
             const std::string& ip,
             int port,
             const std::string& password);
             
    const std::string& getName() const;
    const std::string& getIpAddress() const;
    int getPort() const;
    const std::string& getPassword() const;
    int getUserCount() const;
    
    void setUserCount(int count);
    
private:
    std::string name;
    std::string ipAddress;
    int port;
    std::string password;
    int userCount;
};

#endif // ROOMINFO_HPP
