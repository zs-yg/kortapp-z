#include "config.h"
#include "string_utils.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define MAX_CONFIG_SIZE 1024

typedef struct ConfigEntry {
    char* key;
    char* value;
    struct ConfigEntry* next;
} ConfigEntry;

static ConfigEntry* config_list = NULL;

static ConfigEntry* config_find_entry(const char* key) {
    ConfigEntry* entry = config_list;
    while (entry != NULL) {
        if (strcmp(entry->key, key) == 0) {
            return entry;
        }
        entry = entry->next;
    }
    return NULL;
}

bool config_load(const char* config_file) {
    FILE* file = fopen(config_file, "r");
    if (file == NULL) {
        return false;
    }

    char line[MAX_CONFIG_SIZE];
    while (fgets(line, sizeof(line), file) != NULL) {
        // 去除注释和空白行
        char* comment = strchr(line, '#');
        if (comment != NULL) {
            *comment = '\0';
        }

        str_trim(line);
        if (strlen(line) == 0) {
            continue;
        }

        // 解析键值对
        char* separator = strchr(line, '=');
        if (separator == NULL) {
            continue;
        }

        *separator = '\0';
        char* key = str_trim(line);
        char* value = str_trim(separator + 1);

        // 添加到配置列表
        config_set_string(key, value);
    }

    fclose(file);
    return true;
}

bool config_save(const char* config_file) {
    FILE* file = fopen(config_file, "w");
    if (file == NULL) {
        return false;
    }

    ConfigEntry* entry = config_list;
    while (entry != NULL) {
        fprintf(file, "%s=%s\n", entry->key, entry->value);
        entry = entry->next;
    }

    fclose(file);
    return true;
}

const char* config_get_string(const char* key, const char* default_value) {
    ConfigEntry* entry = config_find_entry(key);
    if (entry != NULL) {
        return entry->value;
    }
    return default_value;
}

int config_get_int(const char* key, int default_value) {
    const char* str = config_get_string(key, NULL);
    if (str != NULL) {
        return atoi(str);
    }
    return default_value;
}

bool config_get_bool(const char* key, bool default_value) {
    const char* str = config_get_string(key, NULL);
    if (str != NULL) {
        return strcmp(str, "true") == 0 || strcmp(str, "1") == 0;
    }
    return default_value;
}

void config_set_string(const char* key, const char* value) {
    ConfigEntry* entry = config_find_entry(key);
    if (entry != NULL) {
        free(entry->value);
        entry->value = strdup(value);
    } else {
        entry = malloc(sizeof(ConfigEntry));
        entry->key = strdup(key);
        entry->value = strdup(value);
        entry->next = config_list;
        config_list = entry;
    }
}

void config_set_int(const char* key, int value) {
    char str[32];
    snprintf(str, sizeof(str), "%d", value);
    config_set_string(key, str);
}

void config_set_bool(const char* key, bool value) {
    config_set_string(key, value ? "true" : "false");
}
