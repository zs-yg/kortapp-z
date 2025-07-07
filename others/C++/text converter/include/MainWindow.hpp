#ifndef MAINWINDOW_HPP
#define MAINWINDOW_HPP

#include <FL/Fl.H>
#include <FL/Fl_Window.H>
#include <FL/Fl_Button.H>
#include <FL/Fl_Input.H>
#include <FL/Fl_Output.H>
#include <FL/Fl_Choice.H>

class MainWindow : public Fl_Window {
public:
    MainWindow(int w, int h, const char* title);
    ~MainWindow();

private:
    Fl_Input* inputText;
    Fl_Output* outputText;
    Fl_Choice* conversionType;
    Fl_Button* convertButton;

    static void ConvertCallback(Fl_Widget* widget, void* data);
    void ConvertText();
};

#endif // MAINWINDOW_HPP
