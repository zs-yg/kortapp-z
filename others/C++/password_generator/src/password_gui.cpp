#include "password_gui.hpp"
#include <cstdlib>

PasswordGUI::PasswordGUI() {
    window = new Fl_Window(400, 350, "密码生成器");
    
    lengthInput = new Fl_Input(150, 50, 200, 30, "密码长度:");
    modeChoice = new Fl_Choice(150, 90, 200, 30, "密码模式:");
    modeChoice->add("纯数字");
    modeChoice->add("纯英文");
    modeChoice->add("纯符号");
    modeChoice->add("数字+英文");
    modeChoice->add("数字+符号");
    modeChoice->add("英文+符号");
    modeChoice->value(0); // 默认选择纯数字模式
    
    generateButton = new Fl_Button(150, 140, 100, 40, "生成密码");
    passwordOutput = new Fl_Output(100, 210, 250, 40, "生成结果:");

    generateButton->callback(onGenerate, this);
    window->end();
}

void PasswordGUI::show() {
    window->show();
}

void PasswordGUI::onGenerate(Fl_Widget* widget, void* data) {
    PasswordGUI* gui = static_cast<PasswordGUI*>(data);
    const char* lengthStr = gui->lengthInput->value();
    int length = atoi(lengthStr);
    
    PasswordGenerator::Mode mode = static_cast<PasswordGenerator::Mode>(gui->modeChoice->value());
    String password = gui->generator.generate(length, mode);
    gui->passwordOutput->value(password.c_str());
}
