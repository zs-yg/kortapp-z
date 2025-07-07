// src/GUI.cpp (改进布局)
#include "GUI.hpp"
#include "Steganography.hpp"
#include "CommonDefs.hpp"
#include <FL/fl_ask.H>
#include <string>

const int PAD = 15;
const int WIDTH = 600;
const int HEIGHT = 600;
const int BTN_WIDTH = 120;
const int BTN_HEIGHT = 30;

MainWindow::MainWindow(int width, int height, const char* title) 
    : Fl_Window(width, height, title) {
    
    begin();
    
    // 创建标签页
    tabs = new Fl_Tabs(0, 0, WIDTH, HEIGHT);
    tabs->selection_color(FL_BLUE);
    
    // 嵌入标签页
    embedGroup = new Fl_Group(0, 30, WIDTH, HEIGHT - 30, "Embed");
    embedGroup->color(FL_WHITE);
    embedGroup->selection_color(FL_WHITE);
    
    int y = PAD + 30;
    
    coverInput = new Fl_Multiline_Input(PAD, y, WIDTH - 2 * PAD, 80, "Cover Text:");
    coverInput->align(FL_ALIGN_TOP_LEFT);
    y += 90;
    
    messageInput = new Fl_Multiline_Input(PAD, y, WIDTH - 2 * PAD, 80, "Secret Message:");
    messageInput->align(FL_ALIGN_TOP_LEFT);
    y += 90;
    
    encodeBtn = new Fl_Button(WIDTH - BTN_WIDTH - PAD, y, BTN_WIDTH, BTN_HEIGHT, "Embed");
    encodeBtn->callback(encode_cb, this);
    encodeBtn->color(FL_GREEN);
    y += 40;
    
    resultOutput = new Fl_Multiline_Output(PAD, y, WIDTH - 2 * PAD, 80, "Result (with hidden text):");
    resultOutput->align(FL_ALIGN_TOP_LEFT);
    resultOutput->textsize(12);
    resultOutput->readonly(1);
    y += 90;
    
    // 状态标签
    Fl_Box* statusBox = new Fl_Box(PAD, y, WIDTH - 2 * PAD, 30, "");
    statusBox->label("Status: Ready");
    
    embedGroup->end();
    
    // 提取标签页
    extractGroup = new Fl_Group(0, 30, WIDTH, HEIGHT - 30, "Extract");
    extractGroup->color(FL_WHITE);
    extractGroup->selection_color(FL_WHITE);
    
    y = PAD + 30;
    
    extractInput = new Fl_Multiline_Input(PAD, y, WIDTH - 2 * PAD, 80, "Text to decode:");
    extractInput->align(FL_ALIGN_TOP_LEFT);
    y += 90;
    
    decodeBtn = new Fl_Button(WIDTH - BTN_WIDTH - PAD, y, BTN_WIDTH, BTN_HEIGHT, "Extract");
    decodeBtn->callback(decode_cb, this);
    decodeBtn->color(FL_BLUE);
    y += 40;
    
    extractedOutput = new Fl_Multiline_Output(PAD, y, WIDTH - 2 * PAD, 80, "Extracted Message:");
    extractedOutput->align(FL_ALIGN_TOP_LEFT);
    extractedOutput->textsize(12);
    extractedOutput->readonly(1);
    y += 90;
    
    // 状态标签
    Fl_Box* extractStatus = new Fl_Box(PAD, y, WIDTH - 2 * PAD, 30, "");
    extractStatus->label("Status: Ready");
    
    extractGroup->end();
    tabs->end();
    
    end();
    
    // 设置默认选择的标签
    tabs->value(embedGroup);
    
    // 窗口美化
    color(FL_WHITE);
    resizable(this);
}
