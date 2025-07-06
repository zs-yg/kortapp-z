#include "../include/ocr_engine.hpp"
#include "../include/error_handler.hpp"
#include <tesseract/baseapi.h>
#include <leptonica/allheaders.h>

OCREngine::OCREngine() : api(new tesseract::TessBaseAPI()), currentLanguage("eng") {
    initTesseract();
}

OCREngine::~OCREngine() {
    if (api) {
        api->End();
        delete api;
    }
}

bool OCREngine::setLanguage(const String& lang) {
    // 支持多语言混合识别，如"eng+chi_sim"
    if (api->Init("C:/msys64/ucrt64/share/tessdata/", lang.c_str())) {
        // 尝试回退到英文
        if (api->Init("C:/msys64/ucrt64/share/tessdata/", "eng")) {
            THROW_EXCEPTION("无法设置OCR语言: " + lang + " 且回退到英文失败");
            return false;
        }
        currentLanguage = "eng";
        return false;
    }
    currentLanguage = lang;
    
    // 针对中文优化识别参数
    if (lang.find("chi") != String::npos) {
        api->SetVariable("tessedit_char_whitelist", 
            "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
            "的一是在不了有和人这中大为上个国我以要他时来用们生到作地于出就分对成会可主发年动同工也能下过子说产种面而方后多定行学法所民得经十三之进着等部度家电力里如水化高自二理起小物现实加量都两体制机当使点从业本去把性好应开它合还因由其些然前外天政四日那社义事平形相全表间样与关各重新线内数正心反你明看原又么利比或但质气第向道命此变条只没结解问意建月公无系军很情者最立代想已通并提直题党程展五果料象员革位入常文总次品式活设及管特件长求老头基资边流路级少图山统接知较将组见计别她手角期根论运农指几九区强放决西被干做必战先回则任取据处队南给色光门即保治北造百规热领七海口东导器压志世金增争济阶油思术极交受联什认六共权收证改清己美再采转更单风切打白教速花带安场身车例真务具万每目至达走积示议声报斗完类八离华名确才科张信马节话米整空元况今集温传土许步群广石记需段研界拉林律叫且究观越织装影算低持音众书布复容儿须际商非验连断深难近矿千周委素技备半办青省列习响约支般史感劳便团往酸历市克何除消构府称太准精值号率族维划选标写存候毛亲快效斯院查江型眼王按格养易置派层片始却专状育厂京识适属圆包火住调满县局照参红细引听该铁价严龙飞");
        api->SetPageSegMode(tesseract::PSM_SPARSE_TEXT_OSD);
    }
    return true;
}

String OCREngine::recognizeFromFile(const String& filePath) {
    Pix* image = pixRead(filePath.c_str());
    if (!image) {
        THROW_EXCEPTION("无法加载图像文件: " + filePath);
    }
    
    String result = recognizeFromImage(image);
    pixDestroy(&image);
    return result;
}

String OCREngine::recognizeFromImage(Pix* image) {
    api->SetImage(image);
    char* text = api->GetUTF8Text();
    if (!text) {
        THROW_EXCEPTION("OCR识别失败");
    }
    
    String result(text);
    delete[] text;
    return result;
}

void OCREngine::initTesseract() {
    if (api->Init("C:/msys64/ucrt64/share/tessdata/", currentLanguage.c_str())) {
        THROW_EXCEPTION("无法初始化Tesseract OCR引擎");
    }
    api->SetPageSegMode(tesseract::PSM_AUTO);
}
