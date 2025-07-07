#include "UserManager.hpp"

void UserManager::addUser(std::shared_ptr<User> user) {
    users.push_back(user);
}

void UserManager::removeUser(const std::string& username) {
    users.erase(
        std::remove_if(users.begin(), users.end(),
            [&username](const std::shared_ptr<User>& user) {
                return user->getUsername() == username;
            }),
        users.end());
}

std::vector<std::shared_ptr<User>> UserManager::getUsers() const {
    return users;
}

std::shared_ptr<User> UserManager::findUser(const std::string& username) const {
    auto it = std::find_if(users.begin(), users.end(),
        [&username](const std::shared_ptr<User>& user) {
            return user->getUsername() == username;
        });
    return it != users.end() ? *it : nullptr;
}
