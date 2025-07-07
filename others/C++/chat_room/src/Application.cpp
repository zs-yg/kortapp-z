#include "Application.hpp"
#include "MainWindow.hpp"
#include <FL/Fl.H>
#include <iostream>

Application::Application(int argc, char** argv) 
    : mainWindow(nullptr) {
    try {
        std::cout << "正在初始化应用程序..." << std::endl;
        mainWindow = new MainWindow(400, 300, "局域网聊天室");
        std::cout << "主窗口创建成功" << std::endl;
    } catch (const std::exception& e) {
        std::cerr << "初始化失败: " << e.what() << std::endl;
    }
}

int Application::run() {
    std::cout << "应用程序启动..." << std::endl;
    
    if (!mainWindow) {
        std::cerr << "致命错误: 主窗口对象创建失败" << std::endl;
        return 1;
    }

    try {
        std::cout << "初始化FLTK图形界面..." << std::endl;
        Fl::scheme("gtk+");
        
        std::cout << "显示主窗口..." << std::endl;
        mainWindow->show();
        
        std::cout << "窗口尺寸: " << mainWindow->w() << "x" << mainWindow->h() << std::endl;
        std::cout << "进入主事件循环..." << std::endl;
        
        int ret = Fl::run();
        std::cout << "应用程序正常退出" << std::endl;
        return ret;
    } catch (const std::exception& e) {
        std::cerr << "运行时错误: " << e.what() << std::endl;
        return 1;
    }
}

Application::~Application() {
    delete mainWindow;
}
