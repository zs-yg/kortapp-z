#ifndef GUI_WINDOW_HPP
#define GUI_WINDOW_HPP

#include <FL/Fl.H>
#include <FL/Fl_Window.H>
#include <FL/Fl_Button.H>
#include <FL/Fl_Text_Display.H>
#include <FL/Fl_Text_Buffer.H>
#include <FL/Fl_Choice.H>
#include "../include/common.hpp"

class GUIWindow {
public:
    GUIWindow(int width, int height, const char* title);
    ~GUIWindow();
    
    // 设置OCR结果文本
    void setOCRResult(const String& text);
    
    // 按钮状态控制
    void disableButtons();
    void enableButtons();
    
    // 获取当前语言设置
    String getLanguage() const;
    
private:
    Fl_Window* window;
    Fl_Text_Display* textDisplay;
    Fl_Text_Buffer* textBuffer;
    Fl_Button* openButton;
    Fl_Button* saveButton;
    Fl_Choice* languageChoice;
    
    // 支持的语言列表
    static constexpr const char* LANGUAGES[3] = {"英文", "简体中文", "中英文混合"};
    static constexpr const char* LANGUAGE_CODES[3] = {"eng", "chi_sim", "eng+chi_sim"};
    
    // 回调函数
    static void openCallback(Fl_Widget* w, void* data);
    static void saveCallback(Fl_Widget* w, void* data);
    
    // 初始化UI
    void initUI();
    
    // 异常处理辅助方法
    static void handleException(void* data);
};

#endif // GUI_WINDOW_HPP
