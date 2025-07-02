#include "utils.h"
#include <stdlib.h>
#include <string.h>
#include <time.h>

char* wchar_to_mb(const wchar_t* wstr) {
    if (!wstr) return NULL;

    int size = WideCharToMultiByte(CP_UTF8, 0, wstr, -1, NULL, 0, NULL, NULL);
    char* mbstr = (char*)malloc(size);
    if (mbstr) {
        WideCharToMultiByte(CP_UTF8, 0, wstr, -1, mbstr, size, NULL, NULL);
    }
    return mbstr;
}

wchar_t* mb_to_wchar(const char* str) {
    if (!str) return NULL;

    int size = MultiByteToWideChar(CP_UTF8, 0, str, -1, NULL, 0);
    wchar_t* wstr = (wchar_t*)malloc(size * sizeof(wchar_t));
    if (wstr) {
        MultiByteToWideChar(CP_UTF8, 0, str, -1, wstr, size);
    }
    return wstr;
}

char* get_current_time_string() {
    time_t now;
    time(&now);
    struct tm* timeinfo = localtime(&now);
    
    char* time_str = (char*)malloc(20);
    if (time_str) {
        strftime(time_str, 20, "%Y-%m-%d %H:%M:%S", timeinfo);
    }
    return time_str;
}
