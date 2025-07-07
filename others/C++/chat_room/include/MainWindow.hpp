#ifndef MAINWINDOW_HPP
#define MAINWINDOW_HPP

#include <FL/Fl.H>
#include <FL/Fl_Window.H>
#include <FL/Fl_Button.H>
#include <FL/Fl_Box.H>
#include <FL/Fl_Input.H>
#include <FL/Fl_Output.H>
#include <memory>
#include "ChatRoom.hpp"

class MainWindow : public Fl_Window {
public:
    MainWindow(int w, int h, const char* title);
    ~MainWindow();

    void showCreateRoomDialog();
    void showJoinRoomDialog();
    void showUsernameDialog();

private:
    Fl_Button* createRoomBtn;
    Fl_Button* joinRoomBtn;
    Fl_Box* titleBox;
    
    Fl_Window* createRoomDialog;
    Fl_Window* joinRoomDialog;
    Fl_Window* usernameDialog;
    
    Fl_Input* portInput;
    Fl_Input* ipInput;
    Fl_Input* passwordInput;
    Fl_Input* usernameInput;
    
    std::unique_ptr<ChatRoom> chatRoom;

    static void onCreateRoomCallback(Fl_Widget* w, void* data);
    static void onJoinRoomCallback(Fl_Widget* w, void* data);
    static void onCreateRoomConfirmCallback(Fl_Widget* w, void* data);
    static void onJoinRoomConfirmCallback(Fl_Widget* w, void* data);
    static void onUsernameConfirmCallback(Fl_Widget* w, void* data);
};

#endif // MAINWINDOW_HPP
