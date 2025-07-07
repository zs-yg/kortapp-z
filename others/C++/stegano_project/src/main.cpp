#include "GUI.hpp"
#include <FL/Fl.H>
#include <Windows.h>

int main() {
    // 确保控制台使用UTF-8编码
    SetConsoleOutputCP(CP_UTF8);
    
    MainWindow win(800, 600, "Zero-Width Steganography");
    win.show();
    
    return Fl::run();
}