#include "about_dialog.h"
#include <tchar.h>

void show_about_dialog(HWND hParent) {
    MessageBox(hParent, 
        _T("系统信息查看器\n版本 1.0\n\n一个简单的Windows系统信息工具"),
        _T("关于"), 
        MB_OK | MB_ICONINFORMATION);
}
