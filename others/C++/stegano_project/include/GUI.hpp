// include/GUI.hpp (添加类前向声明)
#include <FL/Fl.H>
#include <FL/Fl_Window.H>
#include <FL/Fl_Tabs.H>
#include <FL/Fl_Group.H>
#include <FL/Fl_Button.H>
#include <FL/Fl_Input.H>
#include <FL/Fl_Multiline_Input.H>
#include <FL/Fl_Multiline_Output.H>
#include <FL/Fl_Box.H>
#include "Steganography.hpp"

class MainWindow : public Fl_Window {
public:
    MainWindow(int width, int height, const char* title);
    
private:
    Fl_Tabs* tabs;
    Fl_Group* embedGroup;
    Fl_Group* extractGroup;
    
    // Embed widgets
    Fl_Multiline_Input* coverInput;
    Fl_Multiline_Input* messageInput;
    Fl_Button* encodeBtn;
    Fl_Multiline_Output* resultOutput;
    
    // Extract widgets
    Fl_Multiline_Input* extractInput;
    Fl_Button* decodeBtn;
    Fl_Multiline_Output* extractedOutput;
    
    // Callbacks
    static void encode_cb(Fl_Widget* w, void* data);
    static void decode_cb(Fl_Widget* w, void* data);
};