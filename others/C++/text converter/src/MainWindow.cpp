#include "../include/MainWindow.hpp"
#include "../include/Utils.hpp"
#include "../include/Config.hpp"
#include <FL/Fl_Box.H>
#include <FL/fl_ask.H>

MainWindow::MainWindow(int w, int h, const char* title) : Fl_Window(w, h, title) {
    // 初始化UI元素
    inputText = new Fl_Input(Config::INPUT_X, Config::INPUT_Y, 
                            Config::INPUT_WIDTH, Config::INPUT_HEIGHT, "输入文本:");
    outputText = new Fl_Output(Config::OUTPUT_X, Config::OUTPUT_Y, 
                              Config::OUTPUT_WIDTH, Config::OUTPUT_HEIGHT, "输出结果:");
    conversionType = new Fl_Choice(Config::CHOICE_X, Config::CHOICE_Y, 
                                  Config::CHOICE_WIDTH, Config::CHOICE_HEIGHT, "转换类型:");
    convertButton = new Fl_Button(Config::BUTTON_X, Config::BUTTON_Y, 
                                Config::BUTTON_WIDTH, Config::BUTTON_HEIGHT, "转换");

    // 设置转换类型选项
    conversionType->add("二进制");
    conversionType->add("十六进制");
    conversionType->add("ROT13");
    conversionType->add("MD5");
    conversionType->add("SHA1");
    conversionType->add("SHA256");
    conversionType->add("SHA224");
    conversionType->add("SHA384");
    conversionType->add("SHA512");
    conversionType->add("SHA3");
    conversionType->add("Base64");
    conversionType->add("Base32");
    conversionType->add("Ascii85");
    conversionType->add("CRC32");
    conversionType->value(0); // 默认选择二进制

    // 设置按钮回调
    convertButton->callback(ConvertCallback, this);

    end(); // 结束窗口组件添加
}

MainWindow::~MainWindow() {
    // 清理资源
    delete inputText;
    delete outputText;
    delete conversionType;
    delete convertButton;
}

void MainWindow::ConvertCallback(Fl_Widget* widget, void* data) {
    MainWindow* window = static_cast<MainWindow*>(data);
    window->ConvertText();
}

void MainWindow::ConvertText() {
    try {
        const char* input = inputText->value();
        if (!input || strlen(input) == 0) {
            fl_alert("请输入要转换的文本");
            return;
        }

        int type = conversionType->value();
        auto converter = Utils::createConverter(type);
        if (!converter) {
            fl_alert("不支持的转换类型");
            return;
        }

        std::string result = converter->convert(input);
        outputText->value(result.c_str());
    } catch (const std::exception& e) {
        fl_alert(("转换失败: " + std::string(e.what())).c_str());
    } catch (...) {
        fl_alert("未知错误: 转换失败");
    }
}
