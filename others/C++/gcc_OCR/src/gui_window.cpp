#include "../include/gui_window.hpp"
#include "../include/file_io.hpp"
#include "../include/ocr_engine.hpp"
#include <FL/Fl_File_Chooser.H>
#include <thread>

// 异常信息结构体
struct ExceptionData {
    GUIWindow* window;
    String errorMsg;
};

GUIWindow::GUIWindow(int width = 900, int height = 700, const char* title = "OCR识别工具") {
    window = new Fl_Window(width, height, title);
    initUI();
    window->end();
    window->show();
}

GUIWindow::~GUIWindow() {
    delete textBuffer;
    delete window;
}

void GUIWindow::initUI() {
    textBuffer = new Fl_Text_Buffer();
    textDisplay = new Fl_Text_Display(20, 20, 860, 550, "识别结果");
    textDisplay->buffer(textBuffer);
    
    // 添加语言选择下拉菜单
    languageChoice = new Fl_Choice(70, 580, 300, 25, "识别语言");
    for (int i = 0; i < 3; ++i) {
        languageChoice->add(LANGUAGES[i]);
    }
    languageChoice->value(0); // 默认选择英文
    
    openButton = new Fl_Button(390, 580, 150, 25, "打开图片");
    openButton->callback(openCallback, this);
    
    saveButton = new Fl_Button(550, 580, 150, 25, "保存结果");
    saveButton->callback(saveCallback, this);
}

void GUIWindow::setOCRResult(const String& text) {
    textBuffer->text(text.c_str());
}

String GUIWindow::getLanguage() const {
    return LANGUAGE_CODES[languageChoice->value()];
}

void GUIWindow::handleException(void* data) {
    ExceptionData* exData = static_cast<ExceptionData*>(data);
    exData->window->setOCRResult("无法识别");
    exData->window->enableButtons();
    fl_alert("识别错误: %s", exData->errorMsg.c_str());
    delete exData;
}

void GUIWindow::openCallback(Fl_Widget* w, void* data) {
    (void)w;
    GUIWindow* win = static_cast<GUIWindow*>(data);
    const char* filename = fl_file_chooser("选择图片文件", "*.{png,jpg,bmp}", "");
    if (filename) {
        win->setOCRResult("正在识别...");
        win->disableButtons();
        
        std::thread([win, filename]() {
            try {
                OCREngine ocr;
                if (!ocr.setLanguage(win->getLanguage())) {
                    throw std::runtime_error("无法设置识别语言");
                }
                String result = ocr.recognizeFromFile(filename);
                Fl::awake([](void* data) {
                    auto* ctx = static_cast<std::pair<GUIWindow*, String>*>(data);
                    ctx->first->setOCRResult(ctx->second.empty() ? "无法识别" : ctx->second);
                    ctx->first->enableButtons();
                    delete ctx;
                }, new std::pair<GUIWindow*, String>(win, result));
            } catch (const std::exception& e) {
                ExceptionData* exData = new ExceptionData{win, e.what()};
                Fl::awake(handleException, exData);
            }
        }).detach();
    }
}

void GUIWindow::disableButtons() {
    openButton->deactivate();
    saveButton->deactivate();
}

void GUIWindow::enableButtons() {
    openButton->activate();
    saveButton->activate();
}

void GUIWindow::saveCallback(Fl_Widget* w, void* data) {
    (void)w;
    GUIWindow* win = static_cast<GUIWindow*>(data);
    const char* filename = fl_file_chooser("保存识别结果", "*.txt", "");
    if (filename && win->textBuffer->length() > 0) {
        saveTextToFile(filename, win->textBuffer->text());
    }
}
