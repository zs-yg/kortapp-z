 // _              _                             
 //| | _____  _ __| |_ __ _ _ __  _ __       ____
 //| |/ / _ \| '__| __/ _` | '_ \| '_ \ ____|_  /
 //|   | (_) | |  | || (_| | |_) | |_) |_____/ / 
 //|_|\_\___/|_|   \__\__,_| .__/| .__/     /___|
 //                        |_|   |_|             
#include <iostream>
#include <fstream>
#include <vector>
#include <cmath>
#include <windows.h>

using namespace std;

struct Point {
    float x;
    float y;
};

void WritePathToFile(const vector<Point>& path, const string& filename) {
    ofstream outFile(filename);
    if (!outFile) {
        cerr << "无法打开输出文件: " << filename << endl;
        return;
    }

    for (const auto& point : path) {
        outFile << point.x << "," << point.y << "\n";
    }
    outFile.close();
}

vector<Point> CalculateRoundedRectPath(int width, int height, int radius) {
    vector<Point> pathPoints;
    
    const int segments = 10;
    const float angleStep = 3.1415926f / (2 * segments);
    
    // 左上角
    for (int i = 0; i <= segments; i++) {
        float angle = 3.1415926f + i * angleStep;
        pathPoints.push_back({
            radius + radius * cosf(angle),
            radius + radius * sinf(angle)
        });
    }
    
    // 右上角
    for (int i = 0; i <= segments; i++) {
        float angle = 3 * 3.1415926f / 2 + i * angleStep;
        pathPoints.push_back({
            width - radius + radius * cosf(angle),
            radius + radius * sinf(angle)
        });
    }
    
    // 右下角
    for (int i = 0; i <= segments; i++) {
        float angle = 0 + i * angleStep;
        pathPoints.push_back({
            width - radius + radius * cosf(angle),
            height - radius + radius * sinf(angle)
        });
    }
    
    // 左下角
    for (int i = 0; i <= segments; i++) {
        float angle = 3.1415926f / 2 + i * angleStep;
        pathPoints.push_back({
            radius + radius * cosf(angle),
            height - radius + radius * sinf(angle)
        });
    }
    
    // 闭合路径
    pathPoints.push_back(pathPoints[0]);
    
    return pathPoints;
}

int main(int argc, char* argv[]) {
    if (argc != 5) {
        cout << "用法: card_calculator [宽度] [高度] [圆角半径] [输出文件]" << endl;
        return 1;
    }

    int width = stoi(argv[1]);
    int height = stoi(argv[2]);
    int radius = stoi(argv[3]);
    string outputFile = argv[4];

    auto path = CalculateRoundedRectPath(width, height, radius);
    WritePathToFile(path, outputFile);

    return 0;
}
