#ifndef USERMANAGER_HPP
#define USERMANAGER_HPP

#include <algorithm>
#include <vector>
#include <memory>
#include "User.hpp"

class UserManager {
public:
    void addUser(std::shared_ptr<User> user);
    void removeUser(const std::string& username);
    std::vector<std::shared_ptr<User>> getUsers() const;
    std::shared_ptr<User> findUser(const std::string& username) const;

private:
    std::vector<std::shared_ptr<User>> users;
};

#endif // USERMANAGER_HPP
