#ifndef CHATWINDOW_HPP
#define CHATWINDOW_HPP

#include <FL/Fl_Window.H>
#include <FL/Fl_Text_Display.H>
#include <FL/Fl_Text_Buffer.H>
#include <FL/Fl_Input.H>
#include <FL/Fl_Button.H>
#include <FL/Fl_Browser.H>
#include <memory>
#include "ChatRoom.hpp"

class ChatWindow : public Fl_Window {
public:
    ChatWindow(int w, int h, const char* title, std::shared_ptr<ChatRoom> chatRoom);
    
    void appendMessage(const std::string& message);
    void updateUserList();

private:
    std::shared_ptr<ChatRoom> chatRoom;
    Fl_Text_Display* messageDisplay;
    Fl_Text_Buffer* messageBuffer;
    Fl_Input* messageInput;
    Fl_Button* sendButton;
    Fl_Browser* userList;
    
    static void onSendMessageCallback(Fl_Widget* w, void* data);
};

#endif // CHATWINDOW_HPP
