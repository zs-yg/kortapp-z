#ifndef CONFIG_H
#define CONFIG_H

#include <stdbool.h>

/**
 * 加载配置文件
 * @param config_file 配置文件路径
 * @return true表示成功，false表示失败
 */
bool config_load(const char* config_file);

/**
 * 保存配置文件
 * @param config_file 配置文件路径
 * @return true表示成功，false表示失败
 */
bool config_save(const char* config_file);

/**
 * 获取字符串配置值
 * @param key 配置键
 * @param default_value 默认值
 * @return 配置值
 */
const char* config_get_string(const char* key, const char* default_value);

/**
 * 获取整数配置值
 * @param key 配置键
 * @param default_value 默认值
 * @return 配置值
 */
int config_get_int(const char* key, int default_value);

/**
 * 获取布尔配置值
 * @param key 配置键
 * @param default_value 默认值
 * @return 配置值
 */
bool config_get_bool(const char* key, bool default_value);

/**
 * 设置字符串配置值
 * @param key 配置键
 * @param value 配置值
 */
void config_set_string(const char* key, const char* value);

/**
 * 设置整数配置值
 * @param key 配置键
 * @param value 配置值
 */
void config_set_int(const char* key, int value);

/**
 * 设置布尔配置值
 * @param key 配置键
 * @param value 配置值
 */
void config_set_bool(const char* key, bool value);

#endif // CONFIG_H
