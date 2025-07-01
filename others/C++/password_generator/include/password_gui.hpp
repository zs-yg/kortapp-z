#ifndef PASSWORD_GUI_HPP
#define PASSWORD_GUI_HPP

#include <FL/Fl.H>
#include <FL/Fl_Window.H>
#include <FL/Fl_Input.H>
#include <FL/Fl_Choice.H>
#include <FL/Fl_Button.H>
#include <FL/Fl_Output.H>
#include "password_generator.hpp"

class PasswordGUI {
public:
    PasswordGUI();
    void show();

private:
    Fl_Window* window;
    Fl_Input* lengthInput;
    Fl_Choice* modeChoice;
    Fl_Button* generateButton;
    Fl_Output* passwordOutput;
    PasswordGenerator generator;

    static void onGenerate(Fl_Widget* widget, void* data);
};

#endif // PASSWORD_GUI_HPP
