#include <locale.h>
#include "gui_interface.hpp"
#include "png_to_jpg.hpp"
#include "jpg_to_png.hpp"
#include "bmp_to_png.hpp"
#include "bmp_to_jpg.hpp"
#include "png_to_bmp.hpp"
#include "jpg_to_bmp.hpp"
#include "tiff_to_bmp.hpp"
#include "bmp_to_tiff.hpp"
#include "tiff_to_png.hpp"
#include "png_to_tiff.hpp"
#include "tiff_to_jpg.hpp"
#include "jpg_to_tiff.hpp"
#include "webp_to_jpg.hpp"
#include "webp_to_png.hpp"
#include "webp_to_bmp.hpp"
#include "webp_to_tiff.hpp"
#include "jpg_to_webp.hpp"
#include "png_to_webp.hpp"
#include "bmp_to_webp.hpp"
#include "tiff_to_webp.hpp"
#include "jpeg_to_png.hpp"
#include "jpeg_to_bmp.hpp"
#include "jpeg_to_tiff.hpp"
#include "jpeg_to_webp.hpp"
#include "png_to_jpeg.hpp"
#include "bmp_to_jpeg.hpp"
#include "tiff_to_jpeg.hpp"
#include "webp_to_jpeg.hpp"
#include "avif_to_png.hpp"
#include "avif_to_jpg.hpp"
#include "avif_to_jpeg.hpp"
#include "avif_to_bmp.hpp"
#include "avif_to_tiff.hpp"
#include "avif_to_webp.hpp"
#include "png_to_avif.hpp"
#include "jpg_to_avif.hpp"
#include "jpeg_to_avif.hpp"
#include "bmp_to_avif.hpp"
#include "tiff_to_avif.hpp"
#include "webp_to_avif.hpp"
#include <FL/Fl_File_Chooser.H>
#include <FL/fl_ask.H>
#include <stdexcept>

static void init_locale() {
    setlocale(LC_ALL, "chs");
}

MainWindow::MainWindow(int w, int h, const char* title) 
    : Fl_Window(w, h, "图像格式转换器") {
    init_locale();
    
    input_path = new Fl_Input(100, 20, 250, 25, "输入文件:");
    Fl_Button* input_btn = new Fl_Button(360, 20, 30, 25, "...");
    input_btn->callback(input_file_cb, this);
    
    output_path = new Fl_Output(100, 60, 250, 25, "输出文件:");
    Fl_Button* output_btn = new Fl_Button(360, 60, 30, 25, "...");
    output_btn->callback(output_file_cb, this);
    
    format_choice = new Fl_Choice(100, 100, 150, 25, "转换格式:");
    format_choice->add("PNG to JPG");
    format_choice->add("JPG to PNG");
    format_choice->add("BMP to PNG");
    format_choice->add("BMP to JPG");
    format_choice->add("PNG to BMP");
    format_choice->add("JPG to BMP");
    format_choice->add("TIFF to BMP");
    format_choice->add("BMP to TIFF");
    format_choice->add("TIFF to PNG");
    format_choice->add("PNG to TIFF");
    format_choice->add("TIFF to JPG");
    format_choice->add("JPG to TIFF");
    format_choice->add("WEBP to JPG");
    format_choice->add("WEBP to PNG");
    format_choice->add("WEBP to BMP");
    format_choice->add("WEBP to TIFF");
    format_choice->add("JPG to WEBP");
    format_choice->add("PNG to WEBP");
    format_choice->add("BMP to WEBP");
    format_choice->add("TIFF to WEBP");
    format_choice->add("JPEG to PNG");
    format_choice->add("JPEG to BMP");
    format_choice->add("JPEG to TIFF");
    format_choice->add("JPEG to WEBP");
    format_choice->add("PNG to JPEG");
    format_choice->add("BMP to JPEG");
    format_choice->add("TIFF to JPEG");
    format_choice->add("WEBP to JPEG");
    format_choice->add("AVIF to PNG");
    format_choice->add("AVIF to JPG");
    format_choice->add("AVIF to JPEG");
    format_choice->add("AVIF to BMP");
    format_choice->add("AVIF to TIFF");
    format_choice->add("AVIF to WEBP");
    format_choice->add("PNG to AVIF");
    format_choice->add("JPG to AVIF");
    format_choice->add("JPEG to AVIF");
    format_choice->add("BMP to AVIF");
    format_choice->add("TIFF to AVIF");
    format_choice->add("WEBP to AVIF");
    format_choice->value(0);
    
    convert_btn = new Fl_Button(150, 150, 100, 30, "转换");
    convert_btn->callback(convert_cb, this);
    
    end();
}

void MainWindow::input_file_cb(Fl_Widget* w, void* data) {
    MainWindow* win = static_cast<MainWindow*>(data);
    Fl_File_Chooser chooser(".", "*.*", Fl_File_Chooser::SINGLE, "选择输入文件");
    chooser.show();
    while(chooser.shown()) Fl::wait();
    if(chooser.value()) {
        win->input_path->value(chooser.value());
    }
}

void MainWindow::output_file_cb(Fl_Widget* w, void* data) {
    MainWindow* win = static_cast<MainWindow*>(data);
    Fl_File_Chooser chooser(".", "*.*", Fl_File_Chooser::CREATE, "选择输出文件");
    chooser.show();
    while(chooser.shown()) Fl::wait();
    if(chooser.value()) {
        win->output_path->value(chooser.value());
    }
}

void MainWindow::convert_cb(Fl_Widget* w, void* data) {
    MainWindow* win = static_cast<MainWindow*>(data);
    std::string input = win->input_path->value();
    std::string output = win->output_path->value();
    
    if (input.empty() || output.empty()) {
        fl_alert("请输入有效的文件路径!");
        return;
    }
    
    bool success = false;
    try {
        switch(win->format_choice->value()) {
            case 0: // PNG to JPG
                success = PngToJpgConverter::convert(input, output);
                break;
            case 1: // JPG to PNG
                success = JpgToPngConverter::convert(input, output);
                break;
            case 2: // BMP to PNG
                success = BmpToPngConverter::convert(input, output);
                break;
            case 3: // BMP to JPG
                success = BmpToJpgConverter::convert(input, output);
                break;
            case 4: // PNG to BMP
                success = PngToBmpConverter::convert(input, output);
                break;
            case 5: // JPG to BMP
                success = JpgToBmpConverter::convert(input, output);
                break;
            case 6: // TIFF to BMP
                success = TiffToBmpConverter::convert(input, output);
                break;
            case 7: // BMP to TIFF
                success = BmpToTiffConverter::convert(input, output);
                break;
            case 8: // TIFF to PNG
                success = TiffToPngConverter::convert(input, output);
                break;
            case 9: // PNG to TIFF
                success = PngToTiffConverter::convert(input, output);
                break;
            case 10: // TIFF to JPG
                success = TiffToJpgConverter::convert(input, output);
                break;
            case 11: // JPG to TIFF
                success = JpgToTiffConverter::convert(input, output);
                break;
            case 12: // WEBP to JPG
                success = WebpToJpgConverter::convert(input, output);
                break;
            case 13: // WEBP to PNG
                success = WebpToPngConverter::convert(input, output);
                break;
            case 14: // WEBP to BMP
                success = WebpToBmpConverter::convert(input, output);
                break;
            case 15: // WEBP to TIFF
                success = WebpToTiffConverter::convert(input, output);
                break;
            case 16: // JPG to WEBP
                success = JpgToWebpConverter::convert(input, output);
                break;
            case 17: // PNG to WEBP
                success = PngToWebpConverter::convert(input, output);
                break;
            case 18: // BMP to WEBP
                success = BmpToWebpConverter::convert(input, output);
                break;
            case 19: // TIFF to WEBP
                success = TiffToWebpConverter::convert(input, output);
                break;
            case 20: // JPEG to PNG
                success = JpegToPngConverter::convert(input, output);
                break;
            case 21: // JPEG to BMP
                success = JpegToBmpConverter::convert(input, output);
                break;
            case 22: // JPEG to TIFF
                success = JpegToTiffConverter::convert(input, output);
                break;
            case 23: // JPEG to WEBP
                success = JpegToWebpConverter::convert(input, output);
                break;
            case 24: // PNG to JPEG
                success = PngToJpegConverter::convert(input, output);
                break;
            case 25: // BMP to JPEG
                success = BmpToJpegConverter::convert(input, output);
                break;
            case 26: // TIFF to JPEG
                success = TiffToJpegConverter::convert(input, output);
                break;
            case 27: // WEBP to JPEG
                success = WebpToJpegConverter::convert(input, output);
                break;
            case 28: // AVIF to PNG
                success = AvifToPngConverter().convert(input, output);
                break;
            case 29: // AVIF to JPG
                success = AvifToJpgConverter().convert(input, output);
                break;
            case 30: // AVIF to JPEG
                success = AvifToJpegConverter().convert(input, output);
                break;
            case 31: // AVIF to BMP
                success = AvifToBmpConverter().convert(input, output);
                break;
            case 32: // AVIF to TIFF
                success = AvifToTiffConverter().convert(input, output);
                break;
            case 33: // AVIF to WEBP
                success = AvifToWebpConverter().convert(input, output);
                break;
            case 34: // PNG to AVIF
                success = PngToAvifConverter().convert(input, output);
                break;
            case 35: // JPG to AVIF
                success = JpgToAvifConverter().convert(input, output);
                break;
            case 36: // JPEG to AVIF
                success = JpegToAvifConverter().convert(input, output);
                break;
            case 37: // BMP to AVIF
                success = BmpToAvifConverter().convert(input, output);
                break;
            case 38: // TIFF to AVIF
                success = TiffToAvifConverter().convert(input, output);
                break;
            case 39: // WEBP to AVIF
                success = WebpToAvifConverter().convert(input, output);
                break;
        }
        if (!success) throw std::runtime_error("转换失败");
    } catch (const std::exception& e) {
        fl_alert("转换错误: %s", e.what());
        return;
    }
    
    if (success) {
        fl_message("转换成功!");
    } else {
        fl_alert("转换失败，请检查输入文件!");
    }
}
