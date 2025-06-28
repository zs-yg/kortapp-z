#ifndef CONFIG_H
#define CONFIG_H

#include <stddef.h>

// 内存训练器配置
typedef struct {
    size_t default_memory_size;  // 默认内存大小(MB)
    size_t max_memory_size;     // 最大内存大小(MB)
    int fill_mode;             // 默认填充模式
} Config;

// 初始化配置
void init_config();

// 获取当前配置
Config* get_config();

// 保存配置到文件
void save_config();

#endif // CONFIG_H
