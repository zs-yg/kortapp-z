#include "MainWindow.hpp"
#include "ChatWindow.hpp"
#include <FL/Fl_Input.H>
#include <FL/Fl_Output.H>
#include <iostream>
#include <string>

MainWindow::MainWindow(int w, int h, const char* title) 
    : Fl_Window(w, h, title), chatRoom(std::make_unique<ChatRoom>()) {
    
    // 初始化窗口基础属性
    color(FL_WHITE);
    begin();
    
    try {
        // 初始化界面组件
        titleBox = new Fl_Box(w/2-100, 20, 200, 30, "局域网聊天室");
        titleBox->box(FL_NO_BOX);
        titleBox->labelfont(FL_BOLD);
        titleBox->labelsize(20);
        titleBox->labelcolor(FL_BLUE);
        
        createRoomBtn = new Fl_Button(w/2-100, 100, 200, 40, "创建房间");
        createRoomBtn->callback(onCreateRoomCallback, this);
        
        joinRoomBtn = new Fl_Button(w/2-100, 160, 200, 40, "加入房间");
        joinRoomBtn->callback(onJoinRoomCallback, this);
        
        // 初始化对话框指针
        createRoomDialog = nullptr;
        joinRoomDialog = nullptr;
        usernameDialog = nullptr;
        
        end();
    } catch (const std::exception& e) {
        std::cerr << "窗口初始化失败: " << e.what() << std::endl;
        throw;
    }
}

MainWindow::~MainWindow() {
    if (createRoomDialog) delete createRoomDialog;
    if (joinRoomDialog) delete joinRoomDialog;
    if (usernameDialog) delete usernameDialog;
}

void MainWindow::showCreateRoomDialog() {
    if (createRoomDialog) return;
    
    createRoomDialog = new Fl_Window(300, 250, "创建房间");
    createRoomDialog->begin();
    
    portInput = new Fl_Input(100, 30, 180, 30, "端口:");
    passwordInput = new Fl_Input(100, 70, 180, 30, "密码:");
    Fl_Button* confirmBtn = new Fl_Button(100, 120, 100, 30, "确认");
    confirmBtn->callback(onCreateRoomConfirmCallback, this);
    
    createRoomDialog->end();
    createRoomDialog->show();
}

void MainWindow::showJoinRoomDialog() {
    if (joinRoomDialog) return;
    
    joinRoomDialog = new Fl_Window(300, 250, "加入房间");
    joinRoomDialog->begin();
    
    ipInput = new Fl_Input(100, 30, 180, 30, "IP地址:");
    portInput = new Fl_Input(100, 70, 180, 30, "端口:");
    passwordInput = new Fl_Input(100, 110, 180, 30, "密码:");
    
    Fl_Button* confirmBtn = new Fl_Button(100, 160, 100, 30, "确认");
    confirmBtn->callback(onJoinRoomConfirmCallback, this);
    
    joinRoomDialog->end();
    joinRoomDialog->show();
}

void MainWindow::showUsernameDialog() {
    if (usernameDialog) return;
    
    usernameDialog = new Fl_Window(300, 150, "输入用户名");
    usernameDialog->begin();
    
    usernameInput = new Fl_Input(100, 30, 180, 30, "用户名:");
    Fl_Button* confirmBtn = new Fl_Button(100, 80, 100, 30, "确认");
    confirmBtn->callback(onUsernameConfirmCallback, this);
    
    usernameDialog->end();
    usernameDialog->show();
}

void MainWindow::onCreateRoomCallback(Fl_Widget* w, void* data) {
    MainWindow* win = static_cast<MainWindow*>(data);
    win->showCreateRoomDialog();
}

void MainWindow::onJoinRoomCallback(Fl_Widget* w, void* data) {
    MainWindow* win = static_cast<MainWindow*>(data);
    win->showJoinRoomDialog();
}

void MainWindow::onCreateRoomConfirmCallback(Fl_Widget* w, void* data) {
    MainWindow* win = static_cast<MainWindow*>(data);
    int port = std::stoi(win->portInput->value());
    
    std::string password = win->passwordInput->value();
    if (win->chatRoom->createRoom(port, password)) {
        win->createRoomDialog->hide();
        win->showUsernameDialog();
    }
}

void MainWindow::onJoinRoomConfirmCallback(Fl_Widget* w, void* data) {
    MainWindow* win = static_cast<MainWindow*>(data);
    std::string ip = win->ipInput->value();
    int port = std::stoi(win->portInput->value());
    
    std::string password = win->passwordInput->value();
    if (win->chatRoom->joinRoom(ip, port, password)) {
        win->joinRoomDialog->hide();
        win->showUsernameDialog();
    }
}

void MainWindow::onUsernameConfirmCallback(Fl_Widget* w, void* data) {
    MainWindow* win = static_cast<MainWindow*>(data);
    std::string username = win->usernameInput->value();
    
    win->chatRoom->addUser(username);
    win->usernameDialog->hide();
    
    // 创建并显示聊天窗口
    auto chatRoomPtr = std::shared_ptr<ChatRoom>(win->chatRoom.release());
    auto chatWindow = new ChatWindow(800, 600, "聊天室", chatRoomPtr);
    chatWindow->show();
    
    // 隐藏主窗口
    win->hide();
}
