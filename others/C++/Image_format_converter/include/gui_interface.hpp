#pragma once
#include <FL/Fl.H>
#include <FL/Fl_Window.H>
#include <FL/Fl_Button.H>
#include <FL/Fl_Input.H>
#include <FL/Fl_Output.H>
#include <FL/Fl_Choice.H>
#include <string>

class MainWindow : public Fl_Window {
public:
    MainWindow(int w, int h, const char* title);
    
private:
    Fl_Input* input_path;
    Fl_Output* output_path;
    Fl_Choice* format_choice;
    Fl_Button* convert_btn;
    
    static void input_file_cb(Fl_Widget* w, void* data);
    static void output_file_cb(Fl_Widget* w, void* data);
    static void convert_cb(Fl_Widget* w, void* data);
};
