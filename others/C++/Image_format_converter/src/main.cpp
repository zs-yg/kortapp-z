#include "gui_interface.hpp"
#include <FL/Fl.H>

int main(int argc, char** argv) {
    MainWindow window(400, 300, "Image Format Converter");
    window.show(argc, argv);
    return Fl::run();
}
