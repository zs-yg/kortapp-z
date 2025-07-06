#include "../include/common.hpp"
#include "../include/gui_window.hpp"
#include "../include/ocr_engine.hpp"

int main(int argc, char** argv) {
    (void)argc;
    (void)argv;
    try {
        // 初始化GUI窗口
        GUIWindow window(900, 700, "OCR识别器");
        
        // 初始化OCR引擎
        OCREngine ocr;
        
        // 运行主循环
        return Fl::run();
    } catch (const std::exception& e) {
        std::cerr << "错误: " << e.what() << std::endl;
        return EXIT_FAILURE;
    }
}
