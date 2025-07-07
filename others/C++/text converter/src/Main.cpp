#include "../include/MainWindow.hpp"
#include "../include/Config.hpp"
#include <FL/Fl.H>

int main(int argc, char **argv) {
    // 创建主窗口
    MainWindow window(Config::WINDOW_WIDTH, Config::WINDOW_HEIGHT, Config::WINDOW_TITLE);
    
    // 显示窗口
    window.show(argc, argv);
    
    // 运行FLTK主循环
    return Fl::run();
}
