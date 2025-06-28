#include "../include/config.h"
#include <stdio.h>
#include <stdlib.h>

// 全局配置实例
static Config app_config = {
    .default_memory_size = 100,  // 默认100MB
    .max_memory_size = 8192,     // 最大8GB
    .fill_mode = 0               // 默认填充0
};

// 初始化配置
void init_config() {
    // TODO: 从文件加载配置
}

// 获取当前配置
Config* get_config() {
    return &app_config;
}

// 保存配置到文件
void save_config() {
    FILE* fp = fopen("config.bin", "wb");
    if (fp) {
        fwrite(&app_config, sizeof(Config), 1, fp);
        fclose(fp);
    }
}
