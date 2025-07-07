#include "ChatWindow.hpp"
#include <FL/Fl.H>
#include <iostream>

ChatWindow::ChatWindow(int w, int h, const char* title, std::shared_ptr<ChatRoom> chatRoom)
    : Fl_Window(w, h, title), chatRoom(chatRoom) {
    
    // 设置消息回调
    auto networkManager = chatRoom->getNetworkManager();
    networkManager->setMessageCallback([this](const std::string& msg) {
        this->appendMessage(msg);
    });
    
    messageBuffer = new Fl_Text_Buffer();
    messageDisplay = new Fl_Text_Display(20, 20, w-220, h-70);
    messageDisplay->buffer(messageBuffer);
    
    userList = new Fl_Browser(w-190, 20, 170, h-70);
    userList->type(FL_MULTI_BROWSER);
    
    messageInput = new Fl_Input(20, h-40, w-100, 30);
    sendButton = new Fl_Button(w-70, h-40, 60, 30, "发送");
    sendButton->callback(onSendMessageCallback, this);
    
    updateUserList();
}

void ChatWindow::appendMessage(const std::string& message) {
    messageBuffer->append(message.c_str());
    messageBuffer->append("\n");
    messageDisplay->scroll(messageBuffer->count_lines(0, messageBuffer->length()), 0);
}

void ChatWindow::updateUserList() {
    userList->clear();
    for (const auto& user : chatRoom->getUsers()) {
        userList->add(user->getUsername().c_str());
    }
}

void ChatWindow::onSendMessageCallback(Fl_Widget* w, void* data) {
    ChatWindow* win = static_cast<ChatWindow*>(data);
    std::string message = win->messageInput->value();
    
    if (!message.empty()) {
        win->chatRoom->sendChatMessage(message);
        win->messageInput->value("");
        Fl::focus(win->messageInput);
    }
}
