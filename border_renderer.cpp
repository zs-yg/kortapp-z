#include <windows.h>
#include <vector>
#include <fstream>
#include <cmath>
#include <d2d1.h>

// 高性能边框路径生成
void GenerateBorderPath(int width, int height, int radius, const char* outputPath) {
    std::ofstream out(outputPath);
    const float pi = 3.1415926f;
    const int segments = 24; // 高分段数确保平滑
    
    std::vector<POINTFLOAT> points;
    
    // 优化后的圆角路径生成
    auto addArc = [&](float startAngle, float endAngle, float cx, float cy) {
        for (int i = 0; i <= segments; ++i) {
            float angle = startAngle + (endAngle - startAngle) * i / segments;
            points.push_back({
                cx + radius * cosf(angle),
                cy + radius * sinf(angle)
            });
        }
    };

    // 左上角
    addArc(pi, 3*pi/2, radius, radius);
    
    // 右上角 
    addArc(3*pi/2, 2*pi, width - radius, radius);
    
    // 右下角
    addArc(0, pi/2, width - radius, height - radius);
    
    // 左下角
    addArc(pi/2, pi, radius, height - radius);

    // 闭合路径
    points.push_back(points[0]);
    
    // 写入优化格式
    if (out.is_open()) {
        for (const auto& p : points) {
            out << p.x << "," << p.y << "\n";
        }
    }
}

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int) {
    int argc;
    LPWSTR* argv = CommandLineToArgvW(GetCommandLineW(), &argc);
    
    if (argc != 5) return 1;
    
    int width = _wtoi(argv[1]);
    int height = _wtoi(argv[2]);
    int radius = _wtoi(argv[3]);
    
    char outputPath[MAX_PATH];
    wcstombs(outputPath, argv[4], MAX_PATH);
    
    GenerateBorderPath(width, height, radius, outputPath);
    return 0;
}
